using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpPowerUp: MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.ActivateDoubleJump();
                Destroy(gameObject); // Remove the power-up prefab after collecting
            }
        }
    }
}
