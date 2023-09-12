using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;
using UnityEngine.InputSystem.UI;
using static UnityEngine.InputSystem.InputAction;

namespace LevelUpStudio.ChaosBlitzSprint.Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [SerializeField] private SkinnedMeshRenderer playerMesh;
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
            //pManager.SetArrowMaterial(config.playerMaterial);
            config.input.onActionTriggered += Input_onActionTriggered;
            if(config.input.currentControlScheme.Contains("mouse",System.StringComparison.OrdinalIgnoreCase))
            {
                virtualCursor.hasMouse=true;
            }
            
            virtualCursor?.SetPlayerInputHandler(this);
            
            virtualCursor.enabled=true;
            uiInputModule 
                = virtualCursor.GetCursorTranform().gameObject.GetComponent<InputSystemUIInputModule>();

            uiInputModule.gameObject.SetActive(true);
            
            uiInputModule.actionsAsset = config.input.actions;
            //uiInputModule.gameObject.name = config.Input.playerIndex.ToString();
            uiInputModule.gameObject.GetComponent<PlayerConfigurationReferenceHelper>()?
                .SetPlayerConfigurationReference(config);

            playerInstance.cinemachineInputHandler.horizontal = config.input.actions.FindAction("Look");
            //playerInstance.cinemachineInputHandler.vertical = config.input.actions.FindAction("Look");
            //Debug.Log(playerInstance.cinemachineInputHandler.horizontal);
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
                if (obj.action.name == controls.Player.Crouch.name)
                {
                    controller?.OnCrouch(obj);
                }
                if(obj.action.name == controls.Player.InvertX.name 
                    ||obj.action.name == controls.BuildMode.InvertX.name)
                {
                    playerInstance.cinemachineInputHandler.InvertX();
                }
                if(obj.action.name == controls.Player.InvertY.name 
                    ||obj.action.name == controls.BuildMode.InvertY.name)
                {
                    playerInstance.cinemachineInputHandler.InvertY();
                }
                if(obj.action.name == controls.Player.IncreaseCamSens.name 
                    ||obj.action.name == controls.BuildMode.IncreaseCamSens.name)
                {
                    playerInstance.cinemachineInputHandler.IncreaseSensitivity();
                }
                if(obj.action.name == controls.Player.DecreaseCamSens.name 
                    ||obj.action.name == controls.BuildMode.DecreaseCamSens.name)
                {
                    playerInstance.cinemachineInputHandler.DecreaseSensitivity();
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
            if(virtualCursor.hasMouse){
                if (obj.action.name == controls.UI.VirtualMouseNavigate.name )
                {
                    virtualCursor?.OnMousePositionChange(obj);
                }
            }

            // controls.Player.Look.ApplyParameterOverride
            //     ((InvertVector2Processor p) => p.invertX, false);
            // int bindingIndex = lookIA.GetBindingIndex(group: playerConfig.input.currentControlScheme);
            // controls.Player.Look.ApplyBindingOverride(new InputBinding
            // {
            //     groups = playerConfig.input.currentControlScheme,
            //     overrideProcessors = "invert(x=false,y=false)"
            // });
        }
        [SerializeField]private InputAction lookIA;

        public string GetActionControlName(InputAction inputAction)
        {
            var action = playerConfig.input.actions.FindAction(inputAction.name);
            //Debug.Log(inputAction.name+" => "+playerConfig.input.currentControlScheme);    
            int bindingIndex = inputAction.GetBindingIndex(group: playerConfig.input.currentControlScheme);
            //var bindingIndex = inputAction.bindings
            //  .IndexOf(x => x.name.ToString() == playerConfig.input.currentControlScheme);  

            // for (int i = 0; i < inputAction.bindings.Count; i++)
            // {
            //     Debug.Log(i+"||"+inputAction.bindings[i].groups+"-"+inputAction.bindings[i].action+
            //         "-"+inputAction.bindings[i].name);
            // }
            //Debug.Log(bindingIndex+" ic "+inputAction.bindings[bindingIndex].isPartOfComposite.ToString());         
            var displayString = inputAction.GetBindingDisplayString
                (bindingIndex, out string deviceLayoutName, out string controlPath);
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
}