using UnityEngine;

namespace LevelUpStudio.ChaosBlitzSprint.PowerUp
{
    public class DoubleJumpPowerUp: MonoBehaviour, IPowerUp
    {
        void IPowerUp.PowerUp(Player.PlayerInteractor powerUp)
        {
            powerUp.ActivateDoubleJump();
            Destroy(gameObject);
        }
    }
}
