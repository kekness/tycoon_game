using UnityEngine;
using UnityEngine.Tilemaps;

public class Grid_manager : MonoBehaviour
{
    public static Grid_manager Instance;
    public Tilemap tilemap; // Odniesienie do Tilemap
    private bool[,] gridOccupancy;

    public int gridWidth = 50;
    public int gridHeight = 50;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        gridOccupancy = new bool[gridWidth, gridHeight];
    }

    public bool IsSpaceAvailable(Vector2Int position, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3Int tilePosition = new Vector3Int(position.x + x, position.y + y, 0);
                Vector3Int cellPosition = tilemap.WorldToCell(tilemap.CellToWorld(tilePosition));

                // SprawdŸ czy wspó³rzêdne s¹ w granicach siatki
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

    public void OccupySpace(Vector2Int position, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3Int tilePosition = new Vector3Int(position.x + x, position.y + y, 0);
                Vector3Int cellPosition = tilemap.WorldToCell(tilemap.CellToWorld(tilePosition));
                gridOccupancy[cellPosition.x, cellPosition.y] = true;
            }
        }
    }
}
