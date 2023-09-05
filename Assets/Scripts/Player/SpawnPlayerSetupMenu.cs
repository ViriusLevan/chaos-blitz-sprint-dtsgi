using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace LevelUpStudio.ChaosBlitzSprint.Player
{
    public class SpawnPlayerSetupMenu : MonoBehaviour
    {
        private GameObject rootMenu;
        public GameObject playerSetupMenuPrefab;
        public PlayerInput input;

        private void Awake()
        {
            rootMenu = GameObject.Find("PlayerReadyMenu");
            if(rootMenu != null)
            {
                GameObject menu = Instantiate(playerSetupMenuPrefab, rootMenu.transform);
                input.uiInputModule = menu.GetComponentInChildren<InputSystemUIInputModule>();
                menu.GetComponent<PlayerSetupMenuController>().SetPlayerIndex(input.playerIndex);
                menu.GetComponent<PlayerSetupMenuController>().pi = input;
            }
            
        }
    }
}