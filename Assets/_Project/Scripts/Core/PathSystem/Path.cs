using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : Structure
{

    public List<Vector3Int> points = new List<Vector3Int>();

    public Path(string name, float cost)
    {
        this.structureName = name;
        this.cost = cost;
    }

    public void AddPoint(Vector3Int point)
    {
        points.Add(point);
    }

    public void RemovePoint(Vector3Int point)
    {
        points.Remove(point);
    }

}
