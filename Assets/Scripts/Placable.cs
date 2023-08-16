using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "[Placable]", menuName = "Placable")]
public class Placable : ScriptableObject
{
    [SerializeField]private string placableName;
    [SerializeField]private GameObject prefab;
    public enum PlacableType{Platform,Obstacle,Hazard};
    [SerializeField]private PlacableType type;

    [SerializeField]private Sprite sprite;

    public string GetName(){return name;}
    public GameObject GetPrefab(){return prefab;}
    public PlacableType GetPlacableType(){return type;}
    public Sprite GetSprite(){return sprite;}
}
