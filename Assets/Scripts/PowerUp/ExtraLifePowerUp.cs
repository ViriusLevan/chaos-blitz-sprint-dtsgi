using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraLifePowerUp : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.ActivateExtraLife();
                Destroy(gameObject); // Remove the power-up prefab after collecting
            }
        }    
    }
}
