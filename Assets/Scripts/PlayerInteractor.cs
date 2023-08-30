using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

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
					playerController.DisableMeshAndCollider();
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
					PlayDeathFX();
					playerController.DisableMeshAndCollider();
					GameManager.Instance.PlayerDied(playerController
						.playerInputHandler.playerConfig.playerIndex);
				}
				break;
			case "PowerUp":
				Debug.Log("Player touched a PowerUp");
           		other.gameObject.GetComponent<IPowerUp>()?.PowerUp(this);
				break; 
		}
	}

	private void PlayDeathFX()
	{
		SoundManager.Instance?.PlaySound(SoundEnum.HitSound);
		VFXManager.Instance?.PlayEffect(VFXEnum.BloodEffect
			, transform.position
			, new Vector3());
	}
}
