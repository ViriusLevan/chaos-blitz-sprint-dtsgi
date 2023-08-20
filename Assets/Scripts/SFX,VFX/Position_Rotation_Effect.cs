using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position_Rotation_Effect : MonoBehaviour
{
    public static Position_Rotation_Effect instance;
    private void Awake()
    {
        instance = this;
    }
    public Vector3 SetPosEffect = new Vector3(0f, 0f, 0f);
    public Vector3 SetRotEffect = new Vector3(0f, 0f, 0f);
    void Start()
    {
        transform.position = SetPosEffect;
        
        transform.rotation = Quaternion.Euler(SetRotEffect);
    }
}
