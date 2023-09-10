using UnityEngine;
using LevelUpStudio.ChaosBlitzSprint.Player;

namespace LevelUpStudio.ChaosBlitzSprint.PlaceableBehaviour
{
	public class BumpyObstacle : MonoBehaviour
	{
		[SerializeField] private float force = 50f; //Force 10000f
		[SerializeField] private float stunTime = 0.5f;
		private Vector3 hitDir;

		void OnCollisionEnter(Collision collision)
		{
			//Debug.Log(collision.gameObject.name+"0>"+collision.gameObject.tag);
			foreach (ContactPoint contact in collision.contacts)
			{
				if (collision.gameObject.tag == "Player")
				{
					Debug.Log("Bumpy Triggered");
					hitDir = contact.normal;
					collision.gameObject.GetComponent<PlayerController>()
						.HitPlayer(-hitDir * force, stunTime);
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
}