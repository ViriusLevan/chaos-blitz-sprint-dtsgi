using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectEnum
{
    BloodEffect,
    WaterEffect,
    JumpEffect
}
public class VisualFX_Controller : MonoBehaviour
{
    public GameObject posSpawnEffect;
    public GameObject VFX_Blood;
    public GameObject VFX_Water;
    public GameObject VFX_Jump;

    public Dictionary<EffectEnum, GameObject> effectLibrary = new Dictionary<EffectEnum, GameObject>();
    private Quaternion initialRotation;

    private void Start()
    {
        effectLibrary[EffectEnum.WaterEffect] = VFX_Water;
        effectLibrary[EffectEnum.BloodEffect] = VFX_Blood;
        effectLibrary[EffectEnum.JumpEffect] = VFX_Jump;
    }
    public GameObject SpawnEffect;
    
    public void PlayEffect(EffectEnum effect, Vector3 setPosEffect, Vector3 setRotEffect)
    {
        if (effectLibrary.ContainsKey(effect))
        {
            GameObject VFX = effectLibrary[effect];
            Quaternion rotation = Quaternion.Euler(VFX.transform.rotation.eulerAngles + setRotEffect);
            SpawnEffect = Instantiate(VFX, posSpawnEffect.transform.position + setPosEffect, rotation) as GameObject;
            SpawnEffect.transform.SetParent(posSpawnEffect.transform);
        }
        else
        {
            Debug.LogError("Effect not found in the library.");
        }
    }
}
