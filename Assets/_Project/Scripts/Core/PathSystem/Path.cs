using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : Structure
{

    private void Start()
    {
        PathManager.instance.registerPath(coordinates[0]);
    }
    public Path(string name, int cost)
    {
        this.structureName = name;
        this.cost = cost;
    }
    private void OnDestroy()
    {
        // Dodaj warunek sprawdzaj¹cy czy manager jeszcze istnieje
        if (PathManager.instance != null)
        {
            PathManager.instance.unRegisterPath(coordinates[0]);
        }
    }

}
