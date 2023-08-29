using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private MeshRenderer playerMesh;
    public PlayerConfiguration playerConfig{get;private set;}
    private PlayerController controller;
    private PlacementManager pManager;
    public PlayerInputActionsAsset controls{get;private set;}
    public VirtualCursor virtualCursor{get; private set;}
    [SerializeField]public PlayerInstance playerInstance{get;private set;}

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        pManager = GetComponent<PlacementManager>();
        controls = new PlayerInputActionsAsset();
        virtualCursor = GetComponent<VirtualCursor>();
    }
    [SerializeField]
    private InputSystemUIInputModule uiInputModule;
    public void InitializePlayer(PlayerConfiguration config, PlayerInstance instance)
    {
        playerInstance = instance;
        Debug.Log("InputHandlerInstance"+(playerInstance==null));
        playerConfig = config;
        playerMesh.material = config.playerMaterial;
        config.input.onActionTriggered += Input_onActionTriggered;
        
        virtualCursor?.SetPlayerInputHandler(this);
        
        virtualCursor.enabled=true;
        uiInputModule 
            = virtualCursor.GetCursorTranform().gameObject.GetComponent<InputSystemUIInputModule>();

        uiInputModule.gameObject.SetActive(true);
        
        uiInputModule.actionsAsset = config.input.actions;
        //uiInputModule.gameObject.name = config.Input.playerIndex.ToString();
        uiInputModule.gameObject.GetComponent<PlayerConfigurationReferenceHelper>()?
            .SetPlayerConfigurationReference(config);

		playerInstance.cinemachineInputHanlder.horizontal = config.input.actions.FindAction("Look");
		Debug.Log(playerInstance.cinemachineInputHanlder.horizontal);
    }   

    private void Input_onActionTriggered(CallbackContext obj)
    {

        if (obj.action.name == controls.Player.Jump.name)
        {
            controller?.OnJump(obj);
        }

        if(obj.performed){
            if(obj.action.name == controls.BuildMode.Place.name)
            {
                pManager?.PlaceObject(obj);
            }
        }

        if (obj.action.name == controls.UI.Navigate.name)
        {
            virtualCursor?.OnNavigate(obj);
        }
        if (obj.action.name == controls.UI.Click.name )
        {
            virtualCursor?.OnClick(obj);
        }
    }

    public string GetActionControlName(InputAction inputAction)
    {
		var action = playerConfig.input.actions.FindAction(inputAction.name);
        //Debug.Log(inputAction.name+" => "+playerConfig.input.currentControlScheme);    
		int bindingIndex = inputAction.GetBindingIndex(group: playerConfig.input.currentControlScheme);
        //var bindingIndex = inputAction.bindings.IndexOf(x => x.name.ToString() == playerConfig.input.currentControlScheme);  

        // for (int i = 0; i < inputAction.bindings.Count; i++)
        // {
        //     Debug.Log(i+"||"+inputAction.bindings[i].groups+"-"+inputAction.bindings[i].action+
        //         "-"+inputAction.bindings[i].name);
        // }
        //Debug.Log(bindingIndex+" ic "+inputAction.bindings[bindingIndex].isPartOfComposite.ToString());         
		var displayString = inputAction.GetBindingDisplayString(bindingIndex, out string deviceLayoutName, out string controlPath);
        if(inputAction.bindings[bindingIndex].isPartOfComposite)
        {
            bindingIndex+=1;
            while(inputAction.bindings.Count>bindingIndex)
            {
                //Debug.Log(bindingIndex+"-"+inputAction.bindings[bindingIndex].ToString());
                //If binding is NOT part of our group
                if(!inputAction.bindings[bindingIndex].ToString()
                    .Contains("<"))
                    break;
                displayString += " "+inputAction.GetBindingDisplayString(bindingIndex);
                bindingIndex+=1;
            }
        }
        return displayString;
    }
}