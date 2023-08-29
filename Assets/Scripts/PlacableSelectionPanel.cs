using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlacableSelectionPanel : MonoBehaviour
{
    
    [SerializeField] private GameObject placableButtonPrefab;
    [SerializeField] private Transform selectionPanel;
    [SerializeField] private Placable[] placables;
    [SerializeField]private Image bgImage;
    private Color initialColor;
    private void Start() {
        initialColor =  bgImage.color;
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
        
        int xDivisor=-1,yDivisor=-1;
        List<FloatRange> xRanges, yRanges;
        xRanges = new List<FloatRange>();
        yRanges = new List<FloatRange>();
        Dictionary<Vector2,bool> isPartitionOccupied = new Dictionary<Vector2, bool>();
        
        List<Button> spawnedButtons = new List<Button>();
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

            GameObject prefabInstance = Instantiate(placableButtonPrefab, Vector3.zero, Quaternion.identity, selectionPanel);
            prefabInstance.GetComponent<PlacableButton>()?.SetPlacable(
                placables[index]
            );
            Button newButton = prefabInstance.GetComponent<Button>();
            RectTransform panelRect = selectionPanel.GetComponent<RectTransform>();

            float yPanelSize = panelRect.rect.yMax-panelRect.rect.yMin;
            float xPanelSize = panelRect.rect.xMax-panelRect.rect.xMin;
            float xButtonSize = newButton.GetComponent<RectTransform>().rect.xMax 
                - newButton.GetComponent<RectTransform>().rect.xMin;
            float yButtonSize = newButton.GetComponent<RectTransform>().rect.yMax 
                - newButton.GetComponent<RectTransform>().rect.yMin;

            if(yDivisor==-1 || xDivisor==-1){
                xDivisor = FindTightestDivisor(xPanelSize, xButtonSize);
                yDivisor = FindTightestDivisor(yPanelSize, yButtonSize);
                Debug.Log($"Panel Size {xPanelSize}-{yPanelSize}");
                for (int i1 = 0; i1 < xDivisor; i1++)
                {
                    for (int i2 = 0; i2 < yDivisor; i2++)
                    {
                        isPartitionOccupied.Add(
                            new Vector2(i1,i2),false
                        );
                    }
                }
            }
            
            if(xRanges.Count<1 || yRanges.Count<1){
                xRanges = GetRangeList(xPanelSize,xDivisor);
                yRanges = GetRangeList(yPanelSize,yDivisor);
            }
            Vector2 rangeKey;
            do{
                int xRangeIndex = Random.Range(0,xDivisor);
                int yRangeIndex = Random.Range(0,yDivisor);
                rangeKey = new Vector2(xRangeIndex,yRangeIndex);
            }while(isPartitionOccupied[rangeKey]);

            float xPos = Random.Range(xRanges[(int)rangeKey.x].minVal, xRanges[(int)rangeKey.x].maxVal);
            float yPos = Random.Range(yRanges[(int)rangeKey.y].minVal, yRanges[(int)rangeKey.y].maxVal);
            
            xPos = (xPos>xPanelSize/2) ? xPos/2 : xPos*-1;
            yPos = (yPos>yPanelSize/2) ? yPos/2 : yPos*-1;

            //Debug.Log($"Generated Position {xPos}-{yPos}");
            Vector3 spawnPosition = panelRect.TransformDirection(new Vector3(xPos, yPos, 0f));
            newButton.transform.localPosition = spawnPosition;
            
            Debug.Log($"Button Position {newButton.transform.localPosition}");
            isPartitionOccupied[rangeKey]=true;

            // if(spawnedButtons.Count>0)
            // {
            //     bool isOverlapping = false;
            //     int loopCounter=0;
            //     do{
            //         isOverlapping = false;
            //         if(isOverlapping)
            //         {
            //             xPos = Random.Range(panelRect.rect.xMin, panelRect.rect.xMax);
            //             yPos = Random.Range(panelRect.rect.yMin, panelRect.rect.yMax);
            //             spawnPosition = panelRect.TransformDirection(new Vector3(xPos, yPos, 0f));
            //             newButton.transform.localPosition = spawnPosition;
            //         }
            //         foreach (Button butt in spawnedButtons)
            //         {
            //             if(ButtonRectOverlaps(newButton , butt))
            //             {
            //                 isOverlapping=true;
            //             }
            //         }
            //         loopCounter+=1;
            //     }while(isOverlapping && loopCounter<1000);
            //     if(loopCounter>=100)Debug.Log("loop counter exceeded limit isOv"+isOverlapping.ToString());
            // }
            // spawnedButtons.Add(newButton);
                       
        }
    }

    public int FindTightestDivisor(float toBeDivided, float minimumSize)
    {
        int divisor = 1;
        for (;;)
        {
            if(toBeDivided/(divisor+1)<minimumSize)
            {
                return divisor;
            }
            divisor+=1;
        }
    }

    public List<FloatRange> GetRangeList(float toBeDivided, int divisor)
    {
        float interval = toBeDivided/divisor;
        float counter = 0;
        List<FloatRange> rangeList = new List<FloatRange>();
        for (int i = 0; i < divisor; i++)
        {
            FloatRange floatRange = new FloatRange(counter, counter+interval);
            if(floatRange.maxVal>toBeDivided)
                floatRange.maxVal = toBeDivided;

            rangeList.Add(floatRange);
            counter+=interval;
        }
        return rangeList;
    }

    public void HidePickPanel(){
        bgImage.color = new Color(0,0,0,0);
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
        bgImage.color = initialColor;
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


    // public RectTransform panelRect;
    // public Button buttonPrefab;

    // void GenerateButtons(List<Button> spawnedButtons)
    // {
    //     int numberOfButtons = 5;
    //     for (int i = 0; i < numberOfButtons; i++)
    //     {
    //         float xPos = Random.Range(panelRect.rect.xMin, panelRect.rect.xMax);
    //         float yPos = Random.Range(panelRect.rect.yMin, panelRect.rect.yMax);
    //         Vector3 spawnPosition = panelRect.TransformDirection(new Vector3(xPos, yPos, 0f));
    //         Button newButton = Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity, panelRect);
    //         newButton.transform.localPosition = spawnPosition;

    //         if(spawnedButtons.Count>0)
    //         {
    //             bool isOverlapping = false;
    //             do{
    //                 if(isOverlapping)
    //                 {
    //                     xPos = Random.Range(panelRect.rect.xMin, panelRect.rect.xMax);
    //                     yPos = Random.Range(panelRect.rect.yMin, panelRect.rect.yMax);
    //                     spawnPosition = panelRect.TransformDirection(new Vector3(xPos, yPos, 0f));
    //                     newButton.transform.localPosition = spawnPosition;
    //                 }
    //                 foreach (Button butt in spawnedButtons)
    //                 {
    //                     if(ButtonRectOverlaps(newButton , butt))
    //                     {
    //                         isOverlapping=true;
    //                     }
    //                 }
    //             }while(isOverlapping);
    //         }
    //         spawnedButtons.Add(newButton);
    //     }
    // }

    bool ButtonRectOverlaps(Button but1, Button but2)
    {
        RectTransform rectTrans1 = but1.GetComponent<RectTransform>();
        RectTransform rectTrans2 = but2.GetComponent<RectTransform>();

        Rect rect1 = new Rect(rectTrans1.localPosition.x, rectTrans1.localPosition.y, rectTrans1.rect.width, rectTrans1.rect.height);
        Rect rect2 = new Rect(rectTrans2.localPosition.x, rectTrans2.localPosition.y, rectTrans2.rect.width, rectTrans2.rect.height);

        return rect1.Overlaps(rect2);
    }
}

public struct FloatRange
{
    public FloatRange(float minim, float maxim)
    {
        minVal = minim;
        maxVal = maxim;
    }
    public float minVal;
    public float maxVal;
}
