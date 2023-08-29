﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using static UnityEngine.InputSystem.InputAction;

public class PlayerSetupMenuController : MonoBehaviour
{
    public InputSystemUIInputModule module;
    public PlayerInput pi;
    public PlayerInputActionsAsset controls{get;private set;}

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private GameObject readyPanel;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Button readyButton;

    private int playerIndex;
    private float ignoreInputTime = 0.5f;
    private bool inputEnabled;
    
    public void SetPlayerIndex(int pi)
    {
        playerIndex = pi;
        titleText.SetText("Player " + (pi + 1).ToString());
        ignoreInputTime = Time.time + ignoreInputTime;
    }

    void Awake()
    {
        controls = new PlayerInputActionsAsset();
    }
    private void Start() 
    {
        pi.onActionTriggered += Input_onActionTriggered;
    }
    void OnDisable()
    {
        pi.onActionTriggered -= Input_onActionTriggered;
    }
    void OnDestroy()
    {
        pi.onActionTriggered -= Input_onActionTriggered;
    }

    private void Input_onActionTriggered(CallbackContext obj)
    {
        if (obj.action.name == controls.UI.Cancel.name && obj.performed)
        {
            CancelButton();
        }
    }

//May be useful in the future for any singleplayer thing
    // private void OnEnable () => module.cancel.action.performed += Trigger;
    // private void OnDisable () => module.cancel.action.performed -= Trigger;
    // private void Trigger (InputAction.CallbackContext context) 
    // {
    //     Debug.Log("Cancel input event");    
    //     CancelButton();
    // }

    void Update()
    {
        if (Time.time > ignoreInputTime)
        {
            inputEnabled = true;
        }
    }

    public void CancelButton()
    {
        if (!inputEnabled) { return; }
        if (PlayerConfigurationManager.Instance.IsPlayerReady(playerIndex))
        {
            CancelReady();
        }
        else{
            MainMenuManager.Instance.ExitLobby();
        }
    }

    public void SelectColor(Material mat)
    {
        if (!inputEnabled) { return; }

        //TODO maybe use an SO to store material along with cursor index
        int cursorIndex=0;
        if(mat.name.Contains("Red", System.StringComparison.OrdinalIgnoreCase)){
            cursorIndex=0;
        } else if(mat.name.Contains("Purple", System.StringComparison.OrdinalIgnoreCase)){
            cursorIndex=1;
        } else if(mat.name.Contains("Yellow", System.StringComparison.OrdinalIgnoreCase)){
            cursorIndex=2;
        }else if(mat.name.Contains("Green", System.StringComparison.OrdinalIgnoreCase)){
            cursorIndex=3;
        }

        PlayerConfigurationManager.Instance.SetPlayerColor(playerIndex, mat, cursorIndex);
        ReadyPlayer();
        menuPanel.SetActive(false);
        readyPanel.SetActive(true);
        // readyButton.interactable = true;
        // readyButton.Select();
        
    }

    public void ReadyPlayer()
    {
        if (!inputEnabled) { return; }

        PlayerConfigurationManager.Instance.ReadyPlayer(playerIndex);
        //readyButton.gameObject.SetActive(false);
    }

    public void CancelReady()
    {
        PlayerConfigurationManager.Instance.CancelReady(playerIndex);
        menuPanel.SetActive(true);
    }

}