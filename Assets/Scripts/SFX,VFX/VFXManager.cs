using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VFXEnum
{
    Blood,Water,Jump,Win,Cannon,Arrow,Poof,Explosion
}

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance {get; private set;}

    public GameObject VFX_Blood;
    public GameObject VFX_Water;
    public GameObject VFX_Jump;
    public GameObject VFX_Win;
    public GameObject VFX_Cannon;
    public GameObject VFX_Arrow;
    public GameObject VFX_Poof;
    public GameObject VFX_Explosion;


    public Dictionary<VFXEnum, GameObject> effectLibrary = new Dictionary<VFXEnum, GameObject>();
    private Dictionary<VFXEnum, SoundEnum> vfxSoundKey = new Dictionary<VFXEnum, SoundEnum>();

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
        
        effectLibrary[VFXEnum.Water] = VFX_Water;
        effectLibrary[VFXEnum.Blood] = VFX_Blood;
        effectLibrary[VFXEnum.Jump] = VFX_Jump;
        effectLibrary[VFXEnum.Win] = VFX_Win;
        effectLibrary[VFXEnum.Cannon] = VFX_Cannon; 
        effectLibrary[VFXEnum.Arrow] = VFX_Arrow;
        effectLibrary[VFXEnum.Poof] = VFX_Poof;
        effectLibrary[VFXEnum.Blood] = VFX_Blood;
        effectLibrary[VFXEnum.Jump] = VFX_Jump;
        effectLibrary[VFXEnum.Explosion] = VFX_Explosion;

        
        vfxSoundKey[VFXEnum.Water] = SoundEnum.Splash;
        vfxSoundKey[VFXEnum.Blood] = SoundEnum.Hurt;
        vfxSoundKey[VFXEnum.Jump] = SoundEnum.PlayerJump;
        vfxSoundKey[VFXEnum.Win] = SoundEnum.WooHoo;
        vfxSoundKey[VFXEnum.Cannon] = SoundEnum.Cannon; 
        vfxSoundKey[VFXEnum.Arrow] = SoundEnum.Arrow;
        vfxSoundKey[VFXEnum.Poof] = SoundEnum.PoofSound;
        vfxSoundKey[VFXEnum.Explosion] = SoundEnum.Explosion;
    }
    public void PlayEffect(VFXEnum effect, Vector3 position, Vector3 eulerRotation)
    {
        if (effectLibrary.ContainsKey(effect))
        {
            GameObject VFX = effectLibrary[effect];
            Quaternion rotation = Quaternion.Euler(VFX.transform.rotation.eulerAngles + eulerRotation);
            Instantiate(VFX, position, rotation);
            if(vfxSoundKey.ContainsKey(effect))
                SoundManager.Instance?.PlaySound(vfxSoundKey[effect]);
        }
        else
        {
            Debug.LogError("Effect not found in the library.");
        }
    }

    public void PlayEffectSilent(VFXEnum effect, Vector3 setPosEffect, Vector3 setRotEffect)
    {
        if (effectLibrary.ContainsKey(effect))
        {
            GameObject VFX = effectLibrary[effect];
            Quaternion rotation = Quaternion.Euler(VFX.transform.rotation.eulerAngles + setRotEffect);
            Instantiate(VFX, setPosEffect, rotation);
        }
        else
        {
            Debug.LogError("Effect not found in the library.");
        }
    }


}
