using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LevelUpStudio.ChaosBlitzSprint.Player{
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
		public CinemachineInputHandler cinemachineInputHandler{get; private set;}
		//public CinemachineInputProvider cinemachineInputProvider;

		[SerializeField] RectTransform playerPanel;
		[SerializeField] TextMeshProUGUI playerText, scoreText, controlHelpText;

		private void Awake() {
			cinemachineInputHandler = GetComponentInChildren<CinemachineInputHandler>();
			//cinemachineInputProvider = GetComponentInChildren<CinemachineInputProvider>();
			placementManager = GetComponentInChildren<PlacementManager>();
			playerController = GetComponentInChildren<PlayerController>();
			playerInputHandler = GetComponentInChildren<PlayerInputHandler>();
			playerCamera = GetComponentInChildren<Camera>().gameObject;
			
		}

		void Start()
		{
			placementManager?.SetReferenceTransform(buildCameraFollow.transform);
			int playerIndex = playerInputHandler.playerConfig.playerIndex;
			//freeLookCamera.GetComponent<CinemachineInputProvider>().PlayerIndex = playerIndex;
			playerText.text = "Player "+(playerIndex+1);
			
			if(playerIndex==1 || playerIndex==3)
			{
				Debug.Log("PlayerIndex="+playerIndex);
				Vector2 currentPos = playerPanel.anchoredPosition;
				playerPanel.anchorMin = new Vector2(1, 1);
				playerPanel.anchorMax = new Vector2(1, 1);
				// Vector2 shiftDirection 
				// 	= new Vector2(0.5f - playerPanel.anchorMax.x, 0.5f - playerPanel.anchorMax.y);
				// playerPanel.anchoredPosition = shiftDirection * playerPanel.rect.size;    
				currentPos.x *= -1;
				playerPanel.anchoredPosition=currentPos;
			}

			playerPanel.GetComponent<Image>().sprite = 
				GameManager.Instance.GetPanels()[
					playerInputHandler.playerConfig.cursorIndex
				];
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
		public void AddPlayerScore(int addition) {
			playerScore+=addition;
			scoreText.text = playerScore+" pts";
		}
		
		public void SetPlacable(Placement.Placable pl) => placementManager.SetPlacable(pl);

	//TODO maybe put this another class
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
							(playerInputHandler.controls.Player.ActivatePowerUp);
					break;
			}
		}

		public void PickingMode()
		{
			playerInputHandler.virtualCursor.SetCursorTransparency(255);
			playerInputHandler.playerConfig.input.SwitchCurrentActionMap("UI"); 
			SetControlHelpText(PlayerStatus.Picking);
		}

		public void BuildingMode()
		{
			buildCameraFollow.transform.position = GameManager.Instance.buildCamSetPoint.position;
			//playerController.EnableMeshAndCollider();
			playerController.SendBackToSpawn();
			freeLookCamera.Follow = buildCameraFollow.transform;
			freeLookCamera.LookAt = buildCameraFollow.transform;
			freeLookCamera.m_Lens.FieldOfView = 80;
			freeLookCamera.m_Orbits[0].m_Height = 15;
			freeLookCamera.m_Orbits[0].m_Radius = 20;
			freeLookCamera.m_Orbits[1].m_Height = 8;
			freeLookCamera.m_Orbits[1].m_Radius = 12;
			freeLookCamera.m_Orbits[2].m_Height = 4;
			freeLookCamera.m_Orbits[2].m_Radius = 5;

			placementManager.SetCameraTransform(playerCamera.transform);
			placementManager.InstantiateNewPlacable();
			playerInputHandler.playerConfig.input.SwitchCurrentActionMap("BuildMode");
			SetControlHelpText(PlayerStatus.Building);
			cinemachineInputHandler.horizontal 
				= playerInputHandler.playerConfig.input.actions.FindAction("Look");
			//cinemachineInputProvider.XYAxis= buildLookReference;
			//cinemachineInputProvider.ZAxis= buildLookReference;
		}

		[SerializeField]private InputActionReference playerLookReference,buildLookReference;

		public void PlatformingMode(){
			playerController.EnableMeshAndCollider();
			playerController.SendBackToSpawn();
			freeLookCamera.Follow = playerController.gameObject.transform;
			freeLookCamera.LookAt = playerController.gameObject.transform;
			freeLookCamera.m_Lens.FieldOfView = 60;
			freeLookCamera.m_Orbits[0].m_Height = 6;
			freeLookCamera.m_Orbits[0].m_Radius = 10;
			freeLookCamera.m_Orbits[1].m_Height = 4;
			freeLookCamera.m_Orbits[1].m_Radius = 7;
			freeLookCamera.m_Orbits[2].m_Height = 2;
			freeLookCamera.m_Orbits[2].m_Radius = 5;

			playerInputHandler.playerConfig.input.SwitchCurrentActionMap("Player");
			SetControlHelpText(PlayerStatus.Platforming);
			cinemachineInputHandler.horizontal 
				= playerInputHandler.playerConfig.input.actions.FindAction("Look");
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