using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private const float MIN_FOLLOW_Y_OFFSET = 0F;
    private const float MAX_FOLLOW_Y_OFFSET = 3F;
    private const float MIN_FOLLOW_Z_OFFSET = -10F;
    private const float MAX_FOLLOW_Z_OFFSET = -4F;


    [SerializeField]private CinemachineVirtualCamera virtCam;
    [SerializeField]private float moveSpeed = 10f;
    [SerializeField]private float rotationSpeed = 100f;
    [SerializeField]private float zoomAmount =1f;

    private CinemachineTransposer cineTransposer;
    private Vector3 targetFollowOffset;
    private PlayerInput playerInput;

    public static CameraController Instance;
    private void Awake() {
        if(Instance!=null)Destroy(this.gameObject);
        Instance=this;
        DontDestroyOnLoad(this);
    }

    private bool buildingMode=true;
    [SerializeField]private GameObject cameraController, playerObject;

    public void ShiftToPlayer(){
        buildingMode=false;
        virtCam.Follow = cameraController.transform;
    }

    public void EnableBuilding(){
        buildingMode=true;
        virtCam.Follow = playerObject.transform;
    }
    
    private void Start() 
    {
        playerInput = GetComponent<PlayerInput>();
        cineTransposer = virtCam.GetCinemachineComponent<CinemachineTransposer>();
        targetFollowOffset = cineTransposer.m_FollowOffset;
    }

    private void Update() 
    {
        if(buildingMode){
            HandleMovement();
            HandleRotation();
            //HandleZoom();
        }
    }

    Vector2 lookInput;
    private void HandleMovement()
    {
        lookInput = playerInput.actions["CameraLook"].ReadValue<Vector2>();
        lookInput = Vector2.ClampMagnitude(lookInput, 1f);

        Vector3 moveVector = (transform.forward * lookInput.y) + (transform.right*lookInput.x);
        transform.position += moveVector * moveSpeed * Time.deltaTime;
    }

    private void HandleRotation()
    {
        Vector3 rotationVector = new Vector3(0
            ,playerInput.actions["CameraRotate"].ReadValue<float>()
            ,0);
        
        transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;
    }

    private void HandleZoom()
    {
        float zoomInput = playerInput.actions["Zoom"].ReadValue<float>();
        if (zoomInput>0)
        {
            targetFollowOffset.y -= zoomAmount;
            targetFollowOffset.z += zoomAmount*2f;
        }
        else if(zoomInput<0)
        {
            targetFollowOffset.y += zoomAmount;
            targetFollowOffset.z -= zoomAmount*2f;
        }

        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);
        targetFollowOffset.z = Mathf.Clamp(targetFollowOffset.z, MIN_FOLLOW_Z_OFFSET, MAX_FOLLOW_Z_OFFSET);
        cineTransposer.m_FollowOffset 
            = Vector3.Lerp(cineTransposer.m_FollowOffset, targetFollowOffset, Time.deltaTime*zoomAmount);
    }
}
