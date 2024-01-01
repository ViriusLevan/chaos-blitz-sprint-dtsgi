using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelUpStudio.ChaosBlitzSprint
{
    public class ObstacleBombBarrel : MonoBehaviour
    {
        [SerializeField]private float countdownTime=3f;
        private float counter=0f;
        [SerializeField]private bool triggered=false;
        
        void Update()
        {
            if(triggered)
            {
                counter+=Time.deltaTime;
                if(counter>=countdownTime)
                    Explode();
            }
        }

        private void OnCollisionEnter(Collision other) 
        {
            if(other.gameObject.CompareTag("Player"))
                triggered=true;
        }

        private void Explode()
        {
			VFXManager.Instance?.PlayEffect(VFXEnum.Explosion
				, transform.position
				, new Vector3(90,0,0));
            Destroy(gameObject);
        }
    }
}
