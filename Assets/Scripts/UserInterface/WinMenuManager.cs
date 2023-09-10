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

        // Start is called before the first frame update
        void Start()
        {
            List<PlayerConfiguration> sortedPlayerConfigs 
                = PlayerConfigurationManager.Instance.GetPlayerConfigs()
                    .OrderByDescending(o=>o.scoreTotal).ToList();
            foreach (var item in sortedPlayerConfigs)
            {
                scoreboardText.text += $"PlayerIndex {item.playerIndex+1} Score {item.scoreTotal}\n";
            }
            SoundManager.Instance?.PlaySound(SoundEnum.WooHooSound);
            VFXManager.Instance?.PlayEffect(VFXEnum.WinEffect
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