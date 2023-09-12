using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LevelUpStudio.ChaosBlitzSprint.Placement;
using LevelUpStudio.ChaosBlitzSprint.Player;

namespace LevelUpStudio.ChaosBlitzSprint.UI
{
    public class PlacableSelectionPanel : MonoBehaviour
    {
        
        [SerializeField] private GameObject placableButtonPrefab;
        [SerializeField] private Transform selectionPanel;
        [SerializeField] private Placable[] placables;
        [SerializeField]private Image bgImage;
        [SerializeField]private GameObject controlHelpPanel;

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
                //TODO adjust this, both the RNG and the button placement
                int index=0;
                if(GameManager.Instance.GetLapCounter()<1)
                {
                    do{
                        index=Random.Range(0,placables.Length);
                    }while(placables[index].GetPlacableType()!=Placable.PlacableType.Platform);
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
                    xDivisor = FindTightestDivisor(xPanelSize, xButtonSize*1.5f);
                    yDivisor = FindTightestDivisor(yPanelSize, yButtonSize);
                    //Debug.Log($"Divisor {xDivisor}-{yDivisor}");
                    Debug.Log($"Panel Size {xPanelSize}-{yPanelSize}");
                    //Debug.Log($"Button Size {xButtonSize}-{yButtonSize}");
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
                //Debug.Log($"Range Key {rangeKey}");
                //Centerpoint instead of random
                float xPos =  Mathf.Lerp(xRanges[(int)rangeKey.x].minVal,xRanges[(int)rangeKey.x].maxVal,0.5f);
                float yPos =  Mathf.Lerp(yRanges[(int)rangeKey.y].minVal,yRanges[(int)rangeKey.y].maxVal,0.5f);
                // Random.Range(
                //     yRanges[(int)rangeKey.y].minVal + (yButtonSize/2)
                //     , yRanges[(int)rangeKey.y].maxVal - (yButtonSize/2));
                
                xPos = (xPos>xPanelSize/2) ? xPos/2 : xPos*-1;
                yPos = (yPos>yPanelSize/2) ? yPos/2 : yPos*-1;

                //Debug.Log($"Generated Position {xPos}-{yPos}");
                Vector3 spawnPosition = panelRect.TransformDirection(new Vector3(xPos, yPos, 0f));
                newButton.transform.localPosition = spawnPosition;
                
                //Debug.Log($"Button Position {newButton.transform.localPosition}");
                isPartitionOccupied[rangeKey]=true;
                        
            }
        }

        public int FindTightestDivisor(float toBeDivided, float minimumSize)
        {
            int divisor = 1;
            int safety=0;
            while (true)
            {
                safety+=1;
                if(safety>=100)
                {
                    Debug.Log("Safety Breached, Divisor"+divisor);
                    return divisor;
                }

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
            controlHelpPanel.SetActive(false);
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
            controlHelpPanel.SetActive(true);
            bgImage.color = initialColor;
            for(int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }

    }
}