using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementChecker : MonoBehaviour
{
    private Placable.PlacableType placableType;
    public delegate void OnPlacementCheckerEvent();
    public static event OnPlacementCheckerEvent invalidPlacement,validPlacement;

    public void SetPlacableType(Placable.PlacableType pType){
        placableType = pType;
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(placableType!=Placable.PlacableType.Hazard){
            invalidPlacement.Invoke();
        }else{
            if(other.gameObject.layer == LayerMask.NameToLayer("Platform")){
                validPlacement.Invoke();
            }
            else
            {
                invalidPlacement.Invoke();
            }
        }
    }

    private void OnTriggerStay(Collider other) 
    {
        if(placableType!=Placable.PlacableType.Hazard){
            invalidPlacement.Invoke();
        }else{
            if(other.gameObject.layer == LayerMask.NameToLayer("Platform")){
                validPlacement.Invoke();
            }
            else
            {
                invalidPlacement.Invoke();
            }
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if(placableType!=Placable.PlacableType.Hazard){
            validPlacement.Invoke();
        }
        else
        {
            invalidPlacement.Invoke();
        }
    }
}
