using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class CinemachineInputHandler : MonoBehaviour, AxisState.IInputAxisProvider
{
    [HideInInspector] public InputAction horizontal;
    [HideInInspector] public InputAction vertical;

    public float GetAxisValue(int axis)
    {
        switch (axis)
        {
            case 0: 
                return Mathf.Clamp(horizontal.ReadValue<Vector2>().x,-1,1);
            case 1: 
                return Mathf.Clamp(horizontal.ReadValue<Vector2>().y,-1,1);
            case 2: 
                return Mathf.Clamp(vertical.ReadValue<float>(),-1,1);
        }

        return 0;
    }
}