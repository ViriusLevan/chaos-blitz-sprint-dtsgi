using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundEnum
{
    WaterSound,
    HitSound,
    ArrowSound,
    CanonSound,
    PaperSound,
    PoofSound,
    PlayerJump,
    PlayerFinish,
    PlayerDeath
}
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }


    public AudioSource aSourceBGM;

    public Dictionary<SoundEnum, AudioClip> soundLibrary = new Dictionary<SoundEnum, AudioClip>();
    // public AudioClip waterSplashSFX;
    // public AudioClip hitSFX;
    // public AudioClip arrowSFX;
    // public AudioClip canonSFX;
    // public AudioClip paperSFX;
    // public AudioClip jumpSFX;
    // public AudioClip poofSFX;
    public AudioClip playerJump;
    public AudioClip playerFinish;
    public AudioClip playerDeath;


    private void Awake()
    {
        if(Instance != null)
        {
            Debug.Log("[Singleton] Trying to instantiate a seccond instance of a singleton class.");
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
    }

    void Start()
    {
        // soundLibrary[SoundEnum.WaterSound] = waterSplashSFX;
        // soundLibrary[SoundEnum.HitSound] = hitSFX;
        // soundLibrary[SoundEnum.ArrowSound] = arrowSFX;
        // soundLibrary[SoundEnum.CanonSound] = canonSFX;
        // soundLibrary[SoundEnum.PaperSound] = paperSFX;
        // soundLibrary[SoundEnum.JumpSound] = jumpSFX;
        // soundLibrary[SoundEnum.PoofSound] = poofSFX;
        soundLibrary[SoundEnum.PlayerJump] = playerJump;
        soundLibrary[SoundEnum.PlayerFinish] = playerFinish;
        soundLibrary[SoundEnum.PlayerDeath] = playerDeath;
    }
    public void PlaySound(SoundEnum sound)
    {
        if (soundLibrary.ContainsKey(sound))
        {
            AudioClip clip = soundLibrary[sound];
            //audioSource.PlayOneShot(clip);
            AudioSource.PlayClipAtPoint(clip, transform.position,1f);
        }
        else
        {
            Debug.LogError("Sound not found in the library.");
        }
    }

    public void SetBGM()
    {
        aSourceBGM.Play();
    }
}
