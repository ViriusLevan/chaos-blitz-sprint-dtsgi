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

    private void OnTriggerEnter(Collider other) 
    {
        if(placableType!=Placable.PlacableType.Hazard){
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
        if(placableType!=Placable.PlacableType.Hazard){
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

    private void OnTriggerExit(Collider other) 
    {
        if(placableType!=Placable.PlacableType.Hazard){
            manager.ValidPlacement();
        }
        else
        {
            manager.InvalidPlacement();
        }
    }
}
