using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPowerUp : MonoBehaviour, IPowerUp
{
    void IPowerUp.PowerUp(PlayerInteractor powerUp)
    {
        powerUp.ActivateShield();
        Destroy(gameObject);
    }
}
