using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.UI;

public class LevelInitializer : MonoBehaviour
{
    [SerializeField] private InputSystemUIInputModule inputModule;
    [SerializeField] private GameObject[] playerCanvas;
    [SerializeField] private GameObject playerPrefab;

    private void Start()
    {
        var playerConfigs = PlayerConfigurationManager.Instance.GetPlayerConfigs().ToArray();
        for (int i = 0; i < playerConfigs.Length; i++)
        {
            playerCanvas[i].SetActive(true);
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
        PlayerConfigurationManager.Instance.EnableSplitScreen();
    }

    
}