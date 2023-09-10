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
            originalPosition = transform.position;
            originalRotation = transform.localRotation;
            originalForward = transform.forward;
            originalUp = transform.up;
            switch(movement)
            {
                case MovementType.xRotation:
                    break;
                case MovementType.yRotation:
                    transform.DORotate(new Vector3(0,360,0)
                        ,5,RotateMode.LocalAxisAdd)
                        .SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);;
                    break;
                case MovementType.zSwing:
                    transform.DORotate(new Vector3(0,0,originalRotation.eulerAngles.y+90)
                        ,4,RotateMode.LocalAxisAdd)
                        .SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);;
                    break;
                case MovementType.forwardBack:
                     transform.DOMove(originalPosition + (originalForward*3), 3)
                        .SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
                    break;
                case MovementType.upDown:
                     transform.DOMove(originalPosition+ (originalUp*3), 3)
                        .SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
                    break;
            }
        }


        void Update()
        {
            
        }

        void OnDisable()
        {
            //TODO add DoTween.Kill here
            DOTween.Kill(transform, false);
        }
    }
}
