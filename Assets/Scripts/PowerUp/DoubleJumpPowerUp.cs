using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpPowerUp: MonoBehaviour, IPowerUp
{
    void IPowerUp.PowerUp(PlayerInteractor powerUp)
    {
        powerUp.ActivateDoubleJump();
        Destroy(gameObject);
    }
}
