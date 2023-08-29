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
    [SerializeField] private float distance;
    [SerializeField] private float duration;

    private void Start()
    {
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
        switch (dir) {
            case Direction.x :
                transform.DOMoveX(originalPosition.x + distance, 2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
                break;
            case Direction.y :
                transform.DOMoveY(originalPosition.y + distance, 2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
                break;
            case Direction.z :
                transform.DOMoveZ(originalPosition.z + distance, 2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
                break;
        }
    }
}
