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
            Vector3 bounceDirection = transform.TransformDirection(Vector3.up); // Get local up direction of trampoline
            otherRb.AddForce(bounceDirection * bounceForce, ForceMode.Impulse);
        }
    }
}
