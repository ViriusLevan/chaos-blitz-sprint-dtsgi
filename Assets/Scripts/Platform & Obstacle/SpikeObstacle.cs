using UnityEngine;

namespace LevelUpStudio.ChaosBlitzSprint.PlaceableBehaviour
{
    public class SpikeObstacle : MonoBehaviour
    {
        [SerializeField] private float timeThreshold = 2f; // Time threshold before spikes are triggered
        [SerializeField] private float spikeUpAmount = 0.2f; // Amount to move spikes upwards
        [SerializeField] private float spikeDownDelay = 1f; // Delay before spikes move back down
        [SerializeField] private GameObject spike;

        private Vector3 originalSpikesPosition;
        private bool isPlayerOnPlatform;
        private bool isSpikesUp;
        private float elapsedTime;
        private float exitTime;

        private void Start()
        {
            originalSpikesPosition = spike.transform.localPosition; // Assuming the spikes are the first child
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                isPlayerOnPlatform = true;
                elapsedTime = 0f;
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                isPlayerOnPlatform = false;
                exitTime = Time.time;
            }
        }

        private void Update()
        {
            if (isPlayerOnPlatform)
            {
                elapsedTime += Time.deltaTime;

                if (elapsedTime >= timeThreshold && !isSpikesUp)
                {
                    MoveSpikesUp();
                }
            }
            else if (Time.time >= exitTime + spikeDownDelay && isSpikesUp)
            {
                MoveSpikesDown();
            }
        }

        private void MoveSpikesUp()
        {
            Vector3 newSpikesPosition = originalSpikesPosition + Vector3.up * spikeUpAmount;
            transform.GetChild(0).localPosition = newSpikesPosition;
            isSpikesUp = true;
            Debug.Log("Spikes triggered!");
            // You can add code here to kill the player, like calling a player's death function
        }

        private void MoveSpikesDown()
        {
            transform.GetChild(0).localPosition = originalSpikesPosition;
            isSpikesUp = false;
        }
    }
}