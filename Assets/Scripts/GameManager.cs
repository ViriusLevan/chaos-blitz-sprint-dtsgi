using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]private PlacableSelectionPanel placableSelectionPanel;

    public enum GameStatus {Animation, PickPhase, BuildPhase, PlatformingPhase};

    [SerializeField] private RoundType[] availableRoundTypes;
    [SerializeField] private GameStatus currentGameStatus;
    private Dictionary<int, PlayerInstance> playerInstances;

    
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        if(Instance != null)
        {
            Debug.Log("[Singleton] Trying to instantiate a seccond instance of a singleton class.");
            return;
        }
        else
        {
            Instance = this;
        }
        playerInstances = new Dictionary<int, PlayerInstance>();
    }

    void Start()
    {
        List<PlayerConfiguration> pConfigs = PlayerConfigurationManager.Instance.GetPlayerConfigs();
        PhaseSwitch(GameStatus.PickPhase);
    }

    public void AddPlayerInstance(PlayerInstance pi)
    {   
        playerInstances.Add(pi.playerInputHandler.playerConfig.playerIndex, pi);
    }

    private void OnDestroy() 
    {
        
    }

    //Pending Usage/Deletion
    public enum PlayerEventType  {Picked,Built,Died}


    public delegate void OnPhase();
    public static event OnPhase pickPhaseFinished, buildPhaseFinished, platformingPhaseFinished
        , pickPhaseBegin, buildPhaseBegin, platformingPhaseBegin;
    

    public void PlayerPicked(int pIndex, Placable placable = null)
    {
        playerInstances[pIndex].SetPlacable(placable);
        playerInstances[pIndex].SetPlayerStatus(PlayerInstance.PlayerStatus.FinishedPicking);
        foreach(KeyValuePair<int, PlayerInstance> entry in playerInstances)
        {
            if(entry.Value.playerStatus != PlayerInstance.PlayerStatus.FinishedPicking){
                return;
            }
        }
        pickPhaseFinished?.Invoke();
        PhaseSwitch(GameStatus.BuildPhase);
    }

    public void PlayerBuilt(int pIndex)
    {
        playerInstances[pIndex].SetPlayerStatus(PlayerInstance.PlayerStatus.FinishedBuilding);
        foreach(KeyValuePair<int, PlayerInstance> entry in playerInstances)
        {
            if(entry.Value.playerStatus != PlayerInstance.PlayerStatus.FinishedBuilding){
                return;
            }
        }
        buildPhaseFinished?.Invoke();
        PhaseSwitch(GameStatus.PlatformingPhase);
    }

    public void PlayerDied(int pIndex)
    {
        playerInstances[pIndex].SetPlayerStatus(PlayerInstance.PlayerStatus.Dead);
        foreach(KeyValuePair<int, PlayerInstance> entry in playerInstances)
        {
            if(entry.Value.playerStatus != PlayerInstance.PlayerStatus.Dead 
                && entry.Value.playerStatus != PlayerInstance.PlayerStatus.FinishedPlatforming){
                return;
            }
        }
        platformingPhaseFinished?.Invoke();
        PhaseSwitch(GameStatus.PickPhase);
    }

    private int lapWinnerIndex;
    public void PlayerFinished(int pIndex)
    {
        Debug.Log($"Player {pIndex} has Finished");
        playerInstances[pIndex].SetPlayerStatus(PlayerInstance.PlayerStatus.FinishedPlatforming);
        if(lapWinnerIndex == -1) 
            lapWinnerIndex = pIndex;
        else 
            lapWinnerIndex = -1;

        foreach(KeyValuePair<int, PlayerInstance> entry in playerInstances)
        {
            if(entry.Value.playerStatus != PlayerInstance.PlayerStatus.Dead 
                && entry.Value.playerStatus != PlayerInstance.PlayerStatus.FinishedPlatforming){
                return;
            }
        }
        platformingPhaseFinished?.Invoke();
        PhaseSwitch(GameStatus.PickPhase);
    }

    private void PhaseSwitch(GameStatus targetPhase)
    {
        if(currentGameStatus==GameStatus.PlatformingPhase){
            if(lapWinnerIndex!=-1){
                playerInstances[lapWinnerIndex].AddPlayerScore(1);
                lapWinnerIndex=-1;
            }
        }

        currentGameStatus = targetPhase;
        foreach(KeyValuePair<int, PlayerInstance> entry in playerInstances)
        {
            switch(targetPhase){
                case GameStatus.PickPhase:
                    playerInstances[entry.Key].SetPlayerStatus(PlayerInstance.PlayerStatus.Picking);
                    break;
                case GameStatus.BuildPhase:
                    playerInstances[entry.Key].SetPlayerStatus(PlayerInstance.PlayerStatus.Building);
                    break;
                case GameStatus.PlatformingPhase:
                    playerInstances[entry.Key].SetPlayerStatus(PlayerInstance.PlayerStatus.Platforming);
                    break;
                case GameStatus.Animation:
                    playerInstances[entry.Key].SetPlayerStatus(PlayerInstance.PlayerStatus.Awaiting);
                    break;
            }
        }

        switch(targetPhase){
            case GameStatus.PickPhase:
                pickPhaseBegin?.Invoke();
                break;
            case GameStatus.BuildPhase:
                buildPhaseBegin?.Invoke();
                break;
            case GameStatus.PlatformingPhase:
                platformingPhaseBegin?.Invoke();
                break;
            case GameStatus.Animation:
                break;
        }
    }


}
