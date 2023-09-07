using UnityEngine;

namespace LevelUpStudio.ChaosBlitzSprint.PowerUp
{
    public class ExtraLifePowerUp : MonoBehaviour, IPowerUp
    {
        [SerializeField]private Sprite sprite;
        public Sprite GetSprite()
        {
            return sprite;
        }
        void IPowerUp.PowerUp(Player.PlayerInteractor powerUp)
        {
            powerUp.ActivateExtraLife();
        }
    }
}
