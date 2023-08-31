using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumpyObstacle : MonoBehaviour
{
    [SerializeField] private float force = 10f; //Force 10000f
	[SerializeField] private float stunTime = 0.5f;
	private Vector3 hitDir;

	void OnCollisionEnter(Collision collision)
	{
		foreach (ContactPoint contact in collision.contacts)
		{
			if (collision.gameObject.tag == "Player")
			{
				hitDir = contact.normal;
				collision.gameObject.GetComponent<PlayerController>().HitPlayer(-hitDir * force, stunTime);
				return;
			}
		}
		/*if (collision.relativeVelocity.magnitude > 2)
		{
			if (collision.gameObject.tag == "Player")
			{
				//Debug.Log("Hit");
				collision.gameObject.GetComponent<CharacterControls>().HitPlayer(-hitDir*force, stunTime);
			}
			//audioSource.Play();
		}*/
	}
}
