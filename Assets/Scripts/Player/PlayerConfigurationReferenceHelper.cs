using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerConfigurationReferenceHelper : MonoBehaviour
{
    [SerializeField]private PlayerConfiguration playerConfigurationReference;

    public void SetPlayerConfigurationReference(PlayerConfiguration pc){
        playerConfigurationReference = pc;
    }
    public PlayerConfiguration GetPlayerConfigurationReference(){return playerConfigurationReference;}
}

