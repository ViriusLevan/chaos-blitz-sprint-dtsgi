using UnityEngine;
using UnityEngine.UI;
using LevelUpStudio.ChaosBlitzSprint.PowerUp;
using TMPro;

namespace LevelUpStudio.ChaosBlitzSprint.Player
{
	public class PlayerInteractor : MonoBehaviour
	{
		[SerializeField] private PlayerController playerController;
		[SerializeField]private float extraLife=0;
		public bool doubleJumpAvailable=false, hasShield=false;


		[SerializeField]private GameObject extraLifeIcon;
		[SerializeField]private TextMeshProUGUI extraLifeText;
		public void IncreaseExtraLife(){
			Debug.Log("Extra Life Increased");
			extraLife+=1;
			extraLifeIcon.SetActive(true);
			extraLifeText.text = $"x{extraLife}";
		}
		public void DecreaseExtraLife(){
			Debug.Log("Extra Life Decreased");
			extraLife-=1;
			if(extraLife<1)
				extraLifeIcon.SetActive(false);
		}

		public void ActivatePowerUp()
		{
			if(currentPowerup!=null )
			{
				currentPowerup.PowerUp(this);
				currentPowerup=null;
			}
		}
		
		[SerializeField] private GameObject doubleJumpIndicator;
		public void ActivateDoubleJump()
		{
			DeactivateSprite();
			doubleJumpAvailable = true;
			doubleJumpIndicator.SetActive(true);
		}

		[SerializeField] private GameObject shield;
		public void ActivateShield()
		{
			DeactivateSprite();
			hasShield = true;
			shield.SetActive(true);
			// this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
		}

		public void DeactivateSprite()
		{
			currentPowerup=null;
			powerUpImage.sprite = null;
			powerUpImage.color  = new Color(255,255,255,0);
		}

		public enum PUDeactivation {DoubleJump,Shield,ALL}
		public void DeactivatePowerUp(PUDeactivation pu)
		{
			//TODO yeah..
			switch(pu)
			{
				case PUDeactivation.DoubleJump:
					doubleJumpAvailable = false;
					doubleJumpIndicator.SetActive(false);
				break;
				case PUDeactivation.Shield:
					hasShield = false;
					shield.SetActive(false);
				break;
				case PUDeactivation.ALL:
					doubleJumpAvailable = false;
					hasShield = false;
					shield.SetActive(false);
				break;
			}
		}

		private void OnCollisionEnter(Collision other) {
			switch(other.gameObject.tag){
				case "Goal":
					GameManager.Instance.PlayerFinished(playerController
						.playerInputHandler.playerConfig.playerIndex);
					playerController.DisableMovementAndColliders();
					break;
				case "HurtPlayer":
					Debug.Log("Player Hurt");
					if (extraLife>0)
					{
						DecreaseExtraLife();
						playerController.SendBackToSpawn();
					}
					else if (hasShield)
					{
						DeactivatePowerUp(PUDeactivation.Shield);
					}
					else
					{
						PlayDeathFX();
						playerController.TriggerDeathThenWaitToDisable(false);
						//playerController.DisableMeshAndCollider();
					}
					break;
				case "DeadZone":
					Debug.Log("Player touched a DeadZone");
					if (extraLife>0)
					{
						DecreaseExtraLife();
						playerController.SendBackToSpawn();
					}
					else
					{
						if(other.gameObject.name.Contains("Water"))
						{
							PlaySplashFX();
							playerController.TriggerDeathThenWaitToDisable(true);
						}
						else
						{
							PlayDeathFX();
							playerController.TriggerDeathThenWaitToDisable(false);
						}
					}
					break;
				
			}
		}
		[SerializeField]private IPowerUp currentPowerup;
		private void OnTriggerEnter(Collider other) 
		{
			switch(other.gameObject.tag)
			{
				case "PowerUp":
					Debug.Log("Player touched a PowerUp");
					bool isExtraLife = other.gameObject.GetComponent<ExtraLifePowerUp>();
					if(currentPowerup==null || isExtraLife)
					{
						IPowerUp tempPowerUp = other.gameObject.GetComponent<IPowerUp>();
						if(!isExtraLife)
						{
							powerUpImage.sprite = tempPowerUp.GetSprite(); 
							powerUpImage.color  = Color.white;
							currentPowerup = tempPowerUp;
						}
						else
						{
							tempPowerUp.PowerUp(this);
						}

						Destroy(other.gameObject);
					}
					break; 
			}	
		}

		[SerializeField]private Image powerUpImage;

		private void PlayDeathFX()
		{
			VFXManager.Instance?.PlayEffect(VFXEnum.Blood
				, transform.position
				, new Vector3());
		}

		private void PlaySplashFX()
		{
			VFXManager.Instance?.PlayEffect(VFXEnum.Water
				, transform.position + new Vector3(0,2,0)
				, new Vector3());
		}
	}
}