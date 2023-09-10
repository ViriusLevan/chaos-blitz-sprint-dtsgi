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
                Debug.Log($"pos {originalPosition} + OrigForward {originalForward*3}");
                Debug.Log($"Resultant {originalPosition + (originalForward*3)}");
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
