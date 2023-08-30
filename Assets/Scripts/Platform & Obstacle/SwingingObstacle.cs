using UnityEngine;
using DG.Tweening;

public class SwingingObstacle : MonoBehaviour
{
    [SerializeField] private GameObject pivotPoint;
    [SerializeField] private float duration = 2f; // Duration of one full swing
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

    private void Swing()
    {
        Quaternion targetRotation = isSwingingRight ? rightRotation : leftRotation;

        pivotPoint.transform.DORotate(targetRotation.eulerAngles, duration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                // Switch the direction for the next swing
                isSwingingRight = !isSwingingRight;
                // Call Swing() again for continuous looping
                Swing();
            });
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
