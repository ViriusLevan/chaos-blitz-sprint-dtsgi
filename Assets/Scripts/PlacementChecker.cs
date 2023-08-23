using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementChecker : MonoBehaviour
{
    private Placable.PlacableType placableType;
    [SerializeField] private PlacementManager manager;
    public void SetPlacementManager(PlacementManager pm) => manager = pm;

    public void SetPlacableType(Placable.PlacableType pType){
        placableType = pType;
    }

    private bool withinBounds=false;

    private void OnTriggerEnter(Collider other) 
    {
        Debug.Log("enter "+other.gameObject.name);
        if(other.gameObject.CompareTag("Arena")){
            withinBounds=true;
            if(placableType!=Placable.PlacableType.Hazard){
                manager.ValidPlacement();
            }
        }else if(placableType!=Placable.PlacableType.Hazard){
            manager.InvalidPlacement();
        }else{
            if(other.gameObject.layer == LayerMask.NameToLayer("Platform")){
                PointSlicer pSlicer = other.gameObject.GetComponentInParent<PointSlicer>() 
                    ?? other.gameObject.GetComponentInChildren<PointSlicer>();
                manager.ValidPlacement(pSlicer);
            }
            else
            {
                manager.InvalidPlacement();
            }
        }
    }

    private void OnTriggerStay(Collider other) 
    {
        Debug.Log("stay "+other.gameObject.name);
        if(other.gameObject.CompareTag("Arena")){
            withinBounds=true;
        }else if(placableType!=Placable.PlacableType.Hazard){
            manager.InvalidPlacement();
        }else{
            if(other.gameObject.layer == LayerMask.NameToLayer("Platform")){
                PointSlicer pSlicer = other.gameObject.GetComponentInParent<PointSlicer>() 
                    ?? other.gameObject.GetComponentInChildren<PointSlicer>();
                
                if(withinBounds)
                    manager.ValidPlacement(pSlicer);
            }
            else
            {
                manager.InvalidPlacement();
            }
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        Debug.Log("exit "+other.gameObject.name);
        if(other.gameObject.CompareTag("Arena")){
            withinBounds=false;
            manager.InvalidPlacement();
        }else if(placableType!=Placable.PlacableType.Hazard){
            if(withinBounds)
                manager.ValidPlacement();
        }
        else
        {
            manager.InvalidPlacement();
        }
    }
}
