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

    void Awake()
    {
    }

    public float GetAxisValue(int axis)
    {
        switch (axis)
        {
            case 0: 
                float resultX = look.ReadValue<Vector2>().x * sensMultiplier;
                if(invertX) resultX *=-1;
                //Debug.Log($"Result X = {resultX} from {look.ReadValue<Vector2>().x}");
                return resultX;
            case 1: 
                float resultY =  look.ReadValue<Vector2>().y * sensMultiplier;
                if(invertY) resultY *=-1;
                //Debug.Log($"Result Y = {resultY} from {look.ReadValue<Vector2>().y}");
                return resultY;
            case 2: 
                return Mathf.Clamp(vertical.ReadValue<float>(),-1,1);
        }
        return 0;
    }
    [SerializeField] private float minZoomDistance = 3.0f;
    [SerializeField] private float maxZoomDistance = 8.0f;
    public void Zoom(CallbackContext obj)
    {
        CinemachineFramingTransposer cft = 
            GetComponent<CinemachineVirtualCamera>().
                GetCinemachineComponent<CinemachineFramingTransposer>();
                        
        cft.m_CameraDistance = 
            Mathf.Clamp(cft.m_CameraDistance + obj.ReadValue<float>(),
                    minZoomDistance, maxZoomDistance);
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