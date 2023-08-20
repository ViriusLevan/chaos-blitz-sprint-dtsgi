using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class WinningPanel : MonoBehaviour
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
            scoreboardText.text += $"1.Index {item.playerIndex} Score {item.scoreTotal}\n";
        }
    }

}
