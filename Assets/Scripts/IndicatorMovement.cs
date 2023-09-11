using UnityEngine;
using DG.Tweening;

namespace LevelUpStudio.ChaosBlitzSprint
{
    public class IndicatorMovement : MonoBehaviour
    {
        public enum MovementType {xRotation, yRotation, zSwing, forwardBack, upDown}
        [SerializeField]private MovementType movement;
        private Vector3 originalPosition, originalForward, originalUp;
        private Quaternion originalRotation;
        void Start()
        {
            originalPosition = transform.localPosition;
            originalRotation = transform.localRotation;
            originalForward = transform.forward;
            originalUp = transform.up;
            switch(movement)
            {
                case MovementType.xRotation:
                    break;
                case MovementType.yRotation:
                    transform.DOLocalRotate(new Vector3(transform.rotation.x,360,transform.rotation.z)
                        ,5,RotateMode.LocalAxisAdd)
                        .SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
                    // transform.DORotate(new Vector3(0,360,0)
                    //     ,5,RotateMode.LocalAxisAdd)
                    //     .SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);;
                    break;
                case MovementType.zSwing:
                    transform.DOLocalRotate(new Vector3
                        (transform.rotation.x
                        ,transform.rotation.y
                        ,originalRotation.eulerAngles.z+90)
                            ,4,RotateMode.Fast)
                            .SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
                    break;
                case MovementType.forwardBack:
                    transform.DOLocalMove(originalPosition + (originalForward*3), 3)
                        .SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
                    break;
                case MovementType.upDown:
                //Debug.Log($"OrigUpward {originalUp}");
                    transform.DOLocalMove(originalPosition + (originalUp*3), 3)
                        .SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
                    break;
            }
        }


        void Update()
        {
            
        }

        void OnDisable()
        {
            DOTween.Kill(transform, false);
        }
    }
}
