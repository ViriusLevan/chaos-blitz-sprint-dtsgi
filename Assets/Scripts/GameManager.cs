using System.Collections.Generic;
using UnityEngine;
using LevelUpStudio.ChaosBlitzSprint.Player;
using TMPro;

namespace LevelUpStudio.ChaosBlitzSprint
{
    public class GameManager : MonoBehaviour
    {
        
        public enum GameStatus {LapAnimation, PickPhase, BuildPhase, PlatformingPhase,Paused};

        [SerializeField] public GameStatus currentGameStatus;
        public GameStatus GetCurrentGameStatus(){return currentGameStatus;}
        private Dictionary<int, PlayerInstance> playerInstances;

        public static GameManager Instance { get; private set; }

        [SerializeField]private int lapCounter;
        [SerializeField]private RoundType roundType;
        public RoundType GetRoundType(){return roundType;}

        [SerializeField]private bool roundFinished=false;
        //TODO put these into another class?
        [SerializeField]private Transform[] playerSpawnPoints;
        //
        [SerializeField]private Sprite[] cursors;
        [SerializeField] private Sprite[] panels;
        //
        public Transform buildCamSetPoint;


        //TODO split to another class
        public enum CursorColors{Red=0,Purple=1, Yellow=2, Green=3};
        public Sprite[] GetCursors(){
            
            return cursors;}
        public Sprite[] GetPanels(){return panels;}
        //
        public Transform[] GetPlayerSpawnPoints(){return playerSpawnPoints;}
        public int GetLapCounter(){return lapCounter;}

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
            lapCounter=1;
            lapWinnerIndex=-1;
            playerInstances = new Dictionary<int, PlayerInstance>();
            roundType = PlayerConfigurationManager.Instance.roundType;
            List<PlayerConfiguration> pConfigs = PlayerConfigurationManager.Instance.GetPlayerConfigs();
        }

        [SerializeField] private GameObject ghostWalls;
        [SerializeField] private Animator startAnimator;
        [SerializeField] private TextMeshProUGUI lapText;
        [SerializeField] private GameObject lapPanel;
        void Start()
        {
          //  PhaseSwitch(GameStatus.LapAnimation);
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
            , pickPhaseBegin, buildPhaseBegin, platformingPhaseBegin, gamePaused, gameUnpaused;
        
        public void AnimationFinished()
        {
            lapPanel.SetActive(false);
            PhaseSwitch(GameStatus.PickPhase);           
        }

        public void PlayerPicked(int pIndex, Placement.Placable placable = null)
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
           // PhaseSwitch(GameStatus.PickPhase);//
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
            List<int> finishedPlayers = new List<int>();
        //already dead
            if(playerInstances[pIndex].playerStatus==PlayerInstance.PlayerStatus.Dead
                ||playerInstances[pIndex].playerStatus==PlayerInstance.PlayerStatus.Picking
                ||playerInstances[pIndex].playerStatus==PlayerInstance.PlayerStatus.AwaitingAnimation)return;
            playerInstances[pIndex].SetPlayerStatus(PlayerInstance.PlayerStatus.Dead);
            Debug.Log($"Player {pIndex} has Died");
            SoundManager.Instance?.PlaySound(SoundEnum.PlayerDeath);
            foreach(KeyValuePair<int, PlayerInstance> entry in playerInstances)
            {
                if(entry.Value.playerStatus == PlayerInstance.PlayerStatus.FinishedPlatforming)
                    finishedPlayers.Add(entry.Key);
                if(entry.Value.playerStatus != PlayerInstance.PlayerStatus.Dead 
                    && entry.Value.playerStatus != PlayerInstance.PlayerStatus.FinishedPlatforming){
                    return;
                }
            }
            platformingPhaseFinished?.Invoke();
            HandleScoring(finishedPlayers);
            PhaseSwitch(GameStatus.LapAnimation);
        }

