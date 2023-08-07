using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlacementManager : MonoBehaviour
{
    public Placable[] placables;
    private Vector3 pos;
    private RaycastHit hit;
    //can place, cannot place, Platform,Obstacle,Hazard
    [SerializeField] private Material[] materials;
    

    public bool canPlace = true;
    private GameObject pendingObj;
    [SerializeField]private Transform referenceTransform;
    [SerializeField]private PlayerInput playerInput;
    [SerializeField]private Transform cameraTransform;
    [SerializeField]private TextMeshProUGUI placableNameText;

    void Start()
    {
    }    

    private void OnEnable() {
        playerInput.actions["Build"].performed+= SelectObject;
        playerInput.actions["Place"].performed+= PlaceObject;
        playerInput.actions["CycleSelectionForward"].performed+= CycleIndexForward;
        playerInput.actions["CycleSelectionBackward"].performed+= CycleIndexBackward;
        playerInput.actions["ShiftPlayer"].performed+= ShiftToPlayer;
        PlacementChecker.invalidPlacement +=InvalidPlacement;
        PlacementChecker.validPlacement +=ValidPlacement;
    }

    private void OnDisable() {
        // playerInput.actions["Build"].performed-= SelectObject;
        // playerInput.actions["Place"].performed-= PlaceObject;
        // playerInput.actions["CycleForward"].performed-= CycleIndexForward;
        // playerInput.actions["CycleBackward"].performed-= CycleIndexBackward;
        PlacementChecker.invalidPlacement -= InvalidPlacement;
        PlacementChecker.validPlacement -= ValidPlacement;
    }

    public GameObject playerObject;
    private void ShiftToPlayer(InputAction.CallbackContext context)
    {
        pendingObj = null;
        playerInput.actions["Build"].performed-= SelectObject;
        playerInput.actions["Place"].performed-= PlaceObject;
        playerInput.actions["CycleSelectionForward"].performed-= CycleIndexForward;
        playerInput.actions["CycleSelectionBackward"].performed-= CycleIndexBackward;
        playerInput.actions["ShiftPlayer"].performed -= ShiftToPlayer;
        CameraController.Instance.ShiftToPlayer();
        playerObject.SetActive(true);
    }

    private void InvalidPlacement(){canPlace=false;}
    private void ValidPlacement(){canPlace=true;}

    void Update()
    {
        //if(_GameManager.Instance.GetPaused()){return;}
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

    private void VerticalMotion(){
        float verticalInput = playerInput.actions["MoveVertically"].ReadValue<float>();
        if(verticalInput<0 & referenceTransform.position.y>0){
            referenceTransform.position -= new Vector3(0,3f*Time.deltaTime,0);
        }
        if(verticalInput>0 & referenceTransform.position.y<10){
            referenceTransform.position += new Vector3(0,3f*Time.deltaTime,0);
        }
    }

    private void RotatePlacable(){
        float rotateInput = playerInput.actions["RotatePlacable"].ReadValue<float>();
        if(rotateInput<0){
            pendingObj.transform.Rotate(0f, -50.0f*Time.deltaTime, 0.0f, Space.World);
        }
        if(rotateInput>0){
            pendingObj.transform.Rotate(0f, 50.0f*Time.deltaTime, 0.0f, Space.World);
        }
    }
    
    private int placableIndex;

    private void CycleIndexForward(InputAction.CallbackContext context){
        placableIndex+=1;
        if(placableIndex>=placables.Length){
            placableIndex=0;
        }
        if(pendingObj!=null){
            Destroy(pendingObj);
        }
        InstantiateNewObject();
    }
    private void CycleIndexBackward(InputAction.CallbackContext context){
        placableIndex-=1;
        if(placableIndex<0){
            placableIndex=placables.Length-1;
        }
        if(pendingObj!=null){
            Destroy(pendingObj);
        }
        InstantiateNewObject();
    }

    private Placable.PlacableType currentType;
    private void SelectObject(InputAction.CallbackContext context)
    {
        if(pendingObj!=null){
            Destroy(pendingObj);
        }
        InstantiateNewObject();
    }

    private void InstantiateNewObject()
    {
        pendingObj = Instantiate(placables[placableIndex].GetPrefab(), pos
                        , referenceTransform.rotation);
        MonoBehaviour[] mbs = pendingObj.GetComponentsInChildren<MonoBehaviour>();
        foreach (var item in mbs)
        {
            item.enabled=false;
        }
        PlacementChecker pChecker = pendingObj.AddComponent<PlacementChecker>();
        currentType = placables[placableIndex].GetPlacableType();
        canPlace = (currentType==Placable.PlacableType.Hazard) ? false : true;
        pChecker.SetPlacableType(currentType);
        placableNameText.text = placables[placableIndex].name;
        if(placables[placableIndex].name.Contains("Log")){
            pendingObj.transform.eulerAngles += new Vector3(0,0,90);
        }
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
