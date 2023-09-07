using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using LevelUpStudio.ChaosBlitzSprint.Placement;

namespace LevelUpStudio.ChaosBlitzSprint.Player
{
    public class PlacementManager : MonoBehaviour
    {
        private Vector3 pos;
        private RaycastHit hit;
        //can place, cannot place, Platform,Obstacle,Hazard
        [SerializeField] private Material[] materials;

        public bool canPlace = false;
        private GameObject pendingObj;
        [SerializeField]private Transform referenceTransform;
        [SerializeField]private Transform cameraTransform;
        [SerializeField]private TextMeshProUGUI placableNameText;

        [SerializeField]private PlayerInputHandler playerInputHandler;

        public void SetReferenceTransform(Transform t)=> referenceTransform = t;
        public void SetCameraTransform(Transform t)=> cameraTransform=t;


        void Start()
        {
            pendingObjMaterials =  new List<Material>();
            playerInputHandler = GetComponent<PlayerInputHandler>();
        }    

        [SerializeField]private PointSlicer targetPlatform;
        public void InvalidPlacement()
        {
            canPlace=false;
            targetPlatform=null;
        }
        public void ValidPlacement(PointSlicer pSlicer = null){
            canPlace=true;
            targetPlatform = pSlicer;
        }

        void Update()
        {

            if(playerInputHandler.playerInstance.playerStatus != PlayerInstance.PlayerStatus.Building
                && playerInputHandler.playerInstance.playerStatus != PlayerInstance.PlayerStatus.FinishedBuilding)
                return;

            CameraMovement();

            if(pendingObj!=null)
            {
                VerticalMotion();
                RotatePlacable();
                pendingObj.transform.position = pos;
                UpdateMaterials();
            }
        }
        
        [SerializeField] private LayerMask platformLayer;
        [SerializeField] private GameObject arrowIndicator;
        public GameObject GetArrowIndicator(){return arrowIndicator;}
        private void FixedUpdate() 
        {
            if(playerInputHandler.playerInstance.playerStatus 
                != PlayerInstance.PlayerStatus.Building)return;
            if(pendingObj!=null){
                if(currentType == Placable.PlacableType.Hazard){
                    RaycastHit hit;
                    // Debug.DrawLine(cameraTransform.position
                    //         , cameraTransform.forward*20 + cameraTransform.position
                    //         , Color.red, 5.0f);
                    if(Physics.Raycast(cameraTransform.position
                        , cameraTransform.forward, out hit, 20f, platformLayer))
                    {
                        pos = hit.point;
                        //Debug.Log(hit.transform.name+" :"+hit.point);
                        PointSlicer pSlicer = 
                            hit.rigidbody.gameObject.GetComponentInParent<PointSlicer>() 
                            ??  hit.rigidbody.gameObject.GetComponentInChildren<PointSlicer>();
                        Debug.Log(pSlicer.ToString());
                        targetPlatform=pSlicer;
                        ValidPlacement(pSlicer);
                        arrowIndicator.SetActive(true);
                        arrowIndicator.transform.position 
                            = new Vector3(
                                pSlicer.gameObject.transform.position.x
                                ,hit.point.y + 2
                                ,pSlicer.gameObject.transform.position.z
                            );
                    }
                    else
                    {
                        InvalidPlacement();
                        pos = referenceTransform.position;
                        arrowIndicator.SetActive(false);
                    }
                }
                else{
                    pos = referenceTransform.position;
                    arrowIndicator.SetActive(false);
                }
            }
        }

        public Vector3 GetPos(){return pos;}
        public void SetPos(Vector3 newPos)
        {
            pos=newPos;
        }

        public Transform GetReferenceTransform(){return referenceTransform;}

        private Vector3 moveDir;
        private Vector2 moveInput;
        private void CameraMovement(){
            moveInput = playerInputHandler.playerConfig.input.actions["Move"].ReadValue<Vector2>();
            float h = moveInput.x;
            float v = moveInput.y;
            Vector3 v2 = v * playerInputHandler.playerInstance.playerCamera.transform.forward; //Vertical axis to which I want to move with respect to the camera
            Vector3 h2 = h * playerInputHandler.playerInstance.playerCamera.transform.right; //Horizontal axis to which I want to move with respect to the camera
            moveDir = (v2 + h2).normalized; //Global position to which I want to move in magnitude 1
            moveDir.y=0;
            playerInputHandler.playerInstance.buildCameraFollow.transform.position += moveDir * 10f * Time.deltaTime;
        }

