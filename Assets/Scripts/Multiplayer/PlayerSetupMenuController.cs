﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSetupMenuController : MonoBehaviour
{
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

    void Update()
    {
        if (Time.time > ignoreInputTime)
        {
            inputEnabled = true;
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
        readyPanel.SetActive(true);
        readyButton.interactable = true;
        menuPanel.SetActive(false);
        readyButton.Select();
        
    }

    public void ReadyPlayer()
    {
        if (!inputEnabled) { return; }

        PlayerConfigurationManager.Instance.ReadyPlayer(playerIndex);
        readyButton.gameObject.SetActive(false);
    }
}