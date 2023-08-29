using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraLifePowerUp : MonoBehaviour, IPowerUp
{
    void IPowerUp.PowerUp(PlayerInteractor powerUp)
    {
        powerUp.ActivateExtraLife();
        Destroy(gameObject);
    }
}
