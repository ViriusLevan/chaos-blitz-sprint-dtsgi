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
            playerConfigs[i].Input.camera = player.GetComponentInChildren<Camera>();
            playerConfigs[i].Input.uiInputModule = inputModule;
            Debug.Log(playerConfigs[i].Input.uiInputModule);
            player.GetComponentInChildren<PlayerInputHandler>().InitializePlayer(playerConfigs[i]);

            player.GetComponentInChildren<CinemachineInputHandler>().horizontal = playerConfigs[i].Input.actions.FindAction("Look");
            
            Debug.Log(player.GetComponentInChildren<CinemachineInputHandler>().horizontal);
        }
        PlayerConfigurationManager.Instance.EnableSplitScreen();
    }
}