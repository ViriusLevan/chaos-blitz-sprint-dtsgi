using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{

    [SerializeField]private GameObject menuPanel,roundSelectPanel, lobbyPanel;
    [SerializeField]private Button menuFirst, roundSelectFirst, lobbyFirst;

    public void EnterRoundSelection()
    {
        menuPanel.SetActive(false);
        roundSelectPanel.SetActive(true);
        roundSelectFirst.Select();
        // EventSystem.current.SetSelectedGameObject
        //     (null, null);  
        // EventSystem.current.SetSelectedGameObject
        //     (roundSelectFirst.gameObject, null);        
    }
    public void ExitRoundSelection()
    {
        roundSelectPanel.SetActive(false);
        menuPanel.SetActive(true);
        menuFirst.Select();
        EventSystem.current.SetSelectedGameObject
            (null, null);  
        EventSystem.current.SetSelectedGameObject
            (menuFirst.gameObject, null);       
        //menuFirst.OnSelect(null);
        // StartCoroutine(SelectContinueButtonLater(menuFirst));
    }

    public void EnterLobby()
    {
        PlayerConfigurationManager.Instance.EnableJoining();
        roundSelectPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        lobbyFirst.Select();
        EventSystem.current.SetSelectedGameObject
            (null, null);  
        EventSystem.current.SetSelectedGameObject
            (lobbyFirst.gameObject, null);     
    }

    public void ExitLobby()
    {
        PlayerConfigurationManager.Instance.DisableJoining();
        PlayerConfigurationManager.Instance.ClearPlayers();
        lobbyPanel.SetActive(false);
        roundSelectPanel.SetActive(true);
        roundSelectFirst.Select();
        EventSystem.current.SetSelectedGameObject
            (null, null);  
        EventSystem.current.SetSelectedGameObject
            (roundSelectFirst.gameObject, null);     
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

    // IEnumerator SelectContinueButtonLater(Button butt)
    // {
    //     yield return new WaitForSeconds(0.1f);
    //     EventSystem.current.SetSelectedGameObject(null);
    //     EventSystem.current.SetSelectedGameObject(butt.gameObject);
    // }
    
}
