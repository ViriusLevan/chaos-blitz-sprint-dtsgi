using UnityEngine;

namespace LevelUpStudio.ChaosBlitzSprint.PowerUp
{
    public class DoubleJumpPowerUp: MonoBehaviour, IPowerUp
    {
        [SerializeField]private Sprite sprite;
        public GameObject notifDoubleJump;
        public GameObject posNotif;
        public bool isDJActive = false;
        public Sprite GetSprite()
        {
            //GameObject instantiateRef;
            //instantiateRef = Instantiate(notifDoubleJump);
            //Canvas canvas = posNotif.GetComponent<Canvas>();
            //instantiateRef.transform.SetParent(canvas.transform, false);
            // instantiateRef.transform.parent = posNotif.transform;
            //instantiateRef.transform.localPosition = Vector3.zero;
            isDJActive = true;
            notifDoubleJump.SetActive(true);
            return sprite;
        }

        void IPowerUp.PowerUp(Player.PlayerInteractor powerUp)
        {
            isDJActive = false;
            notifDoubleJump.SetActive(false);
            powerUp.ActivateDoubleJump();
        }
    }
}
