using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlacableSelectionPanel : MonoBehaviour
{
    
    [SerializeField] private GameObject placableButtonPrefab;
    [SerializeField] private Transform selectionPanel;
    [SerializeField] private Placable[] placables;
    private Image image;
    private Color initialColor;
    private void Start() {
        image = GetComponent<Image>();
        initialColor =  image.color;
        GameManager.pickPhaseBegin+=PopulatePickPanel;
        GameManager.pickPhaseFinished+=HidePickPanel;
    }

    private void OnDestroy() {
        GameManager.pickPhaseBegin-=PopulatePickPanel;
        GameManager.pickPhaseFinished-=HidePickPanel;
    }

    public void PopulatePickPanel(){
        ShowPickPanel();
        int nOfPlayers = PlayerConfigurationManager.Instance.GetNSpawnedPlayers();
        
        RectTransform selectionPanelRT = selectionPanel.gameObject.GetComponent<RectTransform>();

        //TODO objects spawned = nOfPlayers (+0-+2)

        for (int i = 0; i < nOfPlayers+2; i++)
        {   
            //TODO Improve PlacableButton Position Randomization
            // Vector3 spawnPosition 
            //     = GetBottomLeftCorner(selectionPanelRT) 
            //         - new Vector3(Random.Range(0, selectionPanelRT.rect.x)
            //             , Random.Range(0, selectionPanelRT.rect.y), 0);

            //TODO adjust this, both the RNG and the button placement
            int index=0;
            if(GameManager.Instance.GetLapCounter()<1)
            {
                do{
                    index=Random.Range(0,placables.Length);
                }while(placables[index].GetPlacableType()==Placable.PlacableType.Hazard);
            }
            else
            {
                index=Random.Range(0,placables.Length);
            }

            GameObject prefabInstance = Instantiate(placableButtonPrefab, selectionPanel);
            prefabInstance.GetComponent<PlacableButton>()?.SetPlacable(
                placables[index]
            );                        
                       
        }
    }

    public void HidePickPanel(){
        image.color = new Color(0,0,0,0);
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        DestroySelections();
    }

    public void DestroySelections()
    {
        int i = 0;

        //Array to hold all child obj
        GameObject[] allChildren = new GameObject[selectionPanel.transform.childCount];

        //Find all child obj and store to that array
        foreach (Transform child in selectionPanel.transform)
        {
            allChildren[i] = child.gameObject;
            i += 1;
        }

        //Now destroy them
        foreach (GameObject child in allChildren)
        {
            Destroy(child.gameObject);
        }
    }

    public void ShowPickPanel(){
        image.color = initialColor;
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
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
