using UnityEngine;

namespace LevelUpStudio.ChaosBlitzSprint.PowerUp
{
    public class ShieldPowerUp : MonoBehaviour, IPowerUp
    {
        void IPowerUp.PowerUp(Player.PlayerInteractor powerUp)
        {
            powerUp.ActivateShield();
        }
    }
}