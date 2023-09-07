using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using LevelUpStudio.ChaosBlitzSprint.PowerUp;

namespace LevelUpStudio.ChaosBlitzSprint.Player
{
	public class PlayerInteractor : MonoBehaviour
	{
		[SerializeField] private PlayerController playerController;
		private List<IPowerUp> powerUps;
		private int maximumAllowedPowerUps=1;

		private void Start() 
		{
			powerUps = new List<IPowerUp>();	
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
		}

		public void ActivateShield()
		{
			Debug.Log("Shield Activated");
			playerController.hasShield = true;
			// this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
		}

		public void DeactivateShield()
		{
			Debug.Log("Shield Deactivated");
			playerController.hasShield = false;
			// this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
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
						playerController.StartCoroutine(playerController.TriggerDeathThenWaitToDisable());
						//playerController.DisableMeshAndCollider();
						GameManager.Instance.PlayerDied(playerController
							.playerInputHandler.playerConfig.playerIndex);
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
							PlaySplashFX();
						else
							PlayDeathFX();
						playerController.StartCoroutine(playerController.TriggerDeathThenWaitToDisable());
						//playerController.DisableMeshAndCollider();
						GameManager.Instance.PlayerDied(playerController
							.playerInputHandler.playerConfig.playerIndex);
					}
					break;
				case "PowerUp":
					Debug.Log("Player touched a PowerUp");
					if(powerUps.Count<maximumAllowedPowerUps)
					{
						powerUps.Add(other.gameObject.GetComponent<IPowerUp>());
						Destroy(other.gameObject);
					}
					if(powerUps.Count==1)
					{
						powerUps[0].PowerUp(this);
						//powerUpImage.sprite = ; 
					}
					break; 
			}
		}
		[SerializeField]private Image powerUpImage;

		private void PlayDeathFX()
		{
			SoundManager.Instance?.PlaySound(SoundEnum.HitSound);
			VFXManager.Instance?.PlayEffect(VFXEnum.BloodEffect
				, transform.position
				, new Vector3());
		}

		private void PlaySplashFX()
		{
			SoundManager.Instance?.PlaySound(SoundEnum.WaterSound);
			VFXManager.Instance?.PlayEffect(VFXEnum.WaterEffect
				, transform.position + new Vector3(0,2,0)
				, new Vector3());
		}
	}
}