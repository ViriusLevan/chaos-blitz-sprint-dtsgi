using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{

    [SerializeField]private GameObject menuPanel,roundSelectPanel, lobbyPanel, playerReadyMenu;
    [SerializeField]private Button menuFirst, roundSelectFirst, lobbyFirst;

    public static MainMenuManager Instance { get; private set; }
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
        }
    }

    public void EnterRoundSelection()
    {
        menuPanel.SetActive(false);
        roundSelectPanel.SetActive(true);
        roundSelectFirst.Select();    
    }
    public void ExitRoundSelection()
    {
        roundSelectPanel.SetActive(false);
        menuPanel.SetActive(true);
        menuFirst.Select();
    }

    public void EnterLobby()
    {
        PlayerConfigurationManager.Instance.EnableJoining();
        roundSelectPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    public void ExitLobby()
    {
        PlayerConfigurationManager.Instance.DisableJoining();
        PlayerConfigurationManager.Instance.ClearPlayers();
        ClearReadyPlayerMenu();
        lobbyPanel.SetActive(false);
        roundSelectPanel.SetActive(true);
        roundSelectFirst.Select();
    }

    public void ClearReadyPlayerMenu()
    {
        //Deletes the children of this transform, which are the PlayerConfiguration prefabs
        int i = 0;
        //Array to hold all child obj
        GameObject[] allChildren = new GameObject[playerReadyMenu.transform.childCount];

        //Find all child obj and store to that array
        foreach (Transform child in playerReadyMenu.transform)
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

    public void SetRoundType(RoundType rType)
    {
        PlayerConfigurationManager.Instance.roundType = rType;
        EnterLobby();
    }
}
