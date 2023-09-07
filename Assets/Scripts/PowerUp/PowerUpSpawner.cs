using System.Collections.Generic;
using UnityEngine;

namespace LevelUpStudio.ChaosBlitzSprint.PowerUp
{
    public class PowerUpSpawner : MonoBehaviour
    {
        public Transform arenaTopLeft, arenaBottomRight, arenaGroundTransform;
        public GameObject[] powerUps;
        public List<FloatRange> xRangeList, zRangeList;
        public int xDivisor=5, zDivisor=5;
        private float xSize,zSize;
        void Start()
        {

    //TODO I'm assuming a square arena here, also double check transform xz because this is sequence sensitive
            xSize = Mathf.Abs(arenaTopLeft.position.x-arenaBottomRight.position.x);
            zSize = Mathf.Abs(arenaTopLeft.position.z-arenaBottomRight.position.z);
            Debug.Log($"Arena Size {xSize}-{zSize}");
            //Margins
            xSize -=5f;
            zSize -=5f;

            // xRangeList = GetRangeList(xSize,xDivisor);
            // zRangeList = GetRangeList(zSize,zDivisor);
        }

        private void OnEnable() 
        {
            GameManager.platformingPhaseBegin += SpawnPowerUps;
            GameManager.platformingPhaseFinished += DestroyPowerUps;
        }

        private void OnDisable() 
        {
            GameManager.platformingPhaseBegin -= SpawnPowerUps;
            GameManager.platformingPhaseFinished -= DestroyPowerUps;
        }

        [SerializeField]private Transform[] spawnPoints;
        public void SpawnPowerUps()
        {
            foreach (Transform point in spawnPoints)
            {
                int index = Random.Range(0, powerUps.Length);
                GameObject prefabInstance = Instantiate
                    (powerUps[index],point.position,Quaternion.identity);
                prefabInstance.transform.SetParent(transform);
            }
        }

        public void DestroyPowerUps()
        {
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
        }

        // [SerializeField]private int nOfPowerUpsToSpawn=2;
        // public void SpawnPowerUps()
        // {
        //     Dictionary<Vector2,bool> isPartitionOccupied = new Dictionary<Vector2, bool>();

        //     for (int i1 = 0; i1 < xDivisor; i1++)
        //     {
        //         for (int i2 = 0; i2 < zDivisor; i2++)
        //         {
        //             isPartitionOccupied.Add(
        //                 new Vector2(i1,i2),false
        //             );
        //         }
        //     }

        //     for (int i = 0; i < nOfPowerUpsToSpawn; i++)
        //     {
        //         int index = Random.Range(0, powerUps.Length);
        //         GameObject prefabInstance = Instantiate(powerUps[index],new Vector3(),Quaternion.identity);

        //         Vector2 rangeKey;
        //         int safetyCounter=0;
        //         do{
        //             int xRangeIndex = Random.Range(0,xDivisor);
        //             int yRangeIndex = Random.Range(0,zDivisor);
        //             rangeKey = new Vector2(xRangeIndex,yRangeIndex);

        //             safetyCounter+=1;
        //             if(safetyCounter>100)break;
        //         }while(isPartitionOccupied[rangeKey]);

        //         float xPos = Random.Range(
        //             xRangeList[(int)rangeKey.x].minVal
        //             , xRangeList[(int)rangeKey.x].maxVal);
        //         float zPos = Random.Range(
        //             zRangeList[(int)rangeKey.y].minVal
        //             , zRangeList[(int)rangeKey.y].maxVal);

                
        //         xPos = (xPos>xSize/2) ? xPos/2 : xPos*-1;
        //         zPos = (zPos>zSize/2) ? zPos/2 : zPos*-1;
        //         Debug.Log($"Generated Position {xPos}-{zPos}");

        //         prefabInstance.transform.position = new Vector3(xPos,10,zPos);
        //         prefabInstance.transform.SetParent(arenaGroundTransform);
        //         isPartitionOccupied[rangeKey]=true;
        //     }

        // }

        // public List<FloatRange> GetRangeList(float toBeDivided, int divisor)
        // {
        //     float interval = toBeDivided/divisor;
        //     float counter = 0;
        //     List<FloatRange> rangeList = new List<FloatRange>();
        //     for (int i = 0; i < divisor; i++)
        //     {
        //         FloatRange floatRange = new FloatRange(counter, counter+interval);
        //         if(floatRange.maxVal>toBeDivided)
        //             floatRange.maxVal = toBeDivided;

        //         rangeList.Add(floatRange);
        //         counter+=interval;
        //     }
        //     return rangeList;
        // }
    }
}