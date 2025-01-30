using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : Path
{
    public River underlyingRiver;

    public Bridge(string name, int baseCost, River river) : base(name, baseCost)
    {
        this.underlyingRiver = river;
        this.cost = baseCost + 50;
    }
    public void OnPrefabDestroy()
    {
        underlyingRiver.isBridged = false;
        GridManager.instance.OccupySpace(underlyingRiver.coordinates[0], underlyingRiver.size);
    }
}

