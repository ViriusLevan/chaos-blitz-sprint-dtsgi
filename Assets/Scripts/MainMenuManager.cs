using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{

    [SerializeField]private GameObject menuPanel,roundSelectPanel, lobbyPanel;

    public void EnterRoundSelection()
    {
        roundSelectPanel.SetActive(true);
        menuPanel.SetActive(false);
    }
    public void ExitRoundSelection()
    {
        roundSelectPanel.SetActive(false);
        menuPanel.SetActive(true);
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
        roundSelectPanel.SetActive(true);
        lobbyPanel.SetActive(false);
    }

    public void SetRoundType(RoundType rType)
    {
        PlayerConfigurationManager.Instance.roundType = rType;
        EnterLobby();
    }

    public void BeginGame()
    {
        PlayerConfigurationManager.Instance.BeginGame();
    }

    
}
