using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (CapsuleCollider))]
public class PlayerController : MonoBehaviour {
	
	[SerializeField] private PlayerInputHandler playerInputHandler;
	[SerializeField] private float speed = 10.0f;
	[SerializeField] private float airVelocity = 8f;
	[SerializeField] private float gravity = 10.0f;
	[SerializeField] private float maxVelocityChange = 10.0f;
	[SerializeField] private float jumpHeight = 2.0f;
	[SerializeField] private float maxFallSpeed = 20.0f;
	[SerializeField] private float rotateSpeed = 25f; //Speed the player rotate
    [SerializeField] private GameObject cam;

    private Rigidbody rb;
	private Vector3 moveDir;
	private Vector3 pushDir;
    private Vector2 moveInput;
    private float distToGround;
	private float pushForce;

    private bool canMove = true; //If player is not hit
	private bool isStunned = false;
	private bool wasStunned = false; //If player was stunned before get stunned another time
	private bool slide = false;
    private bool jumped;
    public void OnJump(InputAction.CallbackContext context) => jumped = context.action.triggered;

	// Double Jump PowerUp
	private int jumpsLeft = 0; // Number of jumps left, including double jumps
	private int maxJumps = 2; // Maximum number of jumps allowed, including double jumps
	private bool doubleJumpAvailable = false; // Whether the double jump power-up is available
	public bool hasExtraLife { get; private set; }
	public bool hasShield { get; private set; }

	private void Start ()
	{
		// get the distance to ground
		distToGround = GetComponent<Collider>().bounds.extents.y;
		doubleJumpAvailable = false;
    	jumpsLeft = maxJumps;
	}
	
	private bool IsGrounded ()
	{
		return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
	}
	
	private void Awake ()
	{
		rb = GetComponent<Rigidbody>();
		rb.freezeRotation = true;
		rb.useGravity = false;
		Cursor.visible = false;
	}
	
