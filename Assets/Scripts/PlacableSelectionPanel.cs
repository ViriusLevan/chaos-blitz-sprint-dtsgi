using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacableSelectionPanel : MonoBehaviour
{
    
    [SerializeField] private GameObject placableButtonPrefab;
    [SerializeField] private Transform selectionPanel;
    [SerializeField] private Placable[] placables;
    public void PopulatePickPanel(){
        int nOfPlayers = PlayerConfigurationManager.Instance.GetNSpawnedPlayers();
        
        RectTransform selectionPanelRT = selectionPanel.gameObject.GetComponent<RectTransform>();

        for (int i = 0; i < nOfPlayers; i++)
        {
            //TODO Improve PlacableButton Position Randomization
            // Vector3 spawnPosition 
            //     = GetBottomLeftCorner(selectionPanelRT) 
            //         - new Vector3(Random.Range(0, selectionPanelRT.rect.x)
            //             , Random.Range(0, selectionPanelRT.rect.y), 0);

            GameObject prefabInstance = Instantiate(placableButtonPrefab, selectionPanel);
            prefabInstance.GetComponent<PlacableButton>()?.SetPlacable(
                placables[Random.Range(0,placables.Length)]
            );                        
                       
        }
    }

//Pending usage/deletion
    // Vector3 GetBottomLeftCorner(RectTransform rt)
    // {
    //     Vector3[] v = new Vector3[4];
    //     rt.GetWorldCorners(v);
    //     return v[0];
    // }
}
