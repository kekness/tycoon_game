using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : BaseManager<PathManager>
{
    public bool[,] gridPath;

    public void Awake()
    {
        base.InitializeManager();
        gridPath = new bool[GridManager.instance.gridWidth, GridManager.instance.gridHeight];
    }

    public void registerPath(Vector2Int coordinates)
    {
        gridPath[coordinates[0], coordinates[1]] = true;
    }

    public void unRegisterPath(Vector2Int coordinates)
    {
        gridPath[coordinates[0], coordinates[1]] = false;
    }

    public void DisplayGridPath()
    {
        string gridDisplay = "Grid Path State:\n";
        for (int y = gridPath.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < gridPath.GetLength(0); x++)
                gridDisplay += gridPath[x, y] ? "1 " : "0 ";
            gridDisplay += "\n";
        }
        Debug.Log(gridDisplay);
    }

    public bool IsPathAt(Vector2Int coordinates)
    {
        if (coordinates.x >= 0 && coordinates.y >= 0 &&
            coordinates.x < gridPath.GetLength(0) && coordinates.y < gridPath.GetLength(1))
        {
            return gridPath[coordinates.x, coordinates.y];
        }
        return false;
    }


    public Vector2Int GiveMeOne()
    {
        List<Vector2Int> validPoints = new List<Vector2Int>();

        // Przejd� przez ca�� siatk� i zbierz punkty z warto�ci� true
        for (int x = 0; x < GridManager.instance.gridWidth - 1; x++)
        {
            for (int y = 0; y < GridManager.instance.gridHeight - 1; y++)
            {
                // Za��my, �e dost�p do siatki odbywa si� przez GridManager.instance.grid
                if (gridPath[x, y])
                {
                    validPoints.Add(new Vector2Int(x, y));
                }
            }
        }

        // Sprawd�, czy znaleziono jakiekolwiek punkty
        if (validPoints.Count == 0)
        {
            // Mo�esz rzuci� wyj�tkiem lub zwr�ci� domy�ln� warto��
            throw new System.InvalidOperationException("Brak punkt�w true w siatce.");
        }

        // Wylosuj i zwr�� losowy punkt
        int randomIndex = UnityEngine.Random.Range(0, validPoints.Count);

        Vector2Int result = validPoints[randomIndex];
        return result;
    }
}