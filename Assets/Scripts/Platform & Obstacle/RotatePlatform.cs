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
    }

    private void Rotate()
    {
        transform.DORotate(new Vector3(finalRot.x, finalRot.y, finalRot.z), duration, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
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
        Rotate();
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
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player Collision Enter");
            other.transform.parent.SetParent(transform);
            other.rigidbody.interpolation = RigidbodyInterpolation.None;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player Collision Exit");
            other.transform.parent.SetParent(null);
            other.rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }
}
