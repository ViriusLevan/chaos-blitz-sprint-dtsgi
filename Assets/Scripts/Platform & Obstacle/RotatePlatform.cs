using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RotatePlatform : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private Vector3 finalRot;

    private void Start()
    {
        Rotate();
    }

    private void Rotate()
    {
        transform.DORotate(new Vector3(finalRot.x, finalRot.y, finalRot.z), duration, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.parent.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.parent.transform.SetParent(null);
        }
    }
}
