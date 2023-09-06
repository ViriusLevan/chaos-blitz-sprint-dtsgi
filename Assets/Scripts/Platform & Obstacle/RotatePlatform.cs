using System.Collections;
using UnityEngine;

namespace LevelUpStudio.ChaosBlitzSprint.PlaceableBehaviour
{
    public class RotatePlatform : MonoBehaviour
    {
        [SerializeField] private GameObject rotatingObject;
        [SerializeField] private float rotationSpeed;

        private void Start()
        {
        }

        private bool active=false;
        private void Update() 
        {
            if(active)
                Rotate();
        }

        private void Rotate()
        {
            float rotationAmount = rotationSpeed * Time.deltaTime;
            rotatingObject.transform.Rotate(Vector3.up, rotationAmount);
        }


        void OnEnable()
        {
            GameManager.platformingPhaseBegin+=Activate;
			GameManager.gamePaused+=Deactivate;
			GameManager.gamePaused+=Reactivate;
            GameManager.platformingPhaseFinished+=Reset;
        }

        void OnDisable()
        {
            GameManager.platformingPhaseBegin-=Activate;
			GameManager.gamePaused-=Deactivate;
			GameManager.gamePaused-=Reactivate;
            GameManager.platformingPhaseFinished-=Reset;
            // DOTween.Kill(transform, false);
        }

        private void Activate()
        {
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
			active=false;
            // DOTween.Rewind(transform);
            // DOTween.Kill(transform, true);
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
}