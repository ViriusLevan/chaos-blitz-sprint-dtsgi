using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
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
    public PlayerInputHandler playerInputHandler{get;private set;}
	public CinemachineInputHandler cinemachineInputHanlder{get; private set;}

	[SerializeField] RectTransform playerPanel;
	[SerializeField] TextMeshProUGUI playerText, scoreText;

	private void Awake() {
		cinemachineInputHanlder = GetComponentInChildren<CinemachineInputHandler>();
		placementManager = GetComponentInChildren<PlacementManager>();
        playerController = GetComponentInChildren<PlayerController>();
        playerInputHandler = GetComponentInChildren<PlayerInputHandler>();
		playerCamera = GetComponentInChildren<Camera>().gameObject;
	}

    void Start()
    {
		placementManager?.SetReferenceTransform(buildCameraFollow.transform);
		int playerIndex = playerInputHandler.playerConfig.playerIndex;
		playerText.text = "Player "+playerIndex;
		
		if(playerIndex==1 || playerIndex==3)
		{
			Debug.Log("PlayerIndex="+playerIndex);
			playerPanel.anchorMin = new Vector2(1, 1);
			playerPanel.anchorMax = new Vector2(1, 1);
			Vector2 shiftDirection 
				= new Vector2(0.5f - playerPanel.anchorMax.x, 0.5f - playerPanel.anchorMax.y);
        	playerPanel.anchoredPosition = shiftDirection * playerPanel.rect.size;    
		}
    }

	private void OnDestroy() 
	{
		
	}

    private void Update() {
		
    }

	public void SetBuildCameraFollow(GameObject followTarget){buildCameraFollow = followTarget;}   
    public void SetPlayerStatus(PlayerStatus ps){
		playerStatus = ps;
		switch (ps){
			case PlayerStatus.Picking:
				PickingMode();
				break;
			case PlayerStatus.FinishedPicking:
				playerInputHandler.virtualCursor.ResetMousePosition();
				playerInputHandler.virtualCursor.SetCursorTransparency(0);
				break;
			case PlayerStatus.Building:
				BuildingMode();
				if(placementManager.placable==null){
					playerStatus = PlayerStatus.FinishedBuilding;
				}
				break;
			case PlayerStatus.Platforming:
				PlatformingMode();
				break;
		}
	}
    public void SetPlayerScore(int ps){playerScore=ps;}
    public void AddPlayerScore(int addition) {
		playerScore+=addition;
		scoreText.text = "Score "+playerScore;
	}
	
	public void SetPlacable(Placable pl) => placementManager.SetPlacable(pl);

	public void PickingMode()
	{
		playerInputHandler.virtualCursor.SetCursorTransparency(255);
	    playerInputHandler.playerConfig.input.SwitchCurrentActionMap("UI"); 
	}

    public void BuildingMode()
    {
		buildCameraFollow.transform.position = GameManager.Instance.buildCamSetPoint.position;
		playerController.EnableMeshRenderer();
		playerController.transform.position 
			= GameManager.Instance.GetPlayerSpawnPoints()[
				playerInputHandler.playerConfig.playerIndex
				].position;
		freeLookCamera.Follow = buildCameraFollow.transform;
		freeLookCamera.LookAt = buildCameraFollow.transform;
		placementManager.SetCameraTransform(freeLookCamera.transform);
		placementManager.InstantiateNewPlacable();
		playerInputHandler.playerConfig.input.SwitchCurrentActionMap("BuildMode");
		cinemachineInputHanlder.horizontal 
			= playerInputHandler.playerConfig.input.actions.FindAction("Look");
    }

	public void PlatformingMode(){
		freeLookCamera.Follow = playerController.gameObject.transform;
		freeLookCamera.LookAt = playerController.gameObject.transform;
		playerInputHandler.playerConfig.input.SwitchCurrentActionMap("Player");
		cinemachineInputHanlder.horizontal 
			= playerInputHandler.playerConfig.input.actions.FindAction("Look");
	}

}