using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class CinemachineInputHandler : MonoBehaviour, AxisState.IInputAxisProvider
{
    public InputAction horizontal;
 public InputAction vertical;

    [SerializeField]private bool invertX, invertY;
    [SerializeField]private float sensMultiplier=1f, minSens=0.5f, maxSens=2f;
    public float GetAxisValue(int axis)
    {
        switch (axis)
        {
            case 0: 
                float resultX = Mathf.Clamp
                    (horizontal.ReadValue<Vector2>().x * sensMultiplier
                    ,-1,1);
                if(invertX) resultX *=-1;
                return resultX;
            case 1: 
                float resultY =  Mathf.Clamp
                    (horizontal.ReadValue<Vector2>().y * sensMultiplier
                    ,-1,1);
                if(invertY) resultY *=-1;
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