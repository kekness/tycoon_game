using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int target)
    {
        Debug.Log($"Finding path from {start} to {target}");
        if (!IsPositionValid(start) || !IsPositionValid(target))
        {
            Debug.LogError("Start or target position is invalid!");
            return new List<Vector2Int>();
        }

        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        Node startNode = new Node(start);
        Node targetNode = new Node(target);

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = openList.OrderBy(node => node.FCost).First();

            if (currentNode.Position == targetNode.Position)
            {
                Debug.Log("Path found!");
                return RetracePath(startNode, currentNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (var neighborPosition in GetNeighbors(currentNode.Position))
            {
                if (!PathManager.instance.IsPathAt(neighborPosition) || closedList.Any(node => node.Position == neighborPosition))
                {
                    Debug.Log($"Neighbor {neighborPosition} is not a valid path or already closed.");
                    continue;
                }

                Node neighborNode = new Node(neighborPosition)
                {
                    Parent = currentNode,
                    GCost = currentNode.GCost + 1,
                    HCost = Heuristic(neighborPosition, targetNode.Position)
                };

                if (openList.Any(node => node.Position == neighborPosition && node.FCost <= neighborNode.FCost))
                {
                    Debug.Log($"Neighbor {neighborPosition} is already in open list with a better FCost.");
                    continue;
                }

                openList.Add(neighborNode);
                Debug.Log($"Added neighbor {neighborPosition} to open list.");
            }
        }

        Debug.LogError("No path found to target!");
        return new List<Vector2Int>(); // Brak œcie¿ki
    }
    private bool IsPositionValid(Vector2Int position)
    {
        return position.x >= 0 && position.y >= 0 &&
               position.x < PathManager.instance.gridPath.GetLength(0) &&
               position.y < PathManager.instance.gridPath.GetLength(1);
    }
    private int Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private List<Vector2Int> GetNeighbors(Vector2Int position)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>
        {
            new Vector2Int(position.x + 1, position.y),
            new Vector2Int(position.x - 1, position.y),
            new Vector2Int(position.x, position.y + 1),
            new Vector2Int(position.x, position.y - 1)
        };

        return neighbors.Where(pos => pos.x >= 0 && pos.y >= 0 &&
                                       pos.x < PathManager.instance.gridPath.GetLength(0) &&
                                       pos.y < PathManager.instance.gridPath.GetLength(1)).ToList();
    }

    private List<Vector2Int> RetracePath(Node startNode, Node endNode)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.Position);
            currentNode = currentNode.Parent;
        }

        path.Reverse();
        return path;
    }
}
