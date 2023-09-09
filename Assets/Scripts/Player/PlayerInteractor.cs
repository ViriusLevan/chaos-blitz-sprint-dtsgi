using UnityEngine;
using UnityEngine.UI;
using LevelUpStudio.ChaosBlitzSprint.PowerUp;

namespace LevelUpStudio.ChaosBlitzSprint.Player
{
	public class PlayerInteractor : MonoBehaviour
	{
		[SerializeField] private PlayerController playerController;
		private IPowerUp powerUp;

		private void Start() 
		{
		}

		public void ActivateDoubleJump()
		{
			Debug.Log("Double Jump Activated");
			playerController.doubleJumpAvailable = true;
		}

		public void ActivateExtraLife()
		{
			Debug.Log("Extra Life Activated");
			playerController.hasExtraLife = true;
		}

		public void DeactivateExtraLife()
		{
			Debug.Log("Extra Life Deactivated");
			playerController.hasExtraLife = false;
			DeactivatePowerUp();
		}

		[SerializeField] private GameObject shield;
		public void ActivateShield()
		{
			Debug.Log("Shield Activated");
			playerController.hasShield = true;
			shield.SetActive(true);
			// this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
		}

		public void DeactivateShield()
		{
			Debug.Log("Shield Deactivated");
			playerController.hasShield = false;
			DeactivatePowerUp();
			shield.SetActive(false);
			// this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
		}

		public void DeactivatePowerUp()
		{
			powerUp = null;
			powerUpImage.sprite = null;
			powerUpImage.color  = new Color(255,255,255,0);
			//TODO yeah..
			playerController.doubleJumpAvailable = false;
			playerController.hasExtraLife = false;
			playerController.hasShield = false;
			shield.SetActive(false);
			//
		}

		private void OnCollisionEnter(Collision other) {
			switch(other.gameObject.tag){
				case "Goal":
					GameManager.Instance.PlayerFinished(playerController
						.playerInputHandler.playerConfig.playerIndex);
					break;
				case "HurtPlayer":
					Debug.Log("Player Hurt");
					if (playerController.hasExtraLife)
					{
						playerController.SendBackToSpawn();
						DeactivateExtraLife();
					}
					else if (playerController.hasShield)
					{
						DeactivateShield();
					}
					else
					{
						PlayDeathFX();
						playerController.StartCoroutine(playerController
							.TriggerDeathThenWaitToDisable(false));
						//playerController.DisableMeshAndCollider();
					}
					break;
				case "DeadZone":
					Debug.Log("Player touched a DeadZone");
					if (playerController.hasExtraLife)
					{
						playerController.SendBackToSpawn();
						DeactivateExtraLife();
					}
					else
					{
						if(other.gameObject.name.Contains("Water"))
						{
							PlaySplashFX();
							playerController.StartCoroutine(playerController
								.TriggerDeathThenWaitToDisable(true));
						}
						else
						{
							PlayDeathFX();
							playerController.StartCoroutine(playerController
								.TriggerDeathThenWaitToDisable(false));
						}
					}
					break;
				
			}
		}

		private void OnTriggerEnter(Collider other) 
		{
			switch(other.gameObject.tag)
			{
				case "PowerUp":
					Debug.Log("Player touched a PowerUp");
					if(powerUp==null)
					{
						powerUp = other.gameObject.GetComponent<IPowerUp>();
						powerUp.PowerUp(this);
						powerUpImage.sprite = powerUp.GetSprite(); 
						powerUpImage.color  = Color.white;
						Destroy(other.gameObject);
					}
					break; 
			}	
		}

		[SerializeField]private Image powerUpImage;

		private void PlayDeathFX()
		{
			VFXManager.Instance?.PlayEffect(VFXEnum.BloodEffect
				, transform.position
				, new Vector3());
		}

		private void PlaySplashFX()
		{
			VFXManager.Instance?.PlayEffect(VFXEnum.WaterEffect
				, transform.position + new Vector3(0,2,0)
				, new Vector3());
		}
	}
}