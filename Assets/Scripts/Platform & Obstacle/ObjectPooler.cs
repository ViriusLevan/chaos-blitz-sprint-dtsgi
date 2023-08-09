using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [SerializeField] private GameObject shotyPrefab;
    [SerializeField] private int poolSize = 10;
    private List<GameObject> shotyPool = new List<GameObject>();

    private void Start()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject shoty = Instantiate(shotyPrefab);
            shoty.SetActive(false);
            shotyPool.Add(shoty);
        }
    }

    public GameObject GetPooled()
    {
        for (int i = 0; i < poolSize; i++)
        {
            if (!shotyPool[i].activeInHierarchy)
            {
                return shotyPool[i];
            }
        }

        return null;
    }
}
