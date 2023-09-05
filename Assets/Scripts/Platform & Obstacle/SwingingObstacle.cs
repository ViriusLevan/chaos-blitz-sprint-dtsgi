using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace LevelUpStudio.ChaosBlitzSprint.PlaceableBehaviour
{
    public class SwingingObstacle : MonoBehaviour
    {
        [SerializeField] private GameObject pivotPoint;
        [SerializeField] private float swingingSpeed = 60f;
        [SerializeField] private float swingAngle = 30f;   // Maximum swing angle in degrees
        private Quaternion middleRotation;
        private Quaternion rightRotation;
        private Quaternion leftRotation;
        private bool isSwingingRight = true;

        private void Start()
        {
            middleRotation = pivotPoint.transform.rotation;
            rightRotation = middleRotation * Quaternion.Euler(0f, 0f, swingAngle);
            leftRotation = middleRotation * Quaternion.Euler(0f, 0f, -swingAngle);
        }

        public void Swing()
        {
            StartCoroutine(Swinging());
        }

        private float CustomEaseInOutSine(float t)
        {
            return -0.5f * (Mathf.Cos(Mathf.PI * t) - 1f);
        }

        private IEnumerator Swinging()
        {
            Quaternion startRotation = pivotPoint.transform.rotation;
            Quaternion targetRotation = isSwingingRight ? rightRotation : leftRotation;

            float targetAngle = Quaternion.Angle(startRotation, targetRotation);
            float timeToReachTarget = targetAngle / swingingSpeed;

            float elapsedTime = 0f;
            while (elapsedTime < timeToReachTarget)
            {
                float t = elapsedTime / timeToReachTarget;
                float easedT = CustomEaseInOutSine(t); // Apply custom easing
                pivotPoint.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, easedT);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Complete the swing and switch the direction for the next swing
            pivotPoint.transform.rotation = targetRotation;
            isSwingingRight = !isSwingingRight;

            // Call Swing() again for continuous looping
            StartCoroutine(Swinging());
        }

        void OnEnable()
        {
            GameManager.platformingPhaseBegin+=Activate;
            GameManager.platformingPhaseFinished+=Reset;
        }

        void OnDestroy()
        {
            GameManager.platformingPhaseBegin-=Activate;
            GameManager.platformingPhaseFinished-=Reset;
            DOTween.Kill(transform, false);
        }

        private void Activate()
        {
            Swing();
        }

        private void Deactivate()
        {
            DOTween.Pause(transform);
        }

        private void Reset()
        {
            DOTween.Rewind(transform);
            DOTween.Kill(transform, true);
            //transform.position = originalPosition;
        }
    }
}