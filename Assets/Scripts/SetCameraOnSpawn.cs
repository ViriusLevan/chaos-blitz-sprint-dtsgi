using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SetCameraOnSpawn : MonoBehaviour
{
    [SerializeField]private CinemachineVirtualCamera virtCam;
    [SerializeField]private Camera selfCamera;

    private void Awake() {
        virtCam.Follow = transform;
        virtCam.LookAt = transform;
        int currentNPlayers = PlayerConfigurationManager.Instance.GetNSpawnedPlayers();
        switch(currentNPlayers){
            case 0: 
                virtCam.gameObject.layer = LayerMask.NameToLayer("P1Cam"); 
                selfCamera.cullingMask = ~((1 << LayerMask.NameToLayer("P2Cam")) 
                                        | (1 << LayerMask.NameToLayer("P3Cam")) 
                                        | (1 << LayerMask.NameToLayer("P4Cam")));
                break;
            case 1: 
                virtCam.gameObject.layer = LayerMask.NameToLayer("P2Cam"); 
                selfCamera.cullingMask = ~((1 << LayerMask.NameToLayer("P1Cam")) 
                                        | (1 << LayerMask.NameToLayer("P3Cam")) 
                                        | (1 << LayerMask.NameToLayer("P4Cam")));
                break;
            case 2: 
                virtCam.gameObject.layer = LayerMask.NameToLayer("P3Cam"); 
                selfCamera.cullingMask = ~((1 << LayerMask.NameToLayer("P1Cam")) 
                                        | (1 << LayerMask.NameToLayer("P2Cam")) 
                                        | (1 << LayerMask.NameToLayer("P4Cam")));
                break;
            case 3: 
                virtCam.gameObject.layer = LayerMask.NameToLayer("P4Cam"); 
                selfCamera.cullingMask = ~((1 << LayerMask.NameToLayer("P1Cam")) 
                                        | (1 << LayerMask.NameToLayer("P2Cam")) 
                                        | (1 << LayerMask.NameToLayer("P3Cam")));
                break;
        }
        currentNPlayers+=1;
        PlayerConfigurationManager.Instance.SetNSpawnedPlayers(currentNPlayers);
    }
}
