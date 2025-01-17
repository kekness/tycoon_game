using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Tilemap tilemap; 
    public int mapWidth = 20;       
    public int mapHeight = 20;       
    public TileBase groundTile;
    public GameObject forestPrefab;
    public GameObject riverPrefab;


    private int[,] mapGrid;         
    private Vector2Int riverStart;  

    void Start()
    {
        GenerateMap();
        GenerateForests();
        GenerateRiver();
        RenderMap();
    }
    void GenerateMap()
    {
        mapGrid = new int[mapWidth, mapHeight];
     
        for (int x = 0; x < mapWidth; x++)
            for (int y = 0; y < mapHeight; y++)
                mapGrid[x, y] = 0;
        
        riverStart = new Vector2Int(0, Random.Range(0, mapHeight));
    }
    void GenerateRiver()//random walk
    {
        Vector2Int currentPosition = riverStart;

        while (currentPosition.x < mapWidth-1)
        {
          
            mapGrid[currentPosition.x, currentPosition.y] = 1;
            GridManager.instance.OccupySpace(currentPosition, new Vector2Int(1, 1));

            int direction = Random.Range(0, 3);

            if (direction == 0) 
            {
                currentPosition.x++;
            }
            else if (direction == 1 && currentPosition.y < mapHeight - 1) 
            {
                currentPosition.y++;
            }
            else if (direction == 2 && currentPosition.y > 0) 
            {
                currentPosition.y--;
            }

            currentPosition.x = Mathf.Clamp(currentPosition.x, 0, mapWidth - 1);
            currentPosition.y = Mathf.Clamp(currentPosition.y, 0, mapHeight - 1);
        }
    }
    void GenerateForests()
    {
        List<RectInt> partitions = BSPPartition(new RectInt(0, 0, mapWidth, mapHeight), 4);

        foreach (var partition in partitions)
        {
            if (Random.value < 0.5f) 
            {
                CreateNaturalForest(partition);
            }
        }
    }

    void CreateNaturalForest(RectInt partition)
    {
      
        Vector2 center = new Vector2(
            partition.xMin + partition.width / 2,
            partition.yMin + partition.height / 2
        );

        float maxRadius = Mathf.Min(partition.width, partition.height) / 2f;

        for (int x = partition.xMin; x < partition.xMax; x++)
        {
            for (int y = partition.yMin; y < partition.yMax; y++)
            {
             
                float distance = Vector2.Distance(center, new Vector2(x, y));

                if (distance < maxRadius * (0.8f + Random.value * 0.4f)) 
                {
                    if (mapGrid[x, y] == 0)
                    {
                        mapGrid[x, y] = 2;
                        GridManager.instance.OccupySpace(new Vector2Int(x, y), new Vector2Int(1, 1));
                    }
                        
                    
                }
            }
        }
    }

    List<RectInt> BSPPartition(RectInt area, int depth)
    {
        List<RectInt> partitions = new List<RectInt>();
        Queue<RectInt> queue = new Queue<RectInt>();
        queue.Enqueue(area);

        for (int i = 0; i < depth; i++)
        {
            int count = queue.Count;
            for (int j = 0; j < count; j++)
            {
                RectInt current = queue.Dequeue();
                if (current.width > 4 && current.height > 4)
                {
                    bool splitHorizontally = Random.value > 0.5f;

                    if (splitHorizontally && current.height > 8 || current.width <= 8)
                    {
                        int split = Random.Range(1, current.height - 1);
                        queue.Enqueue(new RectInt(current.x, current.y, current.width, split));
                        queue.Enqueue(new RectInt(current.x, current.y + split, current.width, current.height - split));
                    }
                    else
                    {
                        int split = Random.Range(1, current.width - 1);
                        queue.Enqueue(new RectInt(current.x, current.y, split, current.height));
                        queue.Enqueue(new RectInt(current.x + split, current.y, current.width - split, current.height));
                    }
                }
                else
                {
                   
                    partitions.Add(current);
                }
            }
        }

        while (queue.Count > 0)
        {
            partitions.Add(queue.Dequeue());
        }

        return partitions;
    }

    void RenderMap()
    {
        tilemap.ClearAllTiles();
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                Vector3 worldPosition = tilemap.GetCellCenterWorld(tilePosition);

                if (mapGrid[x, y] == 1)
                {
                    tilemap.SetTile(tilePosition, groundTile);
                    GameObject riverObject = Instantiate(riverPrefab, worldPosition, Quaternion.identity);
                    River river = riverObject.GetComponent<River>();
                    river.coordinates.Add(new Vector2Int(x, y));
                    GridManager.instance.OccupySpace(new Vector2Int(x, y), new Vector2Int(1, 1), river);
                }
                else if (mapGrid[x, y] == 2)
                {
                    tilemap.SetTile(tilePosition, groundTile);
                    GameObject forestObject = Instantiate(forestPrefab, worldPosition, Quaternion.identity);
                    Forest forest = forestObject.GetComponent<Forest>();
                    forest.coordinates.Add(new Vector2Int(x, y));
                    GridManager.instance.OccupySpace(new Vector2Int(x, y), new Vector2Int(1, 1), forest);
                }
                else
                {
                    tilemap.SetTile(tilePosition, groundTile);
                }
            }
        }
    }


}
