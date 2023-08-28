﻿using System.Collections;
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
    //[SerializeField] private List<GameObject> playerJoinTexts;
    private int nSpawnedPlayers = 0;

    //TODO maybe make another static class for this?
    public RoundType roundType;

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
            Destroy(this.gameObject);
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
        SceneLoader.sceneLoaded+=Reset;
    }
    void OnDestroy()
    {
        SceneLoader.sceneLoaded-=Reset;
    }

    public void HandlePlayerJoin(PlayerInput pi)
    {
        Debug.Log("player joined " + pi.playerIndex);
        pi.transform.SetParent(transform);

        if(!playerConfigs.Any(p => p.playerIndex == pi.playerIndex))
        {
            playerConfigs.Add(new PlayerConfiguration(pi));
        }

        //playerJoinTexts[pi.playerIndex].SetActive(false);
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

    public void EnableJoining()
    {
        pim.EnableJoining();
    }

    public void DisableJoining()
    {
        pim.DisableJoining();
    }

    public void ClearPlayers()
    {
        playerConfigs.Clear();

//Deletes the children of this transform, which are the PlayerConfiguration prefabs
        int i = 0;
        //Array to hold all child obj
        GameObject[] allChildren = new GameObject[transform.childCount];

        //Find all child obj and store to that array
        foreach (Transform child in transform)
        {
            allChildren[i] = child.gameObject;
            i += 1;
        }

        //Now destroy them
        foreach (GameObject child in allChildren)
        {
            Destroy(child.gameObject);
        }
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
        BeginGame();
    }

    public void BeginGame()
    {
        if(roundType==null)
        {
            Debug.Log("Round type not selected");
            return;
        }
        if (minPlayers <= playerConfigs.Count 
            && playerConfigs.Count <= maxPlayers 
            && playerConfigs.All(p => p.isReady == true))
        {
            SceneLoader.Instance.LoadScene(SceneLoader.SceneIndex.Gameplay);
        }
        else
        {
            Debug.Log("Not all players are ready");
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
    //TODO rename to materialIndex after splitting 
    //the corresponding property in GameManager to another class
    //AND maybe use enum instead of int
    public int cursorIndex;
    public int scoreTotal;
}