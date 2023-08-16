using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.UI;

public class LevelInitializer : MonoBehaviour
{
    [SerializeField] private InputSystemUIInputModule inputModule;
    [SerializeField] private Transform[] PlayerSpawns;
    [SerializeField] private GameObject[] playerCanvas;
    [SerializeField] private GameObject playerPrefab;

    private void Start()
    {
        var playerConfigs = PlayerConfigurationManager.Instance.GetPlayerConfigs().ToArray();
        for (int i = 0; i < playerConfigs.Length; i++)
        {
            playerCanvas[i].SetActive(true);
            GameObject player = Instantiate(playerPrefab, PlayerSpawns[i].position, PlayerSpawns[i].rotation);
            playerConfigs[i].input.camera = player.GetComponentInChildren<Camera>();
            playerConfigs[i].input.uiInputModule = inputModule;
            Debug.Log(playerConfigs[i].input.uiInputModule);
            
            player.GetComponentInChildren<PlayerInputHandler>()
                .InitializePlayer(playerConfigs[i], player.GetComponent<PlayerInstance>());
        }
        PlayerConfigurationManager.Instance.EnableSplitScreen();
    }

    
}