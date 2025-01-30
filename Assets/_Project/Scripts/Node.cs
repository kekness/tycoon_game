using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector2Int Position;
    public Node Parent;
    public int GCost; // koszt od startu
    public int HCost; // koszt heurystyki do celu
    public int FCost => GCost + HCost;

    public Node(Vector2Int position)
    {
        Position = position;
    }
}

