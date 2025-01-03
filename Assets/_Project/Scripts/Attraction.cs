using UnityEngine;
using System.Collections.Generic;

public class Attraction : MonoBehaviour
{
    public string attractionName;  
    public Vector2Int size = new Vector2Int(2, 2); //grid size
    public int cost;
    public int ticketCost;
    public List<Vector2Int> coordinates = new List<Vector2Int>();


    public ExitEntry entrance;
    public ExitEntry exit;

}