	private void FixedUpdate ()
	{
		if (canMove)
		{
			if (moveDir.x != 0 || moveDir.z != 0)
			{
				Vector3 targetDir = moveDir; //Direction of the character
				targetDir.y = 0;

                if (targetDir == Vector3.zero)
                {
                    targetDir = transform.forward;
                }

                Quaternion tr = Quaternion.LookRotation(targetDir); //Rotation of the character to where it moves
				Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, Time.deltaTime * rotateSpeed); //Rotate the character little by little
				transform.rotation = targetRotation;
			}

			if (IsGrounded())
			{
				jumpsLeft = maxJumps;
			 	// Calculate how fast we should be moving
				Vector3 targetVelocity = moveDir;
				targetVelocity *= speed;

				// Apply a force that attempts to reach our target velocity
				Vector3 velocity = rb.velocity;
				if (targetVelocity.magnitude < velocity.magnitude) //If I'm slowing down the character
				{
					targetVelocity = velocity;
					rb.velocity /= 1.1f;
				}

				Vector3 velocityChange = (targetVelocity - velocity);
				velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
				velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
				velocityChange.y = 0;

				if (!slide)
				{
					if (Mathf.Abs(rb.velocity.magnitude) < speed * 1.0f)
						rb.AddForce(velocityChange, ForceMode.VelocityChange);
				}
				else if (Mathf.Abs(rb.velocity.magnitude) < speed * 1.0f)
				{
					rb.AddForce(moveDir * 0.15f, ForceMode.VelocityChange);
					//Debug.Log(rb.velocity.magnitude);
				}

				// Jump
				if (IsGrounded() && jumped)
				{
                    rb.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
				}
			}
			else
			{
				if (!slide)
				{
					Vector3 targetVelocity = new Vector3(moveDir.x * airVelocity * 0.25f, rb.velocity.y, moveDir.z * airVelocity * 0.25f);
					Vector3 velocity = rb.velocity;
					Vector3 velocityChange = (targetVelocity - velocity);
					velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
					velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
					rb.AddForce(velocityChange, ForceMode.VelocityChange);
					if (velocity.y < -maxFallSpeed)
						rb.velocity = new Vector3(velocity.x, -maxFallSpeed, velocity.z);
				}
				else if (Mathf.Abs(rb.velocity.magnitude) < speed * 1.0f)
				{
					rb.AddForce(moveDir * 0.15f, ForceMode.VelocityChange);
				}

				// Double jump
				if (jumpsLeft > 0 && jumped)
				{
					if (!IsGrounded())
					{
						if (doubleJumpAvailable)
						{
							rb.velocity = new Vector3(rb.velocity.x, CalculateJumpVerticalSpeed(), rb.velocity.z);
							jumpsLeft -= 1;
							doubleJumpAvailable = false; // Disable double jump after using it
						}
						else if (jumpsLeft > 1) // Check if regular jumps are still available
						{
							rb.velocity = new Vector3(rb.velocity.x, CalculateJumpVerticalSpeed(), rb.velocity.z);
							jumpsLeft -= 1;
						}
						jumped = false;
					}
					else // Player is on the ground
					{
						jumpsLeft = maxJumps;
						jumped = false;
						doubleJumpAvailable = true; // Reset double jump availability on landing
					}
				}
			}
		}
		else
		{
			rb.velocity = pushDir * pushForce;
		}
		// We apply gravity manually for more tuning control
		rb.AddForce(new Vector3(0, -gravity * GetComponent<Rigidbody>().mass, 0));
	}

	private void Update()
	{
		float h = moveInput.x;
		float v = moveInput.y;

		Vector3 v2 = v * cam.transform.forward; //Vertical axis to which I want to move with respect to the camera
		Vector3 h2 = h * cam.transform.right; //Horizontal axis to which I want to move with respect to the camera
		moveDir = (v2 + h2).normalized; //Global position to which I want to move in magnitude 1

		RaycastHit hit;
		if (Physics.Raycast(transform.position, -Vector3.up, out hit, distToGround + 0.1f))
		{
			if (hit.transform.tag == "Slide")
			{
				slide = true;
			}
			else
			{
				slide = false;
			}
		}
	}

	public void ActivateDoubleJump()
	{
		doubleJumpAvailable = true;
	}

	public void ActivateExtraLife()
	{
		hasExtraLife = true;
	}

	public void DectivateExtraLife()
	{
		hasExtraLife = false;
	}

	public void ActivateShield()
	{
		hasShield = true;
		this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
	}

	public void DeactivateShield()
	{
		hasShield = false;
		this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
	}

	private void OnCollisionEnter(Collision other) {
		if(other.gameObject.CompareTag("Goal")){
			GameManager.Instance.PlayerFinished(playerInputHandler.playerConfig.playerIndex);
		}
	}

	float CalculateJumpVerticalSpeed () {
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(2 * jumpHeight * gravity);
	}

	public void PlayerDied()
	{
		//TODO - add more stuff, e.g. play animation or sfx
		this.gameObject.SetActive(false);
	}

	public void HitPlayer(Vector3 velocityF, float time)
	{
		rb.velocity = velocityF;

		pushForce = velocityF.magnitude;
		pushDir = Vector3.Normalize(velocityF);
		StartCoroutine(Decrease(velocityF.magnitude, time));
	}

	private IEnumerator Decrease(float value, float duration)
	{
		if (isStunned)
			wasStunned = true;
		isStunned = true;
		canMove = false;

		float delta = 0;
		delta = value / duration;

		for (float t = 0; t < duration; t += Time.deltaTime)
		{
			yield return null;
			if (!slide) //Reduce the force if the ground isnt slide
			{
				pushForce = pushForce - Time.deltaTime * delta;
				pushForce = pushForce < 0 ? 0 : pushForce;
				//Debug.Log(pushForce);
			}
			rb.AddForce(new Vector3(0, -gravity * GetComponent<Rigidbody>().mass, 0)); //Add gravity
		}
		if (wasStunned)
		{
			wasStunned = false;
		}
		else
		{
			isStunned = false;
			canMove = true;
		}

	}
}
