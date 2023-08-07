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
    [SerializeField] private Vector3 originalPos;
    [SerializeField] private float distance;
    [SerializeField] private float duration;

    private void Start()
    {
        originalPos = transform.position;
        MovePos();
    }

    private void MovePos()
    {
        switch (dir) {
            case Direction.x :
                transform.DOMoveX(originalPos.x + distance, 2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
                break;
            case Direction.y :
                transform.DOMoveY(originalPos.y + distance, 2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
                break;
            case Direction.z :
                transform.DOMoveZ(originalPos.z + distance, 2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
                break;
        }
    }
}
