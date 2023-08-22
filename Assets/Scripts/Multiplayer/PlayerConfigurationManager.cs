using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerConfigurationManager : MonoBehaviour
{
    [SerializeField] private int minPlayers=1, maxPlayers = 4;
    [SerializeField] private PlayerInputManager pim;
    [SerializeField] private List<PlayerConfiguration> playerConfigs;
    [SerializeField] private List<GameObject> playerJoinTexts;
    private int nSpawnedPlayers = 0;

    public static PlayerConfigurationManager Instance { get; private set; }

    public void Reset(SceneLoader.SceneIndex sceneIndex)
    {
        if(sceneIndex!= SceneLoader.SceneIndex.MainMenu)return;
        playerConfigs = new List<PlayerConfiguration>();
        nSpawnedPlayers = 0;
    }

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

    private void Start() 
    {
        SceneLoader.SceneLoad+=Reset;
    }
    void OnDestroy()
    {
        SceneLoader.SceneLoad-=Reset;
    }

    public void HandlePlayerJoin(PlayerInput pi)
    {
        Debug.Log("player joined " + pi.playerIndex);
        pi.transform.SetParent(transform);

        if(!playerConfigs.Any(p => p.playerIndex == pi.playerIndex))
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

    public void DisableSplitScreen()
    {
        pim.splitScreen=false;
    }

    public List<PlayerConfiguration> GetPlayerConfigs()
    {
        return playerConfigs;
    }

    public void SetPlayerColor(int index, Material color, int cIndex)
    {
        playerConfigs[index].playerMaterial = color;
        playerConfigs[index].cursorIndex = cIndex;
    }

    public void ReadyPlayer(int index)
    {
        playerConfigs[index].isReady = true;
        if (minPlayers <= playerConfigs.Count 
            && playerConfigs.Count <= maxPlayers 
            && playerConfigs.All(p => p.isReady == true))
        {
            SceneLoader.Instance.LoadScene(SceneLoader.SceneIndex.Gameplay);
        }
    }
}

public class PlayerConfiguration
{
    public PlayerConfiguration(PlayerInput pi)
    {
        playerIndex = pi.playerIndex;
        input = pi;
        scoreTotal=0;
    }

    public PlayerInput input { get; private set; }
    public int playerIndex { get; private set; }
    public bool isReady { get; set; }
    public Material playerMaterial {get; set;}
    public int cursorIndex;
    public int scoreTotal;
}