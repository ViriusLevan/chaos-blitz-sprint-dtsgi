using UnityEngine;

namespace LevelUpStudio.ChaosBlitzSprint.PowerUp
{
    public class ExtraLifePowerUp : MonoBehaviour, IPowerUp
    {
        void IPowerUp.PowerUp(Player.PlayerInteractor powerUp)
        {
            powerUp.ActivateExtraLife();
        }
    }
}
