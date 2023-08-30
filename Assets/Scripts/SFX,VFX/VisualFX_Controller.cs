using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectEnum
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
public class VisualFX_Controller : MonoBehaviour
{
    public GameObject posSpawnEffect, posEffectCanon, posEffectBow, posBallTest;
    public GameObject VFX_Blood;
    public GameObject VFX_Water;
    public GameObject VFX_Jump;
    public GameObject VFX_Win;
    public GameObject VFX_Run;
    public GameObject VFX_Canon;
    public GameObject VFX_Arrow;
    public GameObject VFX_Place;

    public Dictionary<EffectEnum, GameObject> effectLibrary = new Dictionary<EffectEnum, GameObject>();
    private Quaternion initialRotation;

    public static VisualFX_Controller instance;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        effectLibrary[EffectEnum.WaterEffect] = VFX_Water;
        effectLibrary[EffectEnum.BloodEffect] = VFX_Blood;
        effectLibrary[EffectEnum.JumpEffect] = VFX_Jump;
        effectLibrary[EffectEnum.WinEffect] = VFX_Win;
        effectLibrary[EffectEnum.RunEffect] = VFX_Run;
        effectLibrary[EffectEnum.CanonEffect] = VFX_Canon; 
        effectLibrary[EffectEnum.ArrowEffect] = VFX_Arrow;
        effectLibrary[EffectEnum.PlaceEffect] = VFX_Place;
    }
    public GameObject SpawnEffect;
    public bool isCanonShot, isBowShot, isItemPlaced;
    public void PlayEffect(EffectEnum effect, Vector3 setPosEffect, Vector3 setRotEffect)
    {
        if (effectLibrary.ContainsKey(effect))
        {
            GameObject VFX = effectLibrary[effect];
            Quaternion rotation = Quaternion.Euler(VFX.transform.rotation.eulerAngles + setRotEffect);
            if (isCanonShot)
            {
                SpawnEffect = Instantiate(VFX, posEffectCanon.transform.position + setPosEffect, rotation) as GameObject;
                SpawnEffect.transform.SetParent(posEffectCanon.transform);
                isCanonShot = false;
            }
            else if (isBowShot)
            {
                SpawnEffect = Instantiate(VFX, posEffectBow.transform.position + setPosEffect, rotation) as GameObject;
                SpawnEffect.transform.SetParent(posEffectBow.transform);
                isBowShot = false;
            }
            else if (isItemPlaced)
            {
                SpawnEffect = Instantiate(VFX, posBallTest.transform.position + setPosEffect, rotation) as GameObject;
                SpawnEffect.transform.SetParent(posSpawnEffect.transform); //
                isItemPlaced = false;
            }
            else
            {
                SpawnEffect = Instantiate(VFX, posSpawnEffect.transform.position + setPosEffect, rotation) as GameObject;
                SpawnEffect.transform.SetParent(posSpawnEffect.transform);
            }            
            
        }
        else
        {
            Debug.LogError("Effect not found in the library.");
        }
    }
}
