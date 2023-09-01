using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] private float fallTime = 0.5f;
	private Vector3 originalPosition;

	private void Start() {
		originalPosition = transform.position;
	}

	void OnCollisionEnter(Collision collision)
	{
		foreach (ContactPoint contact in collision.contacts)
		{
			if (collision.gameObject.tag == "Player")
			{
				triggered=true;
			}
		}
	}

	IEnumerator Fall(float time)
	{
		yield return new WaitForSeconds(time);
		gameObject.SetActive(false);
	}

	void OnEnable()
    {
        GameManager.platformingPhaseBegin+=Activate;
        GameManager.platformingPhaseFinished+=Reset;
    }

    void OnDisable()
    {
        GameManager.platformingPhaseBegin-=Activate;
        GameManager.platformingPhaseFinished-=Reset;
        // DOTween.Kill(transform, false);
    }

	private bool active=false, triggered=false;
	private float counter=0;
	void Update()
	{
		if(active)
		{
			if(triggered)
			{
				counter+=Time.deltaTime;
				if(counter>=fallTime)
				{
					triggered=false;
					counter=0;

				}
			}
		}
	}

	[SerializeField]float speed = 10f;
	private void FallDown()
	{
		transform.position = 	
			Vector3.Lerp(
				transform.position,
				originalPosition+new Vector3(0,-10,0),
				Time.deltaTime*speed
			);
	
	}

	private void Activate()
    {
		transform.position=originalPosition;
        active=true;
    }

    private void Deactivate()
    {
        active=false;
        // DOTween.Pause(transform);
    }

    private void Reset()
    {
        // DOTween.Rewind(transform);
        // DOTween.Kill(transform, true);
        //transform.position = originalPosition;
    }

}
