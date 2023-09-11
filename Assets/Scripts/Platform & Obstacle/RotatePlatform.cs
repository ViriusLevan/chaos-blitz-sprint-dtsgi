using DG.Tweening;
using UnityEngine;

namespace LevelUpStudio.ChaosBlitzSprint.PlaceableBehaviour
{
    public class RotatePlatform : MonoBehaviour
    {
        [SerializeField] private GameObject rotatingObject;
        [SerializeField] private float rotationSpeed;

        private Vector3 rotForward, rotRight, rotUp;
        private void Awake()
        {
        }

        private void Update() 
        {
        }

        public enum RotationAxis{x,y,z};
        [SerializeField] private RotationAxis rotAxis=RotationAxis.y;

        void OnEnable()
        {
            rotForward=rotatingObject.transform.forward;
            rotRight=rotatingObject.transform.right;
            rotUp=rotatingObject.transform.up;
            GameManager.platformingPhaseBegin+=Activate;
			GameManager.gamePaused+=Deactivate;
			GameManager.gamePaused+=Reactivate;
            GameManager.platformingPhaseFinished+=Reset;
        }

        void OnDisable()
        {
            GameManager.platformingPhaseBegin-=Activate;
			GameManager.gamePaused-=Deactivate;
			GameManager.gamePaused-=Reactivate;
            GameManager.platformingPhaseFinished-=Reset;
            // DOTween.Kill(transform, false);
        }

        private void Activate()
        {
            switch(rotAxis){
                case RotationAxis.x:
                    rotatingObject.transform
                        .DOLocalRotate(new Vector3
                            (360,rotatingObject.transform.rotation.y,rotatingObject.transform.rotation.z)
                            ,5,RotateMode.LocalAxisAdd)
                            .SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
                    break;
                
                case RotationAxis.y:
                    rotatingObject.transform
                        .DOLocalRotate(new Vector3
                            (rotatingObject.transform.rotation.x,360,rotatingObject.transform.rotation.z)
                            ,5,RotateMode.LocalAxisAdd)
                            .SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
                    break;
                
                case RotationAxis.z:
                    rotatingObject.transform
                        .DOLocalRotate(new Vector3
                            (rotatingObject.transform.rotation.x,rotatingObject.transform.rotation.y,360)
                            ,5,RotateMode.LocalAxisAdd)
                            .SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
                    break;
                }
        }

        private void Deactivate()
        {
            DOTween.Pause(rotatingObject.transform);
        }
		private void Reactivate()
		{
            DOTween.Restart(rotatingObject.transform);
		}

        private void Reset()
        {
            DOTween.Rewind(rotatingObject.transform);
        }
        private void OnCollisionEnter(Collision other)
        {
            if (other.collider.gameObject.CompareTag("Player"))
            {
                Debug.Log($"Player Collision Enter {other.gameObject.name}");
                other.gameObject.transform.parent.SetParent(rotatingObject.transform);
                other.rigidbody.interpolation = RigidbodyInterpolation.None;
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.collider.gameObject.CompareTag("Player"))
            {
                Debug.Log("Player Collision Exit");
                other.gameObject.transform.parent.SetParent(null);
                other.rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            }
        }
    }
}