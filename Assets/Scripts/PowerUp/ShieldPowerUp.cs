using UnityEngine;

namespace LevelUpStudio.ChaosBlitzSprint.PowerUp
{
    public class ShieldPowerUp : MonoBehaviour, IPowerUp
    {
        [SerializeField]private Sprite sprite;
        public GameObject notifShield;
        public GameObject posNotif;
        public Sprite GetSprite()
        {
            //GameObject instantiateRef;
            //instantiateRef = Instantiate(notifShield);
            //Canvas canvas = posNotif.GetComponent<Canvas>();
            //instantiateRef.transform.SetParent(canvas.transform, false);
            //instantiateRef.transform.parent = posNotif.transform;
            //instantiateRef.transform.localPosition = Vector3.zero;
            notifShield.SetActive(true);
            return sprite;
        }
        void IPowerUp.PowerUp(Player.PlayerInteractor powerUp)
        {
            notifShield.SetActive(false);
            powerUp.ActivateShield();
        }
    }
}