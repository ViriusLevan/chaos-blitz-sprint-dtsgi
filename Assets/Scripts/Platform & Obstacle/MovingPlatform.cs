using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovingPlatform : MonoBehaviour
{
    public enum Direction
    {
        x,
        y,
        z
    }

    public Direction dir;
    [SerializeField] private Vector3 originalPosition;
    private Vector3 originalForward, originalUpward, originalRight;
    [SerializeField] private float distance;
    [SerializeField] private float duration;

    private void Start()
    {
        originalForward=transform.forward;
        originalUpward =transform.up;
        originalRight = transform.right;
        originalPosition = transform.position;
    }

    
    private void OnEnable() 
    {
        GameManager.platformingPhaseBegin+=Activate;
        GameManager.platformingPhaseFinished+=Reset;
    }

    private void OnDisable() 
    {
        GameManager.platformingPhaseBegin-=Activate;
        GameManager.platformingPhaseFinished-=Reset;
        DOTween.Kill(transform, false);
    }

    private void OnDestroy() 
    {
        GameManager.platformingPhaseBegin-=Activate;
        GameManager.platformingPhaseFinished-=Reset;
        DOTween.Kill(transform, false);
    }

    private void Activate()
    {
        MovePos();
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

    private void MovePos()
    {
        float destination=originalPosition.x;
        switch (dir) {
            case Direction.x :
                destination = (originalForward*distance + originalPosition).x;
                transform.DOMoveX(destination, 2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
                break;
            case Direction.y :
                destination = (originalUpward*distance + originalPosition).y;
                transform.DOMoveY(destination, 2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
                break;
            case Direction.z :
                destination = (originalRight*distance + originalPosition).y;
                transform.DOMoveZ(destination, 2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
                break;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.parent.SetParent(transform);
            other.rigidbody.interpolation = RigidbodyInterpolation.None;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.parent.SetParent(null);
            other.rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }
}
