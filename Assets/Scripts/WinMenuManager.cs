using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

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
            scoreboardText.text += $"PlayerIndex {item.playerIndex} Score {item.scoreTotal}\n";
        }
    }

    public void ExitWinMenu()
    {
        PlayerConfigurationManager.Instance.DisableJoining();
        PlayerConfigurationManager.Instance.DisableSplitScreen();
        PlayerConfigurationManager.Instance.ClearPlayers();
        SceneLoader.Instance?.LoadScene(SceneLoader.SceneIndex.MainMenu);
    }
}
