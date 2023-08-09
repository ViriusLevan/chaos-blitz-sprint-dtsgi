using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceObstacle : MonoBehaviour
{
    [SerializeField] private float bounceForce;

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Player"))
        {
            Rigidbody otherRb = other.rigidbody;
            otherRb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);
        }
    }
}
