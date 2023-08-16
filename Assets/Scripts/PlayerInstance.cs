using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerInstance : MonoBehaviour
{
    public enum PlayerStatus {Awaiting, Picking, FinishedPicking
        , Building, FinishedBuilding, Platforming
        , FinishedPlatforming, Dead};


    [SerializeField]private CinemachineFreeLook freeLookCamera;
	[SerializeField]public GameObject buildCameraFollow{get;private set;}
    [SerializeField]public PlayerStatus playerStatus {get; private set;}
    [SerializeField]public int playerScore {get; private set;}
    [SerializeField]public GameObject playerCamera {get; private set;}
    
	private PlacementManager placementManager;
    private PlayerController playerController;
    private PlayerInputHandler playerInputHandler;
	public CinemachineInputHandler cinemachineInputHanlder{get; private set;}

	private void Awake() {
		cinemachineInputHanlder = GetComponentInChildren<CinemachineInputHandler>();
	}

    void Start()
    {
		placementManager = GetComponentInChildren<PlacementManager>();
        playerController = GetComponentInChildren<PlayerController>();
        playerInputHandler = GetComponentInChildren<PlayerInputHandler>();
		placementManager?.SetReferenceTransform(buildCameraFollow.transform);
		playerStatus = PlayerStatus.Picking;
    }

    private void Update() {
		
    }
	public void SetBuildCameraFollow(GameObject followTarget){buildCameraFollow = followTarget;}   
    public void SetPlayerStatus(PlayerStatus ps){playerStatus = ps;}
    public void SetPlayerScore(int ps){playerScore=ps;}
    public void AddPlayerScore(int addition) => playerScore+=addition;
	

    public void BuildingMode(Placable placable)
    {
		playerStatus = PlayerStatus.Building;
		freeLookCamera.Follow = buildCameraFollow.transform;
		freeLookCamera.LookAt = buildCameraFollow.transform;
		placementManager.SetCameraTransform(freeLookCamera.transform);
		placementManager.InstantiateNewPlacable(placable);
		playerInputHandler.playerConfig.input.SwitchCurrentActionMap("BuildMode");
		cinemachineInputHanlder.horizontal 
			= playerInputHandler.playerConfig.input.actions.FindAction("Look");
    }

	public void PlatformingMode(){
		playerStatus = PlayerStatus.Platforming;
		freeLookCamera.Follow = transform;
		freeLookCamera.LookAt = transform;
		playerInputHandler.playerConfig.input.SwitchCurrentActionMap("Player");
		cinemachineInputHanlder.horizontal 
			= playerInputHandler.playerConfig.input.actions.FindAction("Look");
	}

}
