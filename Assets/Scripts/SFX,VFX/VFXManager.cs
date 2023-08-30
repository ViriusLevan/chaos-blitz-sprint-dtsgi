using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VFXEnum
{
    BloodEffect,
    WaterEffect,
    JumpEffect,
    WinEffect,
    RunEffect,
    CanonEffect, 
    ArrowEffect,
    PlaceEffect
}

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance {get; private set;}

    public GameObject VFX_Blood;
    public GameObject VFX_Water;
    public GameObject VFX_Jump;
    public GameObject VFX_Win;
    public GameObject VFX_Run;
    public GameObject VFX_Canon;
    public GameObject VFX_Arrow;
    public GameObject VFX_Place;

    public Dictionary<VFXEnum, GameObject> effectLibrary = new Dictionary<VFXEnum, GameObject>();
    private Quaternion initialRotation;


    private void Awake()
    {
        if(Instance != null)
        {
            Debug.Log("[Singleton] Trying to instantiate a seccond instance of a singleton class.");
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
    }

    private void Start()
    {
        
        effectLibrary[VFXEnum.WaterEffect] = VFX_Water;
        effectLibrary[VFXEnum.BloodEffect] = VFX_Blood;
        effectLibrary[VFXEnum.JumpEffect] = VFX_Jump;
        effectLibrary[VFXEnum.WinEffect] = VFX_Win;
        effectLibrary[VFXEnum.RunEffect] = VFX_Run;
        effectLibrary[VFXEnum.CanonEffect] = VFX_Canon; 
        effectLibrary[VFXEnum.ArrowEffect] = VFX_Arrow;
        effectLibrary[VFXEnum.PlaceEffect] = VFX_Place;
        effectLibrary[VFXEnum.WaterEffect] = VFX_Water;
        effectLibrary[VFXEnum.BloodEffect] = VFX_Blood;
        effectLibrary[VFXEnum.JumpEffect] = VFX_Jump;
    }
    public void PlayEffect(VFXEnum effect, Vector3 setPosEffect, Vector3 setRotEffect)
    {
        if (effectLibrary.ContainsKey(effect))
        {
            GameObject VFX = effectLibrary[effect];
            Quaternion rotation = Quaternion.Euler(VFX.transform.rotation.eulerAngles + setRotEffect);
            GameObject SpawnEffect = Instantiate(VFX, setPosEffect, rotation);

            //SpawnEffect.transform.SetParent();
        }
        else
        {
            Debug.LogError("Effect not found in the library.");
        }
    }
}