        private void VerticalMotion(){
            float verticalInput 
                = playerInputHandler.playerConfig.input
                    .actions["VerticalMotion"].ReadValue<float>();
            if(verticalInput<0){
                referenceTransform.position -= new Vector3(0,3f*Time.deltaTime,0);
            }
            if(verticalInput>0){
                referenceTransform.position += new Vector3(0,3f*Time.deltaTime,0);
            }
        }

        private void RotatePlacable(){
            float rotateInput 
                = playerInputHandler.playerConfig.input
                    .actions["Rotate"].ReadValue<float>();
            if(rotateInput<0){
                pendingObj.transform.Rotate(0f, -50.0f*Time.deltaTime, 0.0f, Space.World);
            }
            if(rotateInput>0){
                pendingObj.transform.Rotate(0f, 50.0f*Time.deltaTime, 0.0f, Space.World);
            }
        }

        private Placable.PlacableType currentType;
        [SerializeField]public Placable placable{get;private set;}
        public void SetPlacable(Placable pl){placable=pl;}
        public void InstantiateNewPlacable()
        {
            if(placable==null){
                Debug.Log("NULL PLACABLE");
                return;
            }

            targetPlatform=null;
            pendingObj = Instantiate(placable.GetPrefab(), pos
                            , referenceTransform.rotation);
            MonoBehaviour[] mbs = pendingObj.GetComponentsInChildren<MonoBehaviour>();
            foreach (var item in mbs)
            {
                item.enabled=false;
            }
            PlacementChecker pChecker = pendingObj.AddComponent<PlacementChecker>();
            pChecker.SetPlacementManager(this);
            
            currentType = placable.GetPlacableType();
            canPlace = currentType!=Placable.PlacableType.Hazard;
            pChecker.SetPlacableType(currentType);
            //placableNameText.text = placables[placableIndex].name;

            MeshRenderer[] mrs = pendingObj.GetComponentsInChildren<MeshRenderer>();
            foreach (var item in mrs)
            {
                pendingObjMaterials.Add(item.material);
            }

            if(currentType==Placable.PlacableType.Hazard){
                pendingObj.transform.eulerAngles += new Vector3(-90,0,0);
            }
        }
        private List<Material> pendingObjMaterials;

        public void PlaceObject(InputAction.CallbackContext context)
        {
            if(!canPlace) return;
            if(pendingObj==null) return;

            MeshRenderer[] mrs = pendingObj.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < mrs.Length; i++)
            {
                mrs[i].material = pendingObjMaterials[i];
            }
            pendingObjMaterials.Clear();
            
            PlacementChecker toDestroy = pendingObj.GetComponent<PlacementChecker>();
            Destroy(toDestroy);

            MonoBehaviour[] mbs = pendingObj.GetComponentsInChildren<MonoBehaviour>();
            foreach (var item in mbs)
            {
                item.enabled=true;
            }

            Collider[] colls = pendingObj.GetComponentsInChildren<Collider>();
            foreach (Collider coll in colls)
            {
                coll.isTrigger = false;
            }

            if(placable.GetPlacableType()==Placable.PlacableType.Platform)
            {
                SetLayerAllChildren(pendingObj.transform, LayerMask.NameToLayer("Platform") );
            }

            //TODO set cross section material to something else
            if(targetPlatform!=null){
                targetPlatform.SetCrossSectionMaterial(placable.GetCrossSectionMaterial());
                targetPlatform.SetSliceTarget(pendingObj);
                targetPlatform.Slice(targetPlatform.gameObject);
                targetPlatform=null;
            }

            SoundManager.Instance?.PlaySound(SoundEnum.PoofSound);
            VFXManager.Instance?.PlayEffect(VFXEnum.PlaceEffect
                , pendingObj.transform.position
                , new Vector3());

            pendingObj = null;
            arrowIndicator.SetActive(false);
            GameManager.Instance.PlayerBuilt(playerInputHandler.playerConfig.playerIndex);
            Debug.Log($"Player {playerInputHandler.playerConfig.playerIndex} finished placing");
        }

        private void SetLayerAllChildren(Transform root, int layer)
        {
            var children = root.GetComponentsInChildren<Transform>(includeInactive: true);
            foreach (var child in children)
            {
    //            Debug.Log(child.name);
                child.gameObject.layer = layer;
            }
        }

        private void UpdateMaterials()
        {
            if(canPlace)
            {
                MeshRenderer[] mrs = pendingObj.GetComponentsInChildren<MeshRenderer>();
                foreach (var item in mrs)
                {
                    item.material = materials[0];
                }
            }
            else
            {
                MeshRenderer[] mrs = pendingObj.GetComponentsInChildren<MeshRenderer>();
                foreach (var item in mrs)
                {
                    item.material = materials[1];
                }
            }
        }
    }
}
