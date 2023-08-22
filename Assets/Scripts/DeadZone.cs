using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    public Transform checkPoint;

    private void OnCollisionEnter(Collision other) 
    {
            Debug.Log("OnColEnter ="+other.gameObject.tag +" || "+ other.gameObject.name);

        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("AAaaa");
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            if (playerController.hasExtraLife == true)
            {
                playerController.transform.position = checkPoint.position;
                playerController.DectivateExtraLife();
            }
            else
            {
                playerController.DisableMeshRenderer();
                GameManager.Instance.PlayerDied(playerController.GetPlayerInputHandler().playerConfig.playerIndex);
            }
        }    
    }
}
