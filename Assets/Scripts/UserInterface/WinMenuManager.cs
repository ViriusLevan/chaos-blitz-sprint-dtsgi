using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using LevelUpStudio.ChaosBlitzSprint.Player;

namespace LevelUpStudio.ChaosBlitzSprint.UI
{
    public class WinMenuManager : MonoBehaviour
    {
        [SerializeField]private TextMeshProUGUI scoreboardText;
        [SerializeField]private GameObject[]playerModels, modelDiffs;
        [SerializeField]private Animator[] modelAnimators;

        // Start is called before the first frame update
        void Start()
        {
            //if(PlayerConfigurationManager.Instance==null)return;
            scoreboardText.text="Final Score\n";
            List<PlayerConfiguration> sortedPlayerConfigs 
                = PlayerConfigurationManager.Instance.GetPlayerConfigs()
                    .OrderByDescending(o=>o.scoreTotal).ToList();
            for (int i = 0; i < sortedPlayerConfigs.Count; i++)
            {
                scoreboardText.text += $"Player {sortedPlayerConfigs[i].playerIndex+1}"
                    +$" {sortedPlayerConfigs[i].scoreTotal} pts\n";
                Debug.Log("index "+i+" count"+sortedPlayerConfigs.Count);
                playerModels[i].SetActive(true);
                modelDiffs[i].GetComponent<SkinnedMeshRenderer>().material 
                    = sortedPlayerConfigs[i].playerMaterial;
                if(i>0)
                    modelAnimators[i].SetTrigger("sink");
            }
            VFXManager.Instance?.PlayEffect(VFXEnum.Win
                , Camera.main.transform.position + (Vector3.forward*5)
                , new Vector3());
        }

        public void ExitWinMenu()
        {
            PlayerConfigurationManager.Instance.DisableJoining();
            PlayerConfigurationManager.Instance.DisableSplitScreen();
            PlayerConfigurationManager.Instance.ClearPlayers();
            SceneLoader.Instance?.LoadScene(SceneLoader.SceneIndex.MainMenu);
        }
    }
}