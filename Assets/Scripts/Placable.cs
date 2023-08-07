using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "[Placable]", menuName = "Placable")]
public class Placable : ScriptableObject
{
    [SerializeField]private GameObject prefab;
    public enum PlacableType{Platform,Obstacle,Hazard};
    [SerializeField]private PlacableType type;

    public GameObject GetPrefab(){return prefab;}
    public PlacableType GetPlacableType(){return type;}
}
