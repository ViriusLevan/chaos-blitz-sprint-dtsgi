using UnityEngine;

namespace LevelUpStudio.ChaosBlitzSprint.Placement
{
    public class PlacementChecker : MonoBehaviour
    {
        private Placable.PlacableType placableType;
        [SerializeField] private Player.PlacementManager manager;
        public void SetPlacementManager(Player.PlacementManager pm) => manager = pm;

        public void SetPlacableType(Placable.PlacableType pType){
            placableType = pType;
        }

        private bool withinBounds=false;

        private void OnTriggerEnter(Collider other) 
        {
            if(other.gameObject.layer == LayerMask.NameToLayer("IgnorePlacementCheck"))
                return;
            if(other.gameObject.CompareTag("PowerUp")){//do nothing
                return;
            }
            //Debug.Log("enter "+other.gameObject.name);
            if(other.gameObject.CompareTag("Arena")){
                withinBounds=true;
                if(placableType!=Placable.PlacableType.Hazard){
                    manager.ValidPlacement();
                }
            }else if(placableType!=Placable.PlacableType.Hazard){
                manager.InvalidPlacement();
            }else{
                if(other.gameObject.layer == LayerMask.NameToLayer("Platform")){
                    // PointSlicer pSlicer = other.gameObject.GetComponentInParent<PointSlicer>() 
                    //     ?? other.gameObject.GetComponentInChildren<PointSlicer>();
                                            
                    //manager.ValidPlacement();
                }
                else
                {
                    manager.InvalidPlacement();
                }
            }
        }

        private void OnTriggerStay(Collider other) 
        {
            if(other.gameObject.layer == LayerMask.NameToLayer("IgnorePlacementCheck"))
                return;
            if(other.gameObject.CompareTag("PowerUp")){//do nothing
                return;
            }
            //Debug.Log("stay "+other.gameObject.name);
            if(other.gameObject.CompareTag("Arena")){
                withinBounds=true;
            }else if(placableType!=Placable.PlacableType.Hazard){
                manager.InvalidPlacement();
            }else{
                if(other.gameObject.layer == LayerMask.NameToLayer("Platform")){
                    // PointSlicer pSlicer = other.gameObject.GetComponentInParent<PointSlicer>() 
                    //     ?? other.gameObject.GetComponentInChildren<PointSlicer>();
                    
                    // if(withinBounds)
                    //     manager.ValidPlacement();
                }
                else
                {
                    manager.InvalidPlacement();
                }
            }
        }

        private void OnTriggerExit(Collider other) 
        {
            if(other.gameObject.layer == LayerMask.NameToLayer("IgnorePlacementCheck"))
                return;
            if(other.gameObject.CompareTag("PowerUp")){//do nothing
                return;
            }
            //Debug.Log("exit "+other.gameObject.name);
            if(other.gameObject.CompareTag("Arena")){
                withinBounds=false;
                manager.InvalidPlacement();
            }else if(placableType!=Placable.PlacableType.Hazard){
                if(withinBounds)
                    manager.ValidPlacement();
            }
            else
            {
                //manager.InvalidPlacement();
            }
        }

        private void SetPos(Vector3 newPos)
        {
            manager.SetPos(newPos);
            manager.GetReferenceTransform().position=newPos;
        }

        Vector3 FindHighestVert(MeshFilter targetMesh)
        {
            var maxBounds = targetMesh.sharedMesh.bounds.max;

            Matrix4x4 localToWorld = targetMesh.transform.localToWorldMatrix;

            Vector3 hi = localToWorld.MultiplyPoint3x4(maxBounds);

            return hi;
        }
    }
}
