using UnityEngine;

namespace LevelUpStudio.ChaosBlitzSprint.PowerUp
{
    public class ExtraLifePowerUp : MonoBehaviour, IPowerUp
    {
        [SerializeField]private Sprite sprite;
        public GameObject notifExtraLife;
        public GameObject posNotif;
        public Sprite GetSprite()
        {
            GameObject instantiateRef;
            instantiateRef = Instantiate(notifExtraLife);
            Canvas canvas = posNotif.GetComponent<Canvas>();
            instantiateRef.transform.SetParent(canvas.transform, false);
            //instantiateRef.transform.parent = posNotif.transform;
            //instantiateRef.transform.localPosition = Vector3.zero;
            notifExtraLife.SetActive(true);
            return sprite;
        }
        void IPowerUp.PowerUp(Player.PlayerInteractor powerUp)
        {
            notifExtraLife.SetActive(false);
            powerUp.IncreaseExtraLife();
        }
    }
}
