using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallEffect : MonoBehaviour
{
    public GameObject thisBall;
    public Collider detector;
    public Position_Rotation_Effect PRF;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Box"))
        {
            //Debug.Log(thisBall.transform.rotation);
            VisualFX_Controller.instance.isItemPlaced = true;
            Vector3 newPosition = transform.position;
            newPosition.y = -3f;
            Vector3 rotationBall = thisBall.transform.rotation.eulerAngles;
            VisualFX_Controller.instance.PlayEffect(EffectEnum.PlaceEffect, newPosition, rotationBall);

        }
    }

}
