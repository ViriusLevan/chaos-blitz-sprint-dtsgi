using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace LevelUpStudio.ChaosBlitzSprint.Player
{
	[RequireComponent (typeof (Rigidbody))]
	[RequireComponent (typeof (CapsuleCollider))]
	public class PlayerController : MonoBehaviour {
		[SerializeField] public PlayerInputHandler playerInputHandler;
		
		[SerializeField] private float maxSpeed = 10.0f;
		[SerializeField] private float airVelocity = 8f;
		[SerializeField] private float gravity = 10.0f;
		[SerializeField] private float maxVelocityChange = 10.0f;
		[SerializeField] private float jumpHeight = 4.2f;
		[SerializeField] private float maxFallSpeed = 20.0f;
		[SerializeField] private float rotateSpeed = 25f; //Speed the player rotate

		[SerializeField] private Animator playerAnimator;
		private CapsuleCollider capsuleCollider;
		[SerializeField] private Vector3 colliderCenterCrouch, colliderCenterNormal;
		[SerializeField] private float colliderHeightCrouch, colliderHeightNormal;
		[SerializeField] private CapsuleCollider slipCapsule;

		private Rigidbody rb;
		private Vector3 moveDir;
		private Vector3 pushDir;
		private Vector2 moveInput;
		private float distToGround;
		private float pushForce;

		private bool canMove = true; //If player is not hit
		private bool isStunned = false;
		private bool wasStunned = false; //If player was stunned before get stunned another time
		[SerializeField]private bool slide = false;
		[SerializeField]private bool sticky = false;
		private bool jumped;
		private bool isCrouching;
		public void OnJump(InputAction.CallbackContext context)
		{ 
			jumped = context.action.triggered;
			if(jumpsLeft<=0) jumped=false;
		}
		public void OnCrouch(InputAction.CallbackContext context)
		{
			isCrouching = !isCrouching;
			playerAnimator.SetBool("isCrouching", isCrouching);
			if(isCrouching)
			{
				capsuleCollider.center = colliderCenterCrouch;
				capsuleCollider.height = colliderHeightCrouch;
				slipCapsule.center = colliderCenterCrouch;
				slipCapsule.height = colliderHeightCrouch*0.8f;
				maxSpeed = speedCrouched;
			}
			else
			{
				capsuleCollider.center = colliderCenterNormal;
				capsuleCollider.height = colliderHeightNormal;
				slipCapsule.center = colliderCenterNormal;
				slipCapsule.height = colliderHeightNormal*0.8f;
				maxSpeed = speedNormal;
			}
		}

		// Double Jump PowerUp
		private int jumpsLeft = 0; // Number of jumps left, including double jumps
		private int maxJumps = 2; // Maximum number of jumps allowed, including double jumps

		public bool doubleJumpAvailable = false; // Whether the double jump power-up is available
		public bool hasExtraLife;
		public bool hasShield;

		
		private float speedNormal, speedCrouched, jumpNormal, jumpSticky;
		private PlayerInteractor playerInteractor;
		private void Awake ()
		{
			speedNormal = maxSpeed;
			speedCrouched = maxSpeed/2;
			jumpNormal = jumpHeight;
			jumpSticky = jumpHeight/2;
			capsuleCollider = GetComponent<CapsuleCollider>();
			playerInputHandler = GetComponent<PlayerInputHandler>();
			rb = GetComponent<Rigidbody>();
			playerInteractor = GetComponent<PlayerInteractor>();
			rb.freezeRotation = true;
			rb.useGravity = false;
			Cursor.visible = false;
		}

		private void Start ()
		{
			// get the distance to ground
			distToGround = GetComponent<Collider>().bounds.extents.y;
			doubleJumpAvailable = false;
			jumpsLeft = maxJumps;
		}
		private float groundedCounter=0, groundContactTime=0.2f;
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
					Quaternion destRotation = Quaternion.LookRotation(targetDir); 
					//Rotate the character little by little
					Quaternion targetRotation = Quaternion.Slerp
						(transform.rotation, destRotation, Time.deltaTime * rotateSpeed); 
					transform.rotation = targetRotation;
				}
				else
				{
					playerAnimator.SetBool("isMoving",false);
				}

				if (IsGrounded())
				{
					//Debug.Log("IS GROUNDED");
					playerAnimator.SetBool("isGrounded",true);
					groundedCounter+=Time.fixedDeltaTime;
					if(groundedCounter>=groundContactTime){
						//Debug.Log("COUNTER RESET");
						jumpsLeft = maxJumps;
						if(!doubleJumpAvailable)jumpsLeft-=1;
						groundedCounter=0;
					}
					// Calculate how fast we should be moving
					Vector3 targetVelocity = moveDir;
					targetVelocity *= maxSpeed;

					// Apply a force that attempts to reach our target velocity
					Vector3 velocity = rb.velocity;
					if (targetVelocity.magnitude < velocity.magnitude) //If I'm slowing down the character
					{
						targetVelocity = velocity;
						rb.velocity /= 1.1f;
					}

					Vector3 velocityChange = targetVelocity - velocity;
					velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
					velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
					velocityChange.y = 0;
					
					if (Mathf.Abs(rb.velocity.magnitude) < maxSpeed * 1.0f)
					{
						if (slide)
						{
							rb.AddForce(moveDir*maxSpeed*2, ForceMode.VelocityChange);
						}
						else if(sticky)
						{	
							rb.velocity /= 1.5f;
							rb.AddForce(velocityChange/10f, ForceMode.VelocityChange);
						}
						else
						{
							rb.AddForce(velocityChange, ForceMode.VelocityChange);
							//Debug.Log(rb.velocity.magnitude);
						}
					}

					// Jump
					if (jumped && jumpsLeft>0)
					{
						rb.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
						jumpsLeft -= 1;
						jumped = false;
					}
				}
				else
				{
					groundedCounter=0;
					//Debug.Log("NOT GROUNDED");
					playerAnimator.SetBool("isGrounded",false);
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
				//MIDAIR SLIDE!?
					// else if (Mathf.Abs(rb.velocity.magnitude) < speed * 1.0f)
					// {
					// 	rb.AddForce(moveDir * 0.15f, ForceMode.VelocityChange);
					// }
					if (doubleJumpAvailable && jumped)
					{
						Debug.Log("Double jump triggered");
						rb.velocity = new Vector3
							(rb.velocity.x, CalculateJumpVerticalSpeed(), rb.velocity.z);
						jumpsLeft -= 1;
						doubleJumpAvailable = false; // Disable double jump after using it
						playerInteractor.DeactivatePowerUp();
					}
					// Double jump
					// if (jumpsLeft > 0 && jumped)
					// {
						//if (!IsGrounded())
						//{
							// if (doubleJumpAvailable)
							// {
							// 	Debug.Log("Double jump triggered");
							// 	rb.velocity = new Vector3
							// 		(rb.velocity.x, CalculateJumpVerticalSpeed(), rb.velocity.z);
							// 	jumpsLeft -= 1;
							// 	doubleJumpAvailable = false; // Disable double jump after using it
							// 	playerInteractor.DeactivatePowerUp();
							// }
							// else if (jumpsLeft > 1) // Check if regular jumps are still available
							// {
							// 	rb.velocity = new Vector3
							// 		(rb.velocity.x, CalculateJumpVerticalSpeed(), rb.velocity.z);
							// 	jumpsLeft -= 1;
							// }
						//	jumped = false;
						// }
						// else // Player is on the ground
						// {
						// 	Debug.Log("You're a dipshit");
						// 	playerAnimator.SetBool("isGrounded",true);
						// 	jumpsLeft = maxJumps;
						// 	jumped = false;
						// 	//doubleJumpAvailable = true; // Reset double jump availability on landing
						// }
					//}
				}
			}
			else
			{
				rb.velocity = pushDir * pushForce;
			}
			// Apply gravity manually for more finetuned control
			rb.AddForce(new Vector3(0, -gravity * GetComponent<Rigidbody>().mass, 0));
		}
		
		private bool IsGrounded ()
		{
       		RaycastHit hit;
			return	Physics.SphereCast(
					transform.position
					,0.5f
					,-transform.up
					,out hit
					,distToGround+0.2f
					);
		}

		private void Update()
		{
			if(playerInputHandler.playerInstance.playerStatus
				!=PlayerInstance.PlayerStatus.Platforming)
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
			
			Debug.DrawRay(transform.position
					, -transform.up * (distToGround+0.2f)
					, Color.red, 3.0f);
			//if (Physics.Raycast(transform.position, -Vector3.up, out hit, distToGround + 0.2f))
			if(Physics.SphereCast(transform.position,0.5f,-transform.up
					,out hit,distToGround+0.2f))
			{
				slide = hit.collider.transform.CompareTag("Slide");
				sticky = hit.collider.transform.CompareTag("Sticky");
				if(sticky)
					jumpHeight = jumpSticky;
				else
					jumpHeight = jumpNormal;
				// Debug.Log(hit.collider.transform.gameObject.name
				// 	+" -> "+hit.collider.transform.tag+"||"+slide.ToString()+"|"+sticky.ToString());
			}
		}

		public void SendBackToSpawn()
		{
			transform.rotation = Quaternion.identity;
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
			rb.useGravity=true;
			foreach (SkinnedMeshRenderer smr in playerMeshes)
			{
				smr.enabled=true;
			}
			//TODO check again : maybe final build uses another collider
			GetComponent<CapsuleCollider>().enabled=true;
		}

		public IEnumerator TriggerDeathThenWaitToDisable(bool sink)
		{	 
			if(sink)
				playerAnimator.SetTrigger("sink");
			else
				playerAnimator.SetTrigger("die");
			// Wait for the animation to end
			playerInteractor.DeactivatePowerUp();
			yield return new WaitWhile(() => playerAnimator
				.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);
			DisableMeshAndCollider();
		}

		public void DisableMeshAndCollider()
		{
			//TODO - add more stuff, e.g. play animation or sfx
			//this.gameObject.SetActive(false);

			rb.useGravity=false;
			playerAnimator.SetTrigger("reset");
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
				
				while(playerInputHandler.playerInstance.playerStatus 
					!= PlayerInstance.PlayerStatus.Platforming){
					yield return new WaitForSeconds(0.2f);
				}

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
