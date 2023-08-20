using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private MeshRenderer playerMesh;
    private PlayerConfiguration playerConfig;
    private PlayerController controller;
    private PlayerInputActionsAsset controls;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        controls = new PlayerInputActionsAsset();
    }

    public void InitializePlayer(PlayerConfiguration config)
    {
        playerConfig = config;
        playerMesh.material = config.playerMaterial;
        config.Input.onActionTriggered += Input_onActionTriggered;
    }   

    private void Input_onActionTriggered(CallbackContext obj)
    {
        if (obj.action.name == controls.Player.Move.name)
        {
            controller?.OnMove(obj);
        }
        if (obj.action.name == controls.Player.Jump.name)
        {
            controller?.OnJump(obj);
        }
    }


}