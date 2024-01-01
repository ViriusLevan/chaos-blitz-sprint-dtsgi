using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LevelUpStudio.ChaosBlitzSprint.Player
{
    public class VirtualCursor : MonoBehaviour
    {
        [SerializeField] private PlayerInputHandler playerInputHandler;
        public void SetPlayerInputHandler(PlayerInputHandler pi) =>playerInputHandler=pi;
        [SerializeField] private RectTransform cursorTransform;
        [SerializeField] private Canvas centerCanvas;
        [SerializeField] private RectTransform canvasRectTransform;
        [SerializeField] private float cursorSpeed = 1000;

        private bool previousMouseState;
        private Mouse virtualMouse;
        private Camera mainCamera;

        public RectTransform GetCursorTranform(){return cursorTransform;} 
        
        private void OnEnable() {
            GameObject mark = GameObject.FindGameObjectWithTag("UIHelper");
            Debug.Log(SceneManager.GetActiveScene().name);
            Debug.Log(mark.name);
            CursorInitializerHelper cursorInit 
                = mark.GetComponent<CursorInitializerHelper>();
            
            if (cursorTransform==null) 
                cursorTransform = cursorInit.GetCursorTransforms()
                    [playerInputHandler.playerConfig.playerIndex];

            if(centerCanvas == null)  
                centerCanvas= cursorInit.GetCanvas();
            if(canvasRectTransform == null) 
                canvasRectTransform = cursorInit.GetCanvasRectTransform();

            mainCamera = Camera.main;
            if(virtualMouse==null){
                virtualMouse = (Mouse) InputSystem.AddDevice("Virtualmouse");
            }else if(!virtualMouse.added){
                InputSystem.AddDevice(virtualMouse);
            }
            
            InputUser.PerformPairingWithDevice(virtualMouse, playerInputHandler.playerConfig.input.user);

            if(cursorTransform !=null){
                Vector2 position = cursorTransform.anchoredPosition;
                InputState.Change(virtualMouse.position,position);
                cursorTransform.GetComponent<Image>().sprite 
                    = GameManager.Instance.GetCursors()[
                        playerInputHandler.playerConfig.cursorIndex
                        ];
            }
        }


        void OnDisable()
        {
            if(virtualMouse != null & virtualMouse.added){
                InputSystem.RemoveDevice(virtualMouse);
            }
        }

        [SerializeField]private Vector2 deltaValue;
        private bool aButtonIsPressed;
        public void OnNavigate(InputAction.CallbackContext context) 
            => deltaValue = context.ReadValue<Vector2>();
        public void OnClick(InputAction.CallbackContext context) 
            => aButtonIsPressed = context.action.IsPressed();
        
        private Vector2 mousePosition;
        public void OnMousePositionChange(InputAction.CallbackContext context)
            =>  mousePosition = context.ReadValue<Vector2>();

        private void Update() {
            if(playerInputHandler.playerInstance.playerStatus!=PlayerInstance.PlayerStatus.Picking)
                return;


            if(playerInputHandler.playerConfig.input==null){
                Debug.LogError("ERROR : playerInput null reference");
                return;
            }
            UpdateMotion();
        }

        private readonly float referenceWidth = 1280;
        private readonly float referenceHeight = 720;
        [SerializeField]private float speedClamp =10f;

        public bool hasMouse = false;
        public void UpdateMotion(){
            if(virtualMouse == null || virtualMouse.enabled==false){
                return;
            }
            if(!hasMouse){
        //Scaling cursor speed according to current screen width and height
                float scaleX = Screen.width / referenceWidth;
                float scaleY = Screen.height / referenceHeight;
                //Debug.Log($"Scale X {scaleX} Y {scaleY} cspeed {cursorSpeed}");
                float scaledSpeed = scaleX * scaleY * cursorSpeed;
                //averaging value to reduce speed difference
                scaledSpeed = (scaledSpeed + cursorSpeed) / 2;
                //Debug.Log(scaledSpeed);

                deltaValue *= scaledSpeed * Time.deltaTime;
                deltaValue = new Vector2(Mathf.Clamp(deltaValue.x,-speedClamp,speedClamp)
                    ,Mathf.Clamp(deltaValue.y,-speedClamp,speedClamp));

                Vector2 currentPosition = virtualMouse.position.ReadValue();
                Vector2 newPosition = currentPosition + deltaValue;

                newPosition.x =  Mathf.Clamp(newPosition.x, 0, Screen.width);
                newPosition.y =  Mathf.Clamp(newPosition.y, 0, Screen.height);

                InputState.Change(virtualMouse.position, newPosition);
                InputState.Change(virtualMouse.delta, deltaValue);

                AnchorCursor(newPosition);
            }
            else
            {
                InputState.Change(virtualMouse.position, mousePosition);
                AnchorCursor(mousePosition);
            }
            if(previousMouseState != aButtonIsPressed){
                virtualMouse.CopyState<MouseState>(out var mouseState);
                mouseState.WithButton(MouseButton.Left, aButtonIsPressed);
                InputState.Change(virtualMouse, mouseState);
                previousMouseState = aButtonIsPressed;
            }
        }

        public void ResetMousePosition(){
            Vector2 newPosition = new Vector2(0,0);
            InputState.Change(virtualMouse.position, newPosition);
            AnchorCursor(newPosition);
        }

        private void AnchorCursor(Vector2 position){
            Vector2 anchoredPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle
                (canvasRectTransform, position, centerCanvas.renderMode 
                    == RenderMode.ScreenSpaceOverlay ? null : mainCamera, out anchoredPosition);

            cursorTransform.anchoredPosition = anchoredPosition;
        }

        public void SetCursorColor(Color newColor){
            cursorTransform.GetComponent<Image>().color = newColor;
        }

        public void SetCursorTransparency(float transparency){
            Color thisColor = cursorTransform.GetComponent<Image>().color;
            thisColor.a = transparency;
            cursorTransform.GetComponent<Image>().color = thisColor;
        }
    }
}
