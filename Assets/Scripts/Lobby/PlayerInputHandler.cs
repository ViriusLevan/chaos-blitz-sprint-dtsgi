using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerConfiguration playerConfig;
    private PlayerController controller;

    [SerializeField]
    private MeshRenderer playerMesh;

    private TESTMulti controls;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        controls = new TESTMulti();
    }

    public void InitializePlayer(PlayerConfiguration config)
    {
        playerConfig = config;
        playerMesh.material = config.playerMaterial;
        config.Input.onActionTriggered += Input_onActionTriggered;
    }   

    private void Input_onActionTriggered(CallbackContext obj)
    {
        if (obj.action.name == controls.Character.Movement.name)
        {
            controller?.OnMove(obj);
        }
    }


}