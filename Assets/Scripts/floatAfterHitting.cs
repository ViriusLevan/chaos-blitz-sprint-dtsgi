using UnityEngine;
using DG.Tweening;

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

        private Vector3 hitPosition,hitUp;
        private void OnCollisionEnter(Collision other) 
        {
            if(!active){
                active=true;
                rb.isKinematic=true;
                hitPosition = transform.position;
                hitUp  = transform.up;
                extraCollider.enabled=false;
                transform.DOLocalMove(hitPosition + (hitUp*2), 1)
                    .SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            }
        }

        void OnDisable()
        {
            DOTween.Kill(transform, false);
        }
    }
}
