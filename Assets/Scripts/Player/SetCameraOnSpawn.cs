using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SetCameraOnSpawn : MonoBehaviour
{
    [SerializeField] private CinemachineFreeLook freeLookCam;
    [SerializeField] private Camera selfCamera;

    private void Awake() {
        freeLookCam.Follow = transform;
        freeLookCam.LookAt = transform;
        int currentNPlayers = PlayerConfigurationManager.Instance.GetNSpawnedPlayers();
        Debug.Log(currentNPlayers);
        switch(currentNPlayers){
            case 0: 
                freeLookCam.gameObject.layer = LayerMask.NameToLayer("P1Cam"); 
                selfCamera.cullingMask = ~((1 << LayerMask.NameToLayer("P2Cam")) 
                                        | (1 << LayerMask.NameToLayer("P3Cam")) 
                                        | (1 << LayerMask.NameToLayer("P4Cam")));
                break;
            case 1: 
                freeLookCam.gameObject.layer = LayerMask.NameToLayer("P2Cam"); 
                selfCamera.cullingMask = ~((1 << LayerMask.NameToLayer("P1Cam")) 
                                        | (1 << LayerMask.NameToLayer("P3Cam")) 
                                        | (1 << LayerMask.NameToLayer("P4Cam")));
                break;
            case 2: 
                freeLookCam.gameObject.layer = LayerMask.NameToLayer("P3Cam"); 
                selfCamera.cullingMask = ~((1 << LayerMask.NameToLayer("P1Cam")) 
                                        | (1 << LayerMask.NameToLayer("P2Cam")) 
                                        | (1 << LayerMask.NameToLayer("P4Cam")));
                break;
            case 3: 
                freeLookCam.gameObject.layer = LayerMask.NameToLayer("P4Cam"); 
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
