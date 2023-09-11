using UnityEngine;
using DG.Tweening;

namespace LevelUpStudio.ChaosBlitzSprint.PlaceableBehaviour
{
    public class MovingPlatform : MonoBehaviour
    {
        public enum MovementAxis{X,Y,Z}

        public MovementAxis dir;
        [SerializeField] private float distance;
        [SerializeField] private float duration=3;

        private Vector3 originalPosition, originalRight, originalUp;
        private void Awake()
        {
        }

        private void Update() 
        {
        }
        
        private void OnEnable() 
        {
            originalPosition = transform.position;
            originalRight = transform.right;
            originalUp = transform.up;
            GameManager.platformingPhaseBegin+=Activate;
			GameManager.gamePaused+=Deactivate;
			GameManager.gameUnpaused+=Activate;
            GameManager.platformingPhaseFinished+=Reset;
        }

        private void OnDisable() 
        {
            GameManager.platformingPhaseBegin-=Activate;
            GameManager.platformingPhaseFinished-=Reset;
			GameManager.gamePaused-=Deactivate;
			GameManager.gameUnpaused-=Reactivate;
            DOTween.Kill(transform, false);
        }

        private void OnDestroy() 
        {
            GameManager.platformingPhaseBegin-=Activate;
            GameManager.platformingPhaseFinished-=Reset;
			GameManager.gamePaused-=Deactivate;
			GameManager.gameUnpaused-=Reactivate;
            DOTween.Kill(transform, false);
        }

        private void Activate()
        {
            switch(dir)
            {
                case MovementAxis.Z:
                    //Debug.Log($"POS {originalPosition} + Z{originalRight*distance}");
                    //Debug.Log($"RESULTANT {originalPosition + (originalRight*distance)}");
                    transform.DOMove(originalPosition + (originalRight*distance), duration)
                        .SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
                    break;
                case MovementAxis.Y:
                //Debug.Log($"POS {originalPosition} + Y{originalUp*distance}");
                ///Debug.Log($"RESULTANT {originalPosition + (originalUp*distance)}");
                    transform.DOMove(originalPosition + (originalUp*distance), duration)
                        .SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
                    break;
            }
        }

        private void Deactivate()
        {
            DOTween.Pause(transform);
        }
        private void Reactivate()
        {
            DOTween.Pause(transform);
        }
        private void Reset()
        {
            DOTween.Rewind(transform);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.collider.gameObject.CompareTag("Player"))
            {
                other.gameObject.transform.parent.SetParent(transform);
                other.rigidbody.interpolation = RigidbodyInterpolation.None;
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.collider.gameObject.CompareTag("Player"))
            {
                other.gameObject.transform.parent.SetParent(null);
                other.rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            }
        }
    }
}