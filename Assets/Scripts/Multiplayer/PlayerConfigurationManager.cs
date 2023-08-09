using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerConfigurationManager : MonoBehaviour
{
    [SerializeField] private int minPlayers=1, maxPlayers = 4;
    [SerializeField] private PlayerInputManager pim;
    [SerializeField] private List<PlayerConfiguration> playerConfigs;
    [SerializeField] private List<GameObject> playerJoinTexts;
    private int nSpawnedPlayers = 0;

    public static PlayerConfigurationManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.Log("[Singleton] Trying to instantiate a seccond instance of a singleton class.");
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
            playerConfigs = new List<PlayerConfiguration>();
        }
        
    }

    public void HandlePlayerJoin(PlayerInput pi)
    {
        Debug.Log("player joined " + pi.playerIndex);
        pi.transform.SetParent(transform);

        if(!playerConfigs.Any(p => p.PlayerIndex == pi.playerIndex))
        {
            playerConfigs.Add(new PlayerConfiguration(pi));
        }

        playerJoinTexts[pi.playerIndex].SetActive(false);
    }
    
    public void SetNSpawnedPlayers(int newVal)
    {
        nSpawnedPlayers=newVal;
    }

    public int GetNSpawnedPlayers()
    {
        return nSpawnedPlayers;
    }

    public void EnableSplitScreen()
    {
        pim.splitScreen=true;
    }

    public List<PlayerConfiguration> GetPlayerConfigs()
    {
        return playerConfigs;
    }

    public void SetPlayerColor(int index, Material color)
    {
        playerConfigs[index].playerMaterial = color;
    }

    public void ReadyPlayer(int index)
    {
        playerConfigs[index].isReady = true;
        if (minPlayers <= playerConfigs.Count && playerConfigs.Count <= maxPlayers && playerConfigs.All(p => p.isReady == true))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}

public class PlayerConfiguration
{
    public PlayerConfiguration(PlayerInput pi)
    {
        PlayerIndex = pi.playerIndex;
        Input = pi;
    }

    public PlayerInput Input { get; private set; }
    public int PlayerIndex { get; private set; }
    public bool isReady { get; set; }
    public Material playerMaterial {get; set;}
}