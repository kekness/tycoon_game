using System.Collections.Generic;
using UnityEngine;

public class HeatMapManager : BaseManager<HeatMapManager>
{
    private Dictionary<Vector2Int, int> heatData = new Dictionary<Vector2Int, int>();

    public void Awake()
    {
        base.InitializeManager();
    }

    public void RecordData(Vector2Int coordinates)
    {
        if (heatData.ContainsKey(coordinates))
        {
            heatData[coordinates]++;
        }
        else
        {
            heatData[coordinates] = 1;
        }
    }

    public Dictionary<Vector2Int, int> GetHeatData()
    {
        return heatData;
    }

    public void ResetHeatData()
    {
        heatData.Clear();
    }
}