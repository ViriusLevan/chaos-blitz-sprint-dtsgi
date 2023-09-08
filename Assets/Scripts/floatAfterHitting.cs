using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelUpStudio.ChaosBlitzSprint
{
    [RequireComponent(typeof(Rigidbody))]
    public class floatAfterHitting : MonoBehaviour
    {
        private Rigidbody rb;
        private bool active=false;
        [SerializeField]private BoxCollider extraCollider;
        void Awake()
        {
            rb = this.GetComponent<Rigidbody>();
            hitPosition = new Vector3();
        }

        private bool loop;
        void Update()
        {
            if(active)
            {
                if(transform.position == hitPosition+new Vector3(0,2,0))
                {
                    loop=true;
                }else if(transform.position == hitPosition)
                {
                    loop=false;
                }
                if(!loop){
                    transform.position = 	
                        Vector3.Lerp(
                            transform.position,
                            hitPosition+new Vector3(0,2,0),
                            Time.deltaTime*10
                        );
                }else{    
                    transform.position = 	
                        Vector3.Lerp(
                            transform.position,
                            hitPosition,
                            Time.deltaTime*10
                        );
                }
            }
        }
        private Vector3 hitPosition;
        private void OnCollisionEnter(Collision other) 
        {
            if(!active){
                active=true;
                rb.isKinematic=true;
                hitPosition = transform.position;
                extraCollider.enabled=false;
            }
        }
    }
}
