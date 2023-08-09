using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] private float fallTime = 0.5f;

	void OnCollisionEnter(Collision collision)
	{
		foreach (ContactPoint contact in collision.contacts)
		{
			if (collision.gameObject.tag == "Player")
			{
				StartCoroutine(Fall(fallTime));
			}
		}
	}

	IEnumerator Fall(float time)
	{
		yield return new WaitForSeconds(time);
		Destroy(gameObject);
	}
}
