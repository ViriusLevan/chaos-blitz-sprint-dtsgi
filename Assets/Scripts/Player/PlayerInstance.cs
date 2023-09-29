using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LevelUpStudio.ChaosBlitzSprint.Player{
	public class PlayerInstance : MonoBehaviour
	{
		public enum PlayerStatus {AwaitingAnimation, Picking, FinishedPicking
			, Building, FinishedBuilding, Platforming
			, FinishedPlatforming, Dead, Paused};

		[SerializeField]public CinemachineInputHandler cinemachineInputHandler{get; private set;}
		[SerializeField]private CinemachineVirtualCamera virtualCamera;
		[SerializeField]public GameObject buildCameraFollow;
		[SerializeField]public PlayerStatus playerStatus {get; private set;}
		[SerializeField]public int playerScore {get; private set;}
		[SerializeField]public GameObject playerCamera {get; private set;}
		private PlacementManager placementManager;
		private PlayerController playerController;
		public PlayerInputHandler playerInputHandler{get;private set;}

		[SerializeField] RectTransform playerPanel;
		[SerializeField] TextMeshProUGUI playerText, scoreText, controlHelpText;

		private void Awake() {
			placementManager = GetComponentInChildren<PlacementManager>();
			playerController = GetComponentInChildren<PlayerController>();
			playerInputHandler = GetComponentInChildren<PlayerInputHandler>();
			playerCamera = GetComponentInChildren<Camera>().gameObject;
			cinemachineInputHandler = virtualCamera.GetComponent<CinemachineInputHandler>();
			virtualCamera.transform.SetParent(null);
		}


		[SerializeField]private Camera uiCamera;
		[SerializeField]private RectTransform powerUpParent;
		void Start()
		{
			placementManager?.SetReferenceTransform(buildCameraFollow.transform);
			int playerIndex = playerInputHandler.playerConfig.playerIndex;
			playerText.text = "Player "+(playerIndex+1);
			scoreText.text = 0 + " / " + GameManager.Instance?.GetRoundType().GetPointRequirement();
			
			if(playerIndex==1 || playerIndex==3)
			{
				Debug.Log("PlayerIndex="+playerIndex);
				playerPanel.anchorMin = new Vector2(1, 1);
				playerPanel.anchorMax = new Vector2(1, 1);
				playerPanel.anchoredPosition = new Vector3(-400,-75,0);
				powerUpParent.anchorMin = new Vector2(1, 0);
				powerUpParent.anchorMax = new Vector2(1, 0);
				powerUpParent.anchoredPosition = new Vector3(-400,0,0);
			}

			if(playerIndex==0)
			{
				uiCamera.rect = new Rect(0f,0,1,1);
			}
			if(playerIndex==1)
			{
				uiCamera.rect = new Rect(0f,0,1,1);
			}

			playerPanel.GetComponent<Image>().sprite = 
				GameManager.Instance.GetPanels()[
					playerInputHandler.playerConfig.cursorIndex
				];
		}

		private void OnDestroy() 
		{
			
		}

		private void Update() 
		{
			
		}

		public void SetBuildCameraFollow(GameObject followTarget){buildCameraFollow = followTarget;}   
		public void SetPlayerStatus(PlayerStatus ps){
			playerStatus = ps;
			switch (ps){
				case PlayerStatus.Picking:
					PickingMode();
					break;
				case PlayerStatus.FinishedPicking:
					//playerInputHandler.virtualCursor.ResetMousePosition();
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
		public void AddPlayerScore(int addition) {
			playerScore+=addition;
			scoreText.text = playerScore + " / " + GameManager.Instance?.GetRoundType().GetPointRequirement();
		}
		
		public void SetPlacable(Placement.Placable pl) => placementManager.SetPlacable(pl);

	//TODO maybe put this another class
	/*
		public void SetControlHelpText(PlayerStatus status)
		{
			controlHelpText.text = "";
			switch (status){
				case PlayerStatus.Picking:
					break;
				case PlayerStatus.Building:
					controlHelpText.text+= " [Rotate] "+
						playerInputHandler.GetActionControlName
							(playerInputHandler.controls.BuildMode.Rotate);
					controlHelpText.text+= " [VerticalMotion] "+
						playerInputHandler.GetActionControlName
							(playerInputHandler.controls.BuildMode.VerticalMotion);
					controlHelpText.text+= " [Place] "+
						playerInputHandler.GetActionControlName
							(playerInputHandler.controls.BuildMode.Place);
					break;
				case PlayerStatus.Platforming:
					controlHelpText.text+= " [Jump] "+
						playerInputHandler.GetActionControlName
							(playerInputHandler.controls.Player.Jump);
					controlHelpText.text+= " [Crouch] "+
						playerInputHandler.GetActionControlName
							(playerInputHandler.controls.Player.Crouch);				
					controlHelpText.text+= " [Powerup] "+
						playerInputHandler.GetActionControlName
							(playerInputHandler.controls.Player.PowerUp);
					break;
			}
		}
	*/

		public void PickingMode()
		{
			playerInputHandler.virtualCursor.SetCursorTransparency(255);
			playerInputHandler.playerConfig.input.SwitchCurrentActionMap("UI"); 
			//SetControlHelpText(PlayerStatus.Picking);
		}

		public void BuildingMode()
		{
			buildCameraFollow.transform.position = GameManager.Instance.buildCamSetPoint.position;
			//playerController.EnableMeshAndCollider();
			playerController.SendBackToSpawn();
			virtualCamera.Follow = buildCameraFollow.transform;
			virtualCamera.LookAt = buildCameraFollow.transform;

			placementManager.SetCameraTransform(playerCamera.transform);
			placementManager.InstantiateNewPlacable();
			playerInputHandler.playerConfig.input.SwitchCurrentActionMap("BuildMode");
			//SetControlHelpText(PlayerStatus.Building);
			cinemachineInputHandler.look 
				= playerInputHandler.playerConfig.input.actions.FindAction("Look");
			// cinemachineInputHandler.vertical 
			// 	= playerInputHandler.playerConfig.input.actions.FindAction("Zoom");
			//cinemachineInputProvider.XYAxis= buildLookReference;
			//cinemachineInputProvider.ZAxis= buildLookReference;
		}

		[SerializeField]private InputActionReference playerLookReference,buildLookReference;

		public void PlatformingMode(){
			playerController.EnableMeshAndCollider();
			playerController.SendBackToSpawn();
			virtualCamera.Follow = playerController.gameObject.transform;
			virtualCamera.LookAt = playerController.gameObject.transform;

			playerInputHandler.playerConfig.input.SwitchCurrentActionMap("Player");
			//SetControlHelpText(PlayerStatus.Platforming);
			cinemachineInputHandler.look 
				= playerInputHandler.playerConfig.input.actions.FindAction("Look");
			// cinemachineInputHandler.vertical 
			// 	= playerInputHandler.playerConfig.input.actions.FindAction("Zoom");
			// cinemachineInputHandler.horizontal 
			// 	= playerInputHandler.playerConfig.input.actions.FindAction("Look");
			//cinemachineInputProvider.XYAxis= playerLookReference;
			//cinemachineInputProvider.ZAxis= playerLookReference;
		}

	}

	#if UNITY_EDITOR
    [CustomEditor(typeof(PlayerInstance))]
    class PlayerInstance1Editor : Editor{
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            PlayerInstance pInstance = (PlayerInstance) target;
            if (pInstance==null) return;

            if(GUILayout.Button("Print player status")){
                Debug.Log("Status = "+pInstance.playerStatus);
            }
        }
    }
    #endif
}