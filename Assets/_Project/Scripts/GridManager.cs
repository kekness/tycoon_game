using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : BaseManager<GridManager>
{
    public Tilemap tilemap; // Odniesienie do Tilemap
    private bool[,] gridOccupancy;
    public Dictionary<Vector2Int, Structure> structures; // S³ownik przechowuj¹cy struktury na siatce

    public int gridWidth = 50;
    public int gridHeight = 50;

    public void Awake()
    {
        base.InitializeManager();

        gridOccupancy = new bool[gridWidth, gridHeight];
        structures = new Dictionary<Vector2Int, Structure>();
    }

    public bool IsSpaceAvailable(Vector2Int position, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3Int tilePosition = new Vector3Int(position.x + x, position.y + y, 0);
                Vector3Int cellPosition = tilemap.WorldToCell(tilemap.CellToWorld(tilePosition));

                if (cellPosition.x < 0 || cellPosition.y < 0 ||
                    cellPosition.x >= gridWidth || cellPosition.y >= gridHeight ||
                    gridOccupancy[cellPosition.x, cellPosition.y])
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void ReleaseSpace(List<Vector2Int> coordinates)
    {
        foreach (Vector2Int coordinate in coordinates)
        {
            Vector3Int cellPosition = tilemap.WorldToCell(tilemap.CellToWorld(new Vector3Int(coordinate.x, coordinate.y, 0)));

            if (cellPosition.x >= 0 && cellPosition.y >= 0 &&
                cellPosition.x < gridWidth && cellPosition.y < gridHeight)
            {
                gridOccupancy[cellPosition.x, cellPosition.y] = false;
                structures.Remove(coordinate); // Usuñ strukturê z siatki
            }
        }
    }

    public void OccupySpace(Vector2Int position, Vector2Int size, Structure structure = null)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3Int tilePosition = new Vector3Int(position.x + x, position.y + y, 0);
                Vector3Int cellPosition = tilemap.WorldToCell(tilemap.CellToWorld(tilePosition));
                gridOccupancy[cellPosition.x, cellPosition.y] = true;

                if (structure != null)
                {
                    structures[new Vector2Int(position.x + x, position.y + y)] = structure; // Zapisz strukturê
                }
            }
        }
    }

    public T GetStructureAt<T>(Vector2Int position) where T : Structure
    {
        if (structures.TryGetValue(position, out Structure structure))
        {
            return structure as T; // Rzutowanie na typ T
        }
        return null; // Nie znaleziono struktury
    }
    public Vector2Int GetCurrentGridPosition()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = 0;
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
        return new Vector2Int(cellPosition.x, cellPosition.y);
    }
}
