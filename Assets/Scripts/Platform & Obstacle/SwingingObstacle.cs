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

        Swing();
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
}
