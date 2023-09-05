using System.Collections;
using UnityEngine;

namespace LevelUpStudio.ChaosBlitzSprint.PlaceableBehaviour
{
    public class ShootingObstacle : MonoBehaviour
    {
        [SerializeField] private float shootingSpeed;
        [SerializeField] private float shootyLifetime;
        [SerializeField] private float shootInterval;
        [SerializeField] private ObjectPooler objectPooler;

        private float shootTimer = 0f;
        private bool isShooting;

        private void Update()
        {
            // Update the shoot timer
            shootTimer += Time.deltaTime;

            // If the shoot interval has passed, shoot an object and reset the timer
            if (shootTimer >= shootInterval)
            {
                Shoot();
                shootTimer = 0f;
            }
        }

        private void Shoot()
        {
            Vector3 spawnPosition = transform.position;
            Vector3 shootDirection = transform.forward;

            GameObject shooty = objectPooler.GetPooled();

            if (shooty != null)
            {
                shooty.transform.position = spawnPosition;
                shooty.SetActive(true);
                Rigidbody rb = shooty.GetComponent<Rigidbody>();
                rb.velocity = Vector3.zero;
                rb.AddForce(shootDirection.normalized * shootingSpeed, ForceMode.Impulse);

                StartCoroutine(deactivateShooty(shooty));
            }
        }

        private IEnumerator deactivateShooty(GameObject gameObject)
        {
            yield return new WaitForSeconds(shootyLifetime);
            gameObject.SetActive(false);
        }
    }
}