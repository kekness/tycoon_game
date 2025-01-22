using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Structure : MonoBehaviour
{
    public string structureName;
    public float cost;
    public List<Vector2Int> coordinates = new List<Vector2Int>();
    public Vector2Int size = new Vector2Int(1, 1);
    public Sprite sprite;
}
