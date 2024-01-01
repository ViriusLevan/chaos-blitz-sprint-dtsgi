using UnityEngine;
using Cinemachine;

namespace LevelUpStudio.ChaosBlitzSprint.Player
{
    public class SetCameraOnSpawn : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private Camera selfCamera;

        private void Awake() {
            virtualCamera.Follow = transform;
            virtualCamera.LookAt = transform;
            int currentNPlayers = PlayerConfigurationManager.Instance.GetNSpawnedPlayers();
            Debug.Log(currentNPlayers);
            switch(currentNPlayers){
                case 0: 
                    virtualCamera.gameObject.layer = LayerMask.NameToLayer("P1Cam"); 
                    selfCamera.cullingMask = ~((1 << LayerMask.NameToLayer("P2Cam")) 
                                            | (1 << LayerMask.NameToLayer("P3Cam")) 
                                            | (1 << LayerMask.NameToLayer("P4Cam")));
                    break;
                case 1: 
                    virtualCamera.gameObject.layer = LayerMask.NameToLayer("P2Cam"); 
                    selfCamera.cullingMask = ~((1 << LayerMask.NameToLayer("P1Cam")) 
                                            | (1 << LayerMask.NameToLayer("P3Cam")) 
                                            | (1 << LayerMask.NameToLayer("P4Cam")));
                    break;
                case 2: 
                    virtualCamera.gameObject.layer = LayerMask.NameToLayer("P3Cam"); 
                    selfCamera.cullingMask = ~((1 << LayerMask.NameToLayer("P1Cam")) 
                                            | (1 << LayerMask.NameToLayer("P2Cam")) 
                                            | (1 << LayerMask.NameToLayer("P4Cam")));
                    break;
                case 3: 
                    virtualCamera.gameObject.layer = LayerMask.NameToLayer("P4Cam"); 
                    selfCamera.cullingMask = ~((1 << LayerMask.NameToLayer("P1Cam")) 
                                            | (1 << LayerMask.NameToLayer("P2Cam")) 
                                            | (1 << LayerMask.NameToLayer("P3Cam")));
                    break;
            }
            GetComponentInParent<PlayerInstance>().SetBuildCameraFollow(buildCameraFollowTarget);
            PlayerConfigurationManager.Instance.SetNSpawnedPlayers(currentNPlayers+1);
        }
        [SerializeField]private GameObject buildCameraFollowTarget;
    }
}