        private int lapWinnerIndex;
        public void PlayerFinished(int pIndex)
        {
            List<int> finishedPlayers = new List<int>();
            if(playerInstances[pIndex].playerStatus==PlayerInstance.PlayerStatus.FinishedPlatforming
                ||playerInstances[pIndex].playerStatus==PlayerInstance.PlayerStatus.Picking
                ||playerInstances[pIndex].playerStatus==PlayerInstance.PlayerStatus.AwaitingAnimation)return;
            SoundManager.Instance?.PlaySound(SoundEnum.PlayerFinish);
            Debug.Log($"Player {pIndex} has Finished");
            playerInstances[pIndex].SetPlayerStatus(PlayerInstance.PlayerStatus.FinishedPlatforming);
            if(lapWinnerIndex == -1) 
                lapWinnerIndex = pIndex;
            foreach(KeyValuePair<int, PlayerInstance> entry in playerInstances)
            {
                if(entry.Value.playerStatus == PlayerInstance.PlayerStatus.FinishedPlatforming)
                    finishedPlayers.Add(entry.Key);
                if(entry.Value.playerStatus != PlayerInstance.PlayerStatus.Dead 
                    && entry.Value.playerStatus != PlayerInstance.PlayerStatus.FinishedPlatforming){
                    return;
                }
            }
            platformingPhaseFinished?.Invoke();
            HandleScoring(finishedPlayers);
            PhaseSwitch(GameStatus.LapAnimation);
        }


        private void HandleScoring(List<int> indexOfFinishedPlayers)
        {
            for (int i = 0; i < indexOfFinishedPlayers.Count; i++)
            {
                playerInstances[i].AddPlayerScore(5);
                if(indexOfFinishedPlayers[i]==lapWinnerIndex)
                    playerInstances[i].AddPlayerScore(2);
                if(playerInstances[i].playerScore>=roundType.GetPointRequirement()){
                    roundFinished=true;
                }
            }
            lapCounter+=1;

            if(lapCounter>roundType.GetLapAmount()){
                roundFinished=true;
            }
        }



        public void PhaseSwitch(GameStatus targetPhase)
        {
            if(roundFinished){
                //Move score data to their respective PlayerConfiguration in P..C..Manager
                List<PlayerConfiguration> playerConfigs 
                        = PlayerConfigurationManager.Instance.GetPlayerConfigs();
                foreach (var item in playerConfigs)
                {
                    item.scoreTotal = playerInstances[item.playerIndex].playerScore;
                }
                SceneLoader.Instance.LoadScene(SceneLoader.SceneIndex.Winning);
            }

            currentGameStatus = targetPhase;
            foreach(KeyValuePair<int, PlayerInstance> entry in playerInstances)
            {
                switch(targetPhase){
                    case GameStatus.LapAnimation:
                        playerInstances[entry.Key]
                        .SetPlayerStatus(PlayerInstance.PlayerStatus.AwaitingAnimation);
                        break;
                    case GameStatus.PickPhase:
                        if(playerInstances[entry.Key].playerStatus
                            !=PlayerInstance.PlayerStatus.FinishedPicking)
                            playerInstances[entry.Key]
                                .SetPlayerStatus(PlayerInstance.PlayerStatus.Picking);
                        break;                    
                    case GameStatus.BuildPhase:
                        //ghostWalls.SetActive(true);
                        if(playerInstances[entry.Key].playerStatus
                            !=PlayerInstance.PlayerStatus.FinishedBuilding)
                            playerInstances[entry.Key]
                                .SetPlayerStatus(PlayerInstance.PlayerStatus.Building);
                        break;
                    case GameStatus.PlatformingPhase:
                        ghostWalls.SetActive(false);
                        if(playerInstances[entry.Key].playerStatus
                            !=PlayerInstance.PlayerStatus.FinishedPlatforming)
                            playerInstances[entry.Key]
                                .SetPlayerStatus(PlayerInstance.PlayerStatus.Platforming);
                        break;
                    case GameStatus.Paused:
                        playerInstances[entry.Key]
                            .SetPlayerStatus(PlayerInstance.PlayerStatus.Paused);
                        break;
                }
            }
            
            if(currentGameStatus == GameStatus.Paused)
            {
                gameUnpaused.Invoke();
                currentGameStatus = targetPhase;
                return;
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
                case GameStatus.LapAnimation:
                    lapPanel.SetActive(true);
                    lapText.text = "Round "+ lapCounter + " of " + roundType.GetLapAmount();
                    startAnimator.SetTrigger("start");
                    break;
                case GameStatus.Paused:
                    gamePaused.Invoke();
                    break;
            }
        }
    }
}