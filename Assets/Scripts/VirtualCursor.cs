using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;

public class VirtualCursor : MonoBehaviour
{
    [SerializeField] private PlayerInputHandler playerInputHandler;
    public void SetPlayerInputHandler(PlayerInputHandler pi) =>playerInputHandler=pi;
    [SerializeField]
    private RectTransform cursorTransform;
    [SerializeField]
    private Canvas centerCanvas;
    [SerializeField]
    private RectTransform canvasRectTransform;
    [SerializeField]
    private float cursorSpeed = 10f;

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
            cursorTransform = cursorInit.GetCursorTransforms()[playerInputHandler.playerConfig.playerIndex];
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
        }
    }

    void OnDisable()
    {
        if(virtualMouse != null & virtualMouse.added)
            InputSystem.RemoveDevice(virtualMouse);
    }

    [SerializeField]private Vector2 deltaValue;
    private bool aButtonIsPressed;
    public void OnNavigate(InputAction.CallbackContext context) => deltaValue = context.ReadValue<Vector2>();
    public void OnClick(InputAction.CallbackContext context) => aButtonIsPressed = context.action.IsPressed();

    private void Update() {
        if(playerInputHandler.playerInstance.playerStatus!=PlayerInstance.PlayerStatus.Picking)
            return;

        if(playerInputHandler.playerConfig.input==null){
            Debug.LogError("ERROR : playerInput null reference");
            return;
        }
        UpdateMotion();
    }

    public void UpdateMotion(){
        if(virtualMouse == null){
            Debug.Log("ERROR : virtualMouse null reference");
            return;
        }

        deltaValue *= cursorSpeed * Time.deltaTime;
        deltaValue = new Vector2(Mathf.Clamp(deltaValue.x,-1f,1f)
            ,Mathf.Clamp(deltaValue.y,-1f,1f));

        Vector2 currentPosition = virtualMouse.position.ReadValue();
        Vector2 newPosition = currentPosition + deltaValue;

        newPosition.x =  Mathf.Clamp(newPosition.x, 0, Screen.width);
        newPosition.y =  Mathf.Clamp(newPosition.y, 0, Screen.height);

        InputState.Change(virtualMouse.position, newPosition);
        InputState.Change(virtualMouse.delta, deltaValue);

        if(previousMouseState != aButtonIsPressed){
            virtualMouse.CopyState<MouseState>(out var mouseState);
            mouseState.WithButton(MouseButton.Left, aButtonIsPressed);
            InputState.Change(virtualMouse, mouseState);
            previousMouseState = aButtonIsPressed;
        }

        AnchorCursor(newPosition);
    }

    private void AnchorCursor(Vector2 position){
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, position, centerCanvas.renderMode 
            == RenderMode.ScreenSpaceOverlay ? null : mainCamera, out anchoredPosition);

        cursorTransform.anchoredPosition = anchoredPosition;
    }
}
