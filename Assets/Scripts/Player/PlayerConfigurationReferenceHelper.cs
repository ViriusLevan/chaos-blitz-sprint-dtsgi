using UnityEngine;

namespace LevelUpStudio.ChaosBlitzSprint.Player
{
    public class PlayerConfigurationReferenceHelper : MonoBehaviour
    {
        [SerializeField]private PlayerConfiguration playerConfigurationReference;

        public void SetPlayerConfigurationReference(PlayerConfiguration pc){
            playerConfigurationReference = pc;
        }
        public PlayerConfiguration GetPlayerConfigurationReference(){return playerConfigurationReference;}
    }
}
