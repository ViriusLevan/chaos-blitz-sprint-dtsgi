using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

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
        playerInputHandler = GetComponent<PlayerInputHandler>();
    }    

    [SerializeField]private PointSlicer targetPlatform;
    public void InvalidPlacement(){canPlace=false;}
    public void ValidPlacement(PointSlicer pSlicer = null){
        canPlace=true;
        targetPlatform = pSlicer;
    }

    void Update()
    {
        if(playerInputHandler.playerInstance.playerStatus != PlayerInstance.PlayerStatus.Building)
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
    private void FixedUpdate() 
    {
        if(playerInputHandler.playerInstance.playerStatus 
			!= PlayerInstance.PlayerStatus.Building)return;
        if(pendingObj!=null){
            if(currentType == Placable.PlacableType.Hazard){
                RaycastHit hit;
                if(Physics.Raycast(cameraTransform.position
                    , cameraTransform.forward, out hit, 20f, platformLayer))
                {
                    pos = hit.point;
                    //Debug.Log(hit.transform.name+" :"+hit.point);
                    
                    // Debug.DrawLine(cameraTransform.position
                    //     , hit.point
                    //     , Color.red, 5.0f);
                }
                else{
                    pos = referenceTransform.position;
                }
            }
            else{
                pos = referenceTransform.position;
            }
        }
    }

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
        if(verticalInput<0 & referenceTransform.position.y>0){
            referenceTransform.position -= new Vector3(0,3f*Time.deltaTime,0);
        }
        if(verticalInput>0 & referenceTransform.position.y<10){
            referenceTransform.position += new Vector3(0,3f*Time.deltaTime,0);
        }
    }

    private void RotatePlacable(){
        float rotateInput 
            = playerInputHandler.playerConfig.input
                .actions["Move"].ReadValue<float>();
        if(rotateInput<0){
            pendingObj.transform.Rotate(0f, -50.0f*Time.deltaTime, 0.0f, Space.World);
        }
        if(rotateInput>0){
            pendingObj.transform.Rotate(0f, 50.0f*Time.deltaTime, 0.0f, Space.World);
        }
    }

    private Placable.PlacableType currentType;
    public void InstantiateNewPlacable(Placable placable)
    {
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
    }

    public void PlaceObject(InputAction.CallbackContext context)
    {
        if(!canPlace) return;
        if(pendingObj==null) return;

        int materialIndex=2;
        switch(currentType){
            case Placable.PlacableType.Platform:materialIndex=2;break;
            case Placable.PlacableType.Obstacle:materialIndex=3;break;
            case Placable.PlacableType.Hazard:materialIndex=4;break;
        }

        MeshRenderer[] mrs = pendingObj.GetComponentsInChildren<MeshRenderer>();
        foreach (var item in mrs)
        {
            item.material = materials[materialIndex];
        }
        
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

        //TODO set cross section material to something else
        if(targetPlatform!=null){
            targetPlatform.SetCrossSectionMaterial(materials[materialIndex]);
            targetPlatform.SetSliceTarget(pendingObj);
            targetPlatform.Slice();
            targetPlatform=null;
        }

        pendingObj = null;
    }

    private void UpdateMaterials()
    {
        if(pendingObj==null) return;
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
