using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtPlayer : MonoBehaviour
{
    public Transform checkPoint;

    private void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController.hasExtraLife == true)
            {
                playerController.transform.position = checkPoint.position;
                playerController.DectivateExtraLife();
            }
            else if (playerController.hasShield == true)
            {
                playerController.DeactivateShield();
            }
            else
            {
                playerController.DisableMeshRenderer();
            }
        }    
    }
}

