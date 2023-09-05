using System.Collections;
using UnityEngine;

namespace LevelUpStudio.ChaosBlitzSprint.PlaceableBehaviour
{
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
			GameManager.gamePaused+=Deactivate;
			GameManager.gameUnpaused+=Reactivate;
			GameManager.platformingPhaseFinished+=Reset;
		}

		void OnDisable()
		{
			GameManager.platformingPhaseBegin-=Activate;
			GameManager.gamePaused-=Deactivate;
			GameManager.gameUnpaused-=Reactivate;
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
						FallDown();
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
			if(Time.deltaTime*speed>1f)
			{
				triggered=false;
				counter=0;
				transform.position = originalPosition;
			}
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
		private void Reactivate()
		{
			active=true;
		}

		private void Reset()
		{
			// DOTween.Rewind(transform);
			// DOTween.Kill(transform, true);
			//transform.position = originalPosition;
		}
	}
}