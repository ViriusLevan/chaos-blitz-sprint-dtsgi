using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using static UnityEngine.InputSystem.InputAction;

public class CinemachineInputHandler : MonoBehaviour, AxisState.IInputAxisProvider
{
    public InputAction look;
    public InputAction vertical;

    [SerializeField]public bool invertX {get;private set;}
    [SerializeField]public bool invertY {get;private set;}

    // -- zoom control --
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    private CinemachineComponentBase componentBase;
    float cameraDistance;
    [SerializeField] private float defaultCameraBuildZoom = 10f;
    [SerializeField] private float defaultCameraPlayZoom = 8f;
    [SerializeField] private float cameraZoomModifier = 2f;
    [SerializeField] private float minZoomPlayDistance = 5.0f;
    [SerializeField] private float maxZoomPlayDistance = 10.0f;
    [SerializeField] private float minZoomBuildDistance = 5.0f;
    [SerializeField] private float maxZoomBuildDistance = 25.0f;
    [SerializeField] private bool invertedScroll = false;
    private CinemachineFramingTransposer cft;

    void Awake()
    {
        cft = GetComponent<CinemachineVirtualCamera>().
                GetCinemachineComponent<CinemachineFramingTransposer>();
        // temporarily combine build and play zoom default value
        cft.m_CameraDistance = defaultCameraBuildZoom;
    }


    public float GetAxisValue(int axis)
    {
        switch (axis)
        {
            case 0:
                float resultX = look.ReadValue<Vector2>().x; // * sensMultiplier;
                if(invertX) resultX *=-1;
                //Debug.Log($"Result X = {resultX} from {look.ReadValue<Vector2>().x}");
                return resultX;
            case 1:
                float resultY = look.ReadValue<Vector2>().y * -1; // * sensMultiplier;
                if(invertY) resultY *=-1;
                //Debug.Log($"Result Y = {resultY} from {look.ReadValue<Vector2>().y}");
                return resultY;
            case 2:
                return vertical.ReadValue<float>();
                //return Mathf.Clamp(vertical.ReadValue<float>(),-1,1);
        }
        return 0;
    }


    public void Zoom(CallbackContext obj)
    {
        //cft.m_CameraDistance = 
        //    Mathf.Clamp(cft.m_CameraDistance + obj.ReadValue<float>(),
        //            minZoomDistance, maxZoomDistance);

        // --- temporary fix, in final build, zoom level for build and play mode should be separated, and build mode should be able to zoom out farther ---
        // zoom limit in build mode
        cft.m_CameraDistance =
        Mathf.Clamp(cft.m_CameraDistance -
                (invertedScroll ? obj.ReadValue<float>() : obj.ReadValue<float>() / cameraZoomModifier),
                minZoomBuildDistance,
                maxZoomBuildDistance);

        // zoom limit in play mode
        /*
        cft.m_CameraDistance =
        Mathf.Clamp(cft.m_CameraDistance -
                (invertedScroll ? obj.ReadValue<float>() : obj.ReadValue<float>() / cameraZoomModifier),
                minZoomPlayDistance,
                maxZoomPlayDistance); 
        */
    }


    public void InvertX()
    {
        invertX = !invertX;
        Debug.Log("Invert X = "+invertX.ToString());
    }


    public void InvertY()
    {
        invertY = !invertY;
        Debug.Log("Invert Y = "+invertY.ToString());
    }


    [SerializeField]public float sensMultiplier=30f, minSens=10f, maxSens=50f;
    private float sensInterval = 0.1f;
    public void IncreaseSensitivity()
    {
        if(sensMultiplier+sensInterval<=maxSens)
        {
            sensMultiplier+=sensInterval;
            Debug.Log("Sens Increased");
        }
        else
        {
            Debug.Log("Sens already maxed");
        }
    }


    public void DecreaseSensitivity()
    {
        if(sensMultiplier-sensInterval>=minSens)
        {
            sensMultiplier-=sensInterval;
            Debug.Log("Sens Decreased");
        }
        else
        {
            Debug.Log("Sens already minimum");
        }
    }
}