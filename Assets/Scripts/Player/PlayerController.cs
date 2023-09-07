using System.Collections;
using System.Collections.Generic;
using LevelUpStudio.ChaosBlitzSprint.PowerUp;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LevelUpStudio.ChaosBlitzSprint.Player
{
	[RequireComponent (typeof (Rigidbody))]
	[RequireComponent (typeof (CapsuleCollider))]
	public class PlayerController : MonoBehaviour {
		[SerializeField] public PlayerInputHandler playerInputHandler;
		
		[SerializeField] private float speed = 10.0f;
		[SerializeField] private float airVelocity = 8f;
		[SerializeField] private float gravity = 10.0f;
		[SerializeField] private float maxVelocityChange = 10.0f;
		[SerializeField] private float jumpHeight = 2.0f;
		[SerializeField] private float maxFallSpeed = 20.0f;
		[SerializeField] private float rotateSpeed = 25f; //Speed the player rotate

		[SerializeField] private Animator playerAnimator;

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

		public bool doubleJumpAvailable = false; // Whether the double jump power-up is available
		public bool hasExtraLife;
		public bool hasShield;

		private void Start ()
		{
			// get the distance to ground
			distToGround = GetComponent<Collider>().bounds.extents.y;
			doubleJumpAvailable = false;
			jumpsLeft = maxJumps;
		}
		
		private bool IsGrounded ()
		{
			return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.2f);
		}
		
		private void Awake ()
		{
			playerInputHandler = GetComponent<PlayerInputHandler>();
			rb = GetComponent<Rigidbody>();
			rb.freezeRotation = true;
			rb.useGravity = false;
			Cursor.visible = false;
		}
		
		private void FixedUpdate ()
		{
			if(playerInputHandler.playerInstance.playerStatus 
				!= PlayerInstance.PlayerStatus.Platforming){
				return;
			}
			if (canMove)
			{
				if (moveDir.x != 0 || moveDir.z != 0)
				{
					playerAnimator.SetBool("isMoving",true);
					Vector3 targetDir = moveDir; //Direction of the character
					targetDir.y = 0;

					if (targetDir == Vector3.zero)
					{
						targetDir = transform.forward;
					}

					//Rotation of the character to where it moves
					Quaternion tr = Quaternion.LookRotation(targetDir); 
					//Rotate the character little by little
					Quaternion targetRotation = Quaternion.Slerp
						(transform.rotation, tr, Time.deltaTime * rotateSpeed); 
					transform.rotation = targetRotation;
				}
				else
				{
					playerAnimator.SetBool("isMoving",false);
				}

				if (IsGrounded())
				{
					playerAnimator.SetTrigger("jumpEnded");
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
						jumpsLeft -= 1;
						jumped = false;
					}
				}
				else
				{
					if (!slide)
					{
						Vector3 targetVelocity = new Vector3
							(moveDir.x * airVelocity, rb.velocity.y, moveDir.z * airVelocity);
						Vector3 velocity = rb.velocity;
						Vector3 velocityChange = (targetVelocity - velocity);
						velocityChange.x = Mathf.Clamp
							(velocityChange.x, -maxVelocityChange, maxVelocityChange);
						velocityChange.z = Mathf.Clamp
							(velocityChange.z, -maxVelocityChange, maxVelocityChange);
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
								Debug.Log("Double jump triggered");
								rb.velocity = new Vector3
									(rb.velocity.x, CalculateJumpVerticalSpeed(), rb.velocity.z);
								jumpsLeft -= 1;
								doubleJumpAvailable = false; // Disable double jump after using it
							}
							else if (jumpsLeft > 1) // Check if regular jumps are still available
							{
								rb.velocity = new Vector3
									(rb.velocity.x, CalculateJumpVerticalSpeed(), rb.velocity.z);
								jumpsLeft -= 1;
							}
							jumped = false;
						}
						else // Player is on the ground
						{
							playerAnimator.SetTrigger("jumpEnded");
							jumpsLeft = maxJumps;
							jumped = false;
							//doubleJumpAvailable = true; // Reset double jump availability on landing
						}
					}
				}
			}
			else
			{
				rb.velocity = pushDir * pushForce;
			}
			// Apply gravity manually for more tuning control
			rb.AddForce(new Vector3(0, -gravity * GetComponent<Rigidbody>().mass, 0));
		}

		private void Update()
		{
			if(playerInputHandler.playerInstance.playerStatus!=PlayerInstance.PlayerStatus.Platforming)
				return;
			moveInput = playerInputHandler.playerConfig.input
					.actions["Move"].ReadValue<Vector2>();
			float h = moveInput.x;
			float v = moveInput.y;

			Vector3 v2 = v * playerInputHandler
				.playerInstance.playerCamera.transform.forward; 
				//Vertical axis to which I want to move with respect to the camera
			Vector3 h2 = h * playerInputHandler
				.playerInstance.playerCamera.transform.right; 
				//Horizontal axis to which I want to move with respect to the camera
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

		public void SendBackToSpawn()
		{
			transform.position 
				= GameManager.Instance.GetPlayerSpawnPoints()[
					playerInputHandler.playerConfig.playerIndex
					].position;
		}

		private void PlayJumpFX()
		{
			VFXManager.Instance?.PlayEffect(VFXEnum.JumpEffect
				, transform.position
				, new Vector3());
			SoundManager.Instance?.PlaySound(SoundEnum.PlayerJump);
		}

		float CalculateJumpVerticalSpeed () {
			playerAnimator.SetTrigger("jumpStarted");
			PlayJumpFX();
			// From the jump height and gravity we deduce the upwards speed 
			// for the character to reach at the apex.
			return Mathf.Sqrt(2 * jumpHeight * gravity);
		}

		//[SerializeField]private MeshRenderer[]playerMeshes;
		[SerializeField]private SkinnedMeshRenderer[]playerMeshes;
		public void EnableMeshAndCollider()
		{
			foreach (SkinnedMeshRenderer smr in playerMeshes)
			{
				smr.enabled=true;
			}
			//TODO check again : maybe final build uses another collider
			GetComponent<CapsuleCollider>().enabled=true;
		}

		public IEnumerator TriggerDeathThenWaitToDisable()
		{	 
			// Wait for the animation to end
			playerAnimator.SetTrigger("die");
			yield return new WaitWhile(() => playerAnimator
				.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);
			DisableMeshAndCollider();
		}

		public void DisableMeshAndCollider()
		{
			//TODO - add more stuff, e.g. play animation or sfx
			//this.gameObject.SetActive(false);

			playerAnimator.SetTrigger("die");
			foreach (SkinnedMeshRenderer smr in playerMeshes)
			{
				smr.enabled=false;
			}
			GetComponent<CapsuleCollider>().enabled=false;
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
}
