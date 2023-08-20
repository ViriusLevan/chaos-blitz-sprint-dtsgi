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
    JumpSound,
    PoofSound
}
public class SoundController : MonoBehaviour
{
    public AudioSource audioSource;

    public Dictionary<SoundEnum, AudioClip> soundLibrary = new Dictionary<SoundEnum, AudioClip>();
    public AudioClip waterSplashSFX;
    public AudioClip hitSFX;
    public AudioClip arrowSFX;
    public AudioClip canonSFX;
    public AudioClip paperSFX;
    public AudioClip jumpSFX;
    public AudioClip poofSFX;
    void Start()
    {
        soundLibrary[SoundEnum.WaterSound] = waterSplashSFX;
        soundLibrary[SoundEnum.HitSound] = hitSFX;
        soundLibrary[SoundEnum.ArrowSound] = arrowSFX;
        soundLibrary[SoundEnum.CanonSound] = canonSFX;
        soundLibrary[SoundEnum.PaperSound] = paperSFX;
        soundLibrary[SoundEnum.JumpSound] = jumpSFX;
        soundLibrary[SoundEnum.PoofSound] = poofSFX;
    }
    public void PlaySound(SoundEnum sound)
    {
        if (soundLibrary.ContainsKey(sound))
        {
            AudioClip clip = soundLibrary[sound];
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogError("Sound not found in the library.");
        }
    }
}
