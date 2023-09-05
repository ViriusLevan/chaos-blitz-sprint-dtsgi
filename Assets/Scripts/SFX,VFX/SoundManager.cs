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
    PlayerDeath,
    LaughSound,
    FireworkSound
}
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }


    [SerializeField]private AudioSource aSourceBGM;

    public Dictionary<SoundEnum, AudioClip> soundLibrary = new Dictionary<SoundEnum, AudioClip>();
    public AudioClip waterSplashSFX;
    public AudioClip hitSFX;
    public AudioClip arrowSFX;
    public AudioClip canonSFX;
    public AudioClip paperSFX;
    public AudioClip playerJump;
    public AudioClip poofSFX;
    public AudioClip playerFinish;
    public AudioClip playerDeath;
    public AudioClip laughSFX; 
    public AudioClip FireworkSFX;


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
        if (!PlayerPrefs.HasKey("musicVolume"))
        {            
            SoundManager.Instance?.SetMusicVolume( 0.3f);
            SoundManager.Instance?.SetSFXVolume(0.10f);
        }
        else
        {
            musicVolume = PlayerPrefs.GetFloat("musicVolume");
            sfxVolume = PlayerPrefs.GetFloat("sfxVolume");
        }
    }

    void Start()
    {
        soundLibrary[SoundEnum.WaterSound] = waterSplashSFX;
        soundLibrary[SoundEnum.HitSound] = hitSFX;
        soundLibrary[SoundEnum.ArrowSound] = arrowSFX;
        soundLibrary[SoundEnum.CanonSound] = canonSFX;
        soundLibrary[SoundEnum.PaperSound] = paperSFX;
        soundLibrary[SoundEnum.PoofSound] = poofSFX;
        soundLibrary[SoundEnum.PlayerJump] = playerJump;
        soundLibrary[SoundEnum.PlayerFinish] = playerFinish;
        soundLibrary[SoundEnum.PlayerDeath] = playerDeath;
        soundLibrary[SoundEnum.LaughSound] = laughSFX; 
        soundLibrary[SoundEnum.FireworkSound] = FireworkSFX;
    }
    public void PlaySound(SoundEnum sound)
    {
        if (soundLibrary.ContainsKey(sound))
        {
            AudioClip clip = soundLibrary[sound];
            //audioSource.PlayOneShot(clip);
            AudioSource.PlayClipAtPoint(clip, transform.position,sfxVolume);
            Debug.Log("SoundEnum="+sound.ToString());
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

    
    public float musicVolume {get; private set;}
    public float sfxVolume {get; private set;}

    public delegate void OnVolumeChanged();
    public static event OnVolumeChanged musicVolumeChange, sfxVolumeChange;

    public void SetMusicVolume(float newVol)
    {
        musicVolume = newVol;
        musicVolumeChange?.Invoke();
        aSourceBGM.volume = newVol;
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float newVol)
    {
        sfxVolume = newVol;
        sfxVolumeChange?.Invoke();
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
        PlayerPrefs.Save();
    }
}
