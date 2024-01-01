using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LevelUpStudio.ChaosBlitzSprint.Player
{
    public class PlayerConfigurationManager : MonoBehaviour
    {
        [SerializeField] private int minPlayers=1, maxPlayers = 4;
        [SerializeField] private PlayerInputManager pim;
        [SerializeField] private List<PlayerConfiguration> playerConfigs;
        //[SerializeField] private List<GameObject> playerJoinTexts;
        private int nSpawnedPlayers = 0;

        //TODO maybe make another static class for this?
        public RoundType roundType;

        public static PlayerConfigurationManager Instance { get; private set; }

        public void VariableReset(SceneLoader.SceneIndex sceneIndex)
        {
            if(sceneIndex!= SceneLoader.SceneIndex.MainMenu)return;
            ClearPlayers();
            nSpawnedPlayers = 0;
        }

        private void Awake()
        {
            if(Instance != null)
            {
                Debug.Log("[Singleton] Trying to instantiate a seccond instance of a singleton class.");
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(Instance);
                playerConfigs = new List<PlayerConfiguration>();
            }
        }

        private void Start() 
        {
            colorPlayerIndex = new Dictionary<string, int>();
            SceneLoader.sceneLoaded+=VariableReset;
        }
        void OnDestroy()
        {
            SceneLoader.sceneLoaded-=VariableReset;
            if(this==Instance)
            {
                DisableSplitScreen();
            }
        }

        public void HandlePlayerJoin(PlayerInput pi)
        {
            Debug.Log("player joined " + pi.playerIndex);
            pi.transform.SetParent(transform);

            if(!playerConfigs.Any(p => p.playerIndex == pi.playerIndex))
            {
                playerConfigs.Add(new PlayerConfiguration(pi));
            }

            //playerJoinTexts[pi.playerIndex].SetActive(false);
        }
        
        public void SetNSpawnedPlayers(int newVal)
        {
            Debug.Log("nVal "+newVal);
            nSpawnedPlayers=newVal;
        }

        public int GetNSpawnedPlayers()
        {
            return nSpawnedPlayers;
        }

        public void EnableSplitScreen()
        {
            pim.splitScreen=true;
        }

        public void DisableSplitScreen()
        {
            pim.splitScreen=false;
        }

        public void EnableJoining()
        {
            pim.EnableJoining();
        }

        public void DisableJoining()
        {
            pim.DisableJoining();
        }

        public void ClearPlayers()
        {
            playerConfigs.Clear();
            colorPlayerIndex.Clear();

    //Deletes the children of this transform, which are the PlayerConfiguration prefabs
            int i = 0;
            //Array to hold all child obj
            GameObject[] allChildren = new GameObject[transform.childCount];

            //Find all child obj and store to that array
            foreach (Transform child in transform)
            {
                allChildren[i] = child.gameObject;
                i += 1;
            }

            //Now destroy them
            foreach (GameObject child in allChildren)
            {
                Destroy(child.gameObject);
            }
            Debug.Log("Players cleared");
        }

        public List<PlayerConfiguration> GetPlayerConfigs()
        {
            return playerConfigs;
        }

        public delegate void OnColor(string colorName);
        public static event OnColor colorTaken, colorReturned;
        private Dictionary<string,int>colorPlayerIndex;
        public bool IsColorTaken(string colorKey){return colorPlayerIndex.ContainsKey(colorKey);}
        public void SetPlayerColor(int index, Material color, int cIndex)
        {
            colorPlayerIndex[color.name]=index;
            colorTaken?.Invoke(color.name);

            playerConfigs[index].playerMaterial = color;
            playerConfigs[index].cursorIndex = cIndex;
        }

        public void ReadyPlayer(int index)
        {
            Debug.Log($"Player {index} ready");
            playerConfigs[index].isReady = true;
            
            //BeginGame();
        }

        public void CancelReady(int index)
        {
            string keyToRemove="";
            foreach(KeyValuePair<string, int> entry in colorPlayerIndex)
            {
                if(entry.Value == index){
                    keyToRemove =entry.Key;
                    break;
                }
            }
            colorPlayerIndex.Remove(keyToRemove);
            colorReturned?.Invoke(keyToRemove);

            Debug.Log($"Player {index} cancelled");
            playerConfigs[index].isReady = false;
        }

        public bool IsPlayerReady(int index)
        {
            return playerConfigs[index].isReady;
        }

        public void BeginGame()
        {
            if(roundType==null)
            {
                Debug.Log("Round type not selected");
                return;
            }
            if (minPlayers <= playerConfigs.Count 
                && playerConfigs.Count <= maxPlayers 
                && playerConfigs.All(p => p.isReady == true))
            {
                DisableJoining();
                SceneLoader.Instance.LoadScene(SceneLoader.SceneIndex.Gameplay);
            }
            else
            {
                if(minPlayers > playerConfigs.Count)
                {
                    Debug.Log("Not enough players to start");
                }
                else
                {
                    Debug.Log("Not all players are ready");
                }
            }
        }
    }
}