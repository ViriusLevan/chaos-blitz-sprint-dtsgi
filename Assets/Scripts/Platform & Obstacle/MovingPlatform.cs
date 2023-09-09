using UnityEngine;
using DG.Tweening;

namespace LevelUpStudio.ChaosBlitzSprint.PlaceableBehaviour
{
    public class MovingPlatform : MonoBehaviour
    {
        public enum MovementDirection
        {
            Horizontal,
            Vertical
        }

        public MovementDirection dir;
        private Vector3 initialPosition;
        [SerializeField] private float distance;
        [SerializeField] private float speed;

        private void Start()
        {
            initialPosition = transform.localPosition;
        }

        private bool active=false;

        private void Update() 
        {
            if(active)
                MovePos();
        }
        
        private void OnEnable() 
        {
            GameManager.platformingPhaseBegin+=Activate;
			GameManager.gamePaused+=Deactivate;
			GameManager.gameUnpaused+=Reactivate;
            GameManager.platformingPhaseFinished+=Reset;
        }

        private void OnDisable() 
        {
            GameManager.platformingPhaseBegin-=Activate;
			GameManager.gamePaused-=Deactivate;
			GameManager.gameUnpaused-=Reactivate;
            GameManager.platformingPhaseFinished-=Reset;
            //DOTween.Kill(transform, false);
        }

        private void OnDestroy() 
        {
            GameManager.platformingPhaseBegin-=Activate;
            GameManager.platformingPhaseFinished-=Reset;
            //DOTween.Kill(transform, false);
        }

        private void Activate()
        {
            active=true;
        }

        private void Deactivate()
        {
            active=false;
            //DOTween.Pause(transform);
        }
		private void Reactivate()
		{
			active=true;
		}
        
        private void Reset()
        {
            //DOTween.Rewind(transform);
            //DOTween.Kill(transform, true);
            active=false;
            //transform.position = originalPosition;
        }

        private void MovePos()
        {
            // Calculate the movement direction based on the local rotation
            Vector3 movementDirection = dir == MovementDirection.Horizontal 
                ? transform.TransformDirection(Vector3.right) : transform.TransformDirection(Vector3.up);

            // Calculate the normalized time using the custom easing function
            float easedTime = CustomEaseInOutSine(Mathf.PingPong(Time.time * speed / distance, 1f));

            // Calculate the target position based on the initial position and the movement direction
            Vector3 targetPosition = initialPosition + movementDirection * easedTime * distance;

            // Move the platform to the target position
            transform.localPosition = targetPosition;
        }

        private float CustomEaseInOutSine(float t)
        {
            return -0.5f * (Mathf.Cos(Mathf.PI * t) - 1f);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.transform.parent.SetParent(transform);
                other.rigidbody.interpolation = RigidbodyInterpolation.None;
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.transform.parent.SetParent(null);
                other.rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            }
        }
    }
}