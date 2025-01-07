using UnityEngine;
using System.Collections.Generic;

public abstract class Building : MonoBehaviour
{
    public string buildingName;
    
    public int cost;
    public List<Vector2Int> coordinates = new List<Vector2Int>();
    public Vector2Int size = new Vector2Int(1, 1);
}
