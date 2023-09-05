using UnityEngine;
using UnityEngine.InputSystem.UI;
using TMPro;
using LevelUpStudio.ChaosBlitzSprint.Player;

namespace LevelUpStudio.ChaosBlitzSprint
{
    public class LevelInitializer : MonoBehaviour
    {
        [SerializeField] private InputSystemUIInputModule inputModule;
        [SerializeField] private GameObject[] playerCanvas;
        [SerializeField] private GameObject playerPrefab;
        
        //TODO maybe move this to another class?
        [SerializeField] private TextMeshProUGUI roundPointDisplay;

        private void Start()
        {
            var playerConfigs = PlayerConfigurationManager.Instance.GetPlayerConfigs().ToArray();
            for (int i = 0; i < playerConfigs.Length; i++)
            {
                //playerCanvas[i].SetActive(true);
                GameObject player = Instantiate(playerPrefab
                    , GameManager.Instance.GetPlayerSpawnPoints()[i].position
                    , GameManager.Instance.GetPlayerSpawnPoints()[i].rotation);
                playerConfigs[i].input.camera = player.GetComponentInChildren<Camera>();
                playerConfigs[i].input.uiInputModule = inputModule;
                Debug.Log(playerConfigs[i].input.uiInputModule);
                
                PlayerInstance instance = player.GetComponent<PlayerInstance>();
                Debug.Log("Initializer instance"+(instance == null));
                Debug.Log("Initializer ins pih"+(instance.playerInputHandler == null));
                instance.playerInputHandler.InitializePlayer(playerConfigs[i], instance);

                GameManager.Instance.AddPlayerInstance(instance);
            }
            int pointReq = GameManager.Instance.GetRoundType().GetPointRequirement();
            roundPointDisplay.text = $"First to {pointReq} points";
            PlayerConfigurationManager.Instance.EnableSplitScreen();
        }
    }
}