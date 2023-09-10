using System.Collections;
using UnityEngine;

namespace LevelUpStudio.ChaosBlitzSprint.PlaceableBehaviour
{
    public class ShootingObstacle : MonoBehaviour
    {
        [SerializeField] private float shootingSpeed;
        [SerializeField] private float shootyLifetime;
        [SerializeField] private float shotInterval;
        [SerializeField] private ObjectPooler objectPooler;

        private float intervalCounter = 0f;
        
        [SerializeField] private Animator crossbowAnimator;

        private void Update()
        {
            // Update the shoot timer
            intervalCounter += Time.deltaTime;

            // If the shoot interval has passed, shoot an object and reset the timer
            if (intervalCounter >= shotInterval)
            {
                if(isCannon){
                    Shoot();
                }else{
                    crossbowAnimator.SetTrigger("Firing");
                }
            }
        }

        [SerializeField]private Transform spawnPoint;
        [SerializeField]private bool isCannon=false;
        public void Shoot()
        {
            intervalCounter = 0f;
            Vector3 shootDirection = isCannon ? 
                (transform.forward+(Vector3.up * 0.25f)) : transform.forward ;

            GameObject shooty = objectPooler.GetPooled();

            if (shooty != null)
            {
                if(isCannon)
                    VFXManager.Instance?.PlayEffect(VFXEnum.CannonEffect
                        , spawnPoint.transform.position
                        , new Vector3());
                else
                    VFXManager.Instance?.PlayEffect(VFXEnum.ArrowEffect
                        , spawnPoint.transform.position
                        , new Vector3());
                shooty.transform.position = spawnPoint.position;
                shooty.transform.rotation = spawnPoint.rotation;
                shooty.SetActive(true);
                Rigidbody rb = shooty.GetComponent<Rigidbody>();
                rb.constraints = RigidbodyConstraints.None;
                rb.freezeRotation=false;
                rb.velocity = Vector3.zero;
                rb.AddForce(shootDirection.normalized * shootingSpeed, ForceMode.Impulse);

                StartCoroutine(deactivateShooty(shooty));
            }
        }

        private IEnumerator deactivateShooty(GameObject gameObject)
        {
            yield return new WaitForSeconds(shootyLifetime);
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
            gameObject.SetActive(false);
        }
    }
}