using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyLoop : MonoBehaviour
{
    public ParticleSystem particle;
    public TEST _test;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    //public ParticleSystem smokeRun;
    public bool isDestroyed;
    void Update()
    {
        if (isDestroyed)
        {
           // Debug.Log(_test.stopRun);
            particle = GetComponent<ParticleSystem>();
            particle.Stop(); // Menghentikan emisi partikel
            float duration = particle.main.duration; // Mendapatkan durasi partikel
            Destroy(gameObject, duration);
        }
        else
        {
            return;
        }        
    }    
    public Position_Rotation_Effect PRF;
    public void playRunEffect(bool isRun = true)
    {
        if (isRun)
        {
            VisualFX_Controller.instance.PlayEffect(EffectEnum.RunEffect, PRF.SetPosEffect, PRF.SetRotEffect);
        }
        else
        {
            Destroy(VisualFX_Controller.instance.SpawnEffect);
        }
    }
    
}
