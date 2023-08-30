using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST : MonoBehaviour
{
    public SoundManager soundController;
    public VisualFX_Controller vfxController;

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

    public void playLaugh() //Sound Effect Hit by Obstacle
    {
        soundController.PlaySound(SoundEnum.LaughSound);
    }
    public void playFirework() //Sound Effect Hit by Obstacle
    {
        soundController.PlaySound(SoundEnum.FireworkSound);
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
    public void effectWin() //Visual Effect Character Jump
    {
        Invoke("playLaugh", 6/5); Invoke("playFirework", -1);
        vfxController.PlayEffect(EffectEnum.WinEffect, PRF.SetPosEffect, PRF.SetRotEffect);
    }
    public void effectCanon() //Visual Effect Canon
    {
        vfxController.isCanonShot = true;
        playBoom();
        vfxController.PlayEffect(EffectEnum.CanonEffect, PRF.SetPosEffect, PRF.SetRotEffect);
        
    }
    public void effectBow() //Visual Effect Arrow
    {
        vfxController.isBowShot = true;
        playArrow();
        vfxController.PlayEffect(EffectEnum.ArrowEffect, PRF.SetPosEffect, PRF.SetRotEffect);        
    }
    public GameObject _Ball, posSpawnBall;
    public void effectPlaceItem() //Visual Effect Arrow
    {        
        playPoof();
        GameObject prefabBall;
        prefabBall = Instantiate(_Ball, posSpawnBall.transform.position, posSpawnBall.transform.rotation);        
    }    
}
