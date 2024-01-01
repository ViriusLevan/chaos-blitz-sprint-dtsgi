
using UnityEngine;

namespace LevelUpStudio.ChaosBlitzSprint.PowerUp
{
    public interface IPowerUp
    {
        public void PowerUp(Player.PlayerInteractor powerUp);
        public Sprite GetSprite();
    }
}