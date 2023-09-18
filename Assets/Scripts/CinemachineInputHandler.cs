using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class CinemachineInputHandler : MonoBehaviour, AxisState.IInputAxisProvider
{
    public InputAction horizontal;
    public InputAction vertical;

    [SerializeField]public bool invertX {get;private set;}
    [SerializeField]public bool invertY {get;private set;}

    void Awake()
    {
    }
    [SerializeField] private float minZoomDistance = 3.0f;
    [SerializeField] private float maxZoomDistance = 8.0f;
    public float GetAxisValue(int axis)
    {
        switch (axis)
        {
            case 0: 
                float resultX = horizontal.ReadValue<float>() * sensMultiplier;
                if(invertX) resultX *=-1;
                //Debug.Log($"Result X = {resultX} from {horizontal.ReadValue<float>()}");
                return resultX;
            case 1: 
                float resultY =  vertical.ReadValue<float>() * sensMultiplier;
                if(invertY) resultY *=-1;
                //Debug.Log($"Result Y = {resultY} from {vertical.ReadValue<float>()}");
                CinemachineFramingTransposer cft = GetComponent<CinemachineVirtualCamera>().
                    GetCinemachineComponent<CinemachineFramingTransposer>();
                
                cft.m_CameraDistance = 
                    Mathf.Clamp(cft.m_CameraDistance + resultY,
                            minZoomDistance, maxZoomDistance);

                return resultY;
            case 2: 
                return Mathf.Clamp(vertical.ReadValue<float>(),-1,1);
        }
        return 0;
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

    [SerializeField]public float sensMultiplier=1f, minSens=0.5f, maxSens=2f;
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

    // public void ZoomCamera(float input)
    // {
    //     // actual zoom mechanic. taking account if the control is inverted
    //     //, sensitivity, and min max distance
    //     float inputValue = context.ReadValue<float>() * cinemachineInputHandler.sensMultiplier;
    //     if(cinemachineInputHandler.invertY) inputValue *=-1;

    //     cft.m_CameraDistance = 
    //         Mathf.Clamp(cft.m_CameraDistance + inputValue,
    //                 minZoomDistance, maxZoomDistance);
    // }
}