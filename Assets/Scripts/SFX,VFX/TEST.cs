using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST : MonoBehaviour
{
    public SoundManager soundController;
    public VFXManager vfxController;

    public void playPaper() //Sound Effect Paper Change Scene
    {
        soundController.PlaySound(SoundEnum.PaperSound);
    }
    public void playBoom() //Sound Effect Canon
    {
        soundController.PlaySound(SoundEnum.CanonSound);
    }
    public void playArrow() //Sound Effect Arrow
    {
        soundController.PlaySound(SoundEnum.ArrowSound);
    }
    public void playWater() //Sound Effect Fall to Sea
    {
        soundController.PlaySound(SoundEnum.WaterSound);
    }
    public void playHit() //Sound Effect Hit by Obstacle
    {
        soundController.PlaySound(SoundEnum.HitSound);
    }
    public void playJump() //Sound Effect Hit by Obstacle
    {
        soundController.PlaySound(SoundEnum.PlayerJump);
    }
    public void playPoof() //Sound Effect Hit by Obstacle
    {
        soundController.PlaySound(SoundEnum.PoofSound);
    }


    //==========================================================================================================================
    public Position_Rotation_Effect PRF;
    public void effectWater() //Visual Effect Splash Water
    {
        playWater();
        vfxController.PlayEffect(EffectEnum.WaterEffect, PRF.SetPosEffect, PRF.SetRotEffect);
    }
    public void effectBlood() //Visual Effect Character Blood
    {
        playHit();
        vfxController.PlayEffect(EffectEnum.BloodEffect, PRF.SetPosEffect, PRF.SetRotEffect);
    }
    public void effectJump() //Visual Effect Character Jump
    {
        playJump();
        vfxController.PlayEffect(EffectEnum.JumpEffect, PRF.SetPosEffect, PRF.SetRotEffect);
    }
}
