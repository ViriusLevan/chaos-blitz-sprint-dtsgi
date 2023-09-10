using System.Collections;
using Unity.VisualScripting;
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

        public enum RotationAxis{x,y,z};
        [SerializeField] private RotationAxis rotAxis=RotationAxis.y;

        private void Rotate()
        {
            Vector3 rotation = new Vector3();
            if(rotAxis==RotationAxis.x){
                rotation = Vector3.forward;
            }else if(rotAxis==RotationAxis.y){
                rotation = Vector3.up;
            }else if(rotAxis==RotationAxis.z){
                rotation = Vector3.right;
            }
            float rotationAmount = rotationSpeed * Time.deltaTime;
            rotatingObject.transform.Rotate(rotation, rotationAmount);
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
            if(rotAxis==RotationAxis.x){
            rotatingObject.transform.rotation 
                = Quaternion.Euler(0,transform.rotation.y, transform.rotation.z).normalized;
            }else if(rotAxis==RotationAxis.y){
                rotatingObject.transform.rotation 
                = Quaternion.Euler(transform.rotation.x,0, transform.rotation.z).normalized;
            }else if(rotAxis==RotationAxis.z){
                rotatingObject.transform.rotation 
                = Quaternion.Euler(transform.rotation.x,transform.rotation.y, 0).normalized;
            }
            // DOTween.Rewind(transform);
            // DOTween.Kill(transform, true);
            //transform.position = originalPosition;
        }
        private void OnCollisionEnter(Collision other)
        {
            if (other.collider.gameObject.CompareTag("Player"))
            {
                Debug.Log($"Player Collision Enter {other.gameObject.name}");
                other.gameObject.transform.parent.SetParent(rotatingObject.transform);
                other.rigidbody.interpolation = RigidbodyInterpolation.None;
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.collider.gameObject.CompareTag("Player"))
            {
                Debug.Log("Player Collision Exit");
                other.gameObject.transform.parent.SetParent(null);
                other.rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            }
        }
    }
}