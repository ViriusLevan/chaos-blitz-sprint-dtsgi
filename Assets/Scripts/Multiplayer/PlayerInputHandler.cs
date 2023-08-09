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
    private PlacementManager pManager;
    private PlayerInputActionsAsset controls;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        pManager = GetComponent<PlacementManager>();
        controls = new PlayerInputActionsAsset();
    }

    public void InitializePlayer(PlayerConfiguration config)
    {
        playerConfig = config;
        playerMesh.material = config.playerMaterial;
        config.Input.onActionTriggered += Input_onActionTriggered;

        GetComponent<PlacementManager>()?.SetPlayerInput(config.Input);
    }   

    private void Input_onActionTriggered(CallbackContext obj)
    {

        //Is there a better way of assigning these?
        
        if (obj.action.name == controls.Player.Move.name)
        {
            controller?.OnMove(obj);
        }
        if (obj.action.name == controls.Player.Jump.name)
        {
            controller?.OnJump(obj);
        }
        if(obj.performed){
            if (obj.action.name == controls.Player.BuildToggle.name
                || obj.action.name == controls.BuildMode.BuildToggle.name){
                controller?.OnBuildToggle(playerConfig);
            }
            if(obj.action.name == controls.BuildMode.Place.name)
            {
                pManager?.PlaceObject(obj);
            }
            if(obj.action.name == controls.BuildMode.CycleSelectionForward.name)
            {
                pManager?.CycleIndexForward(obj);
            }
            if(obj.action.name == controls.BuildMode.CycleSelectionBackward.name)
            {
                pManager?.CycleIndexBackward(obj);
            }
        }
    }


}