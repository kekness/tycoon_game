using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Visitor : MonoBehaviour
{
    public int hunger;
    public int thirst;
    public int happiness;
    public int disgust;
    public int fear;
    public Tilemap tilemap;
    float gameTime;
    public float speed;
    private Queue<Vector3> pathPoints = new Queue<Vector3>();

    private void Awake()
    {
      
        tilemap = AttractionPlacer.instance?.tilemap;
    }
    private Vector3 lastPosition;
    private float stuckThreshold = 0.1f; // Odleg�o��, przy kt�rej uznajemy, �e NPC utkn��

    private void Update()
    {
        speed = 2f * (ClockUI.instance?.getAcceleration() ?? 1f);
        gameTime = ClockUI.instance.GetGameTime(); // Pobierz czas gry
        if (gameTime - lastRecordTime >= recordIntervalGameSeconds)
        {
            Vector2Int gridPos = GetCurrentGridPosition();
            if (gridPos.x != -1 && gridPos.y != -1)
            {
                HeatMapManager.instance.RecordData(gridPos);
            }
            lastRecordTime = gameTime;
        }
        // Sprawd�, czy NPC utkn�� w tym samym miejscu
        if (Vector3.Distance(lastPosition, transform.position) < stuckThreshold && pathPoints.Count > 0)
        {
            Debug.LogError("NPC is stuck, trying to re-evaluate path.");
            // Mo�na tu spr�bowa� ponownie obliczy� �cie�k� lub co� naprawi�
        }

        lastPosition = transform.position;

        // Kontynuuj ruch, je�li jest �cie�ka do przej�cia
        if (pathPoints.Count > 0 && !isMoving)
        {
            MoveToNextPoint();
        }
    }
    private float lastRecordTime = 0f;
    private float recordIntervalGameSeconds = 1f; // Co 60 sekund czasu gry

    public Vector2Int GetCurrentGridPosition()
    {
        if (tilemap == null)
        {
            Debug.LogError("Tilemap is not assigned!");
            return new Vector2Int(-1, -1);
        }

        // Konwersja pozycji �wiata na siatk�
        Vector3Int cellPosition = tilemap.WorldToCell(transform.position);
        return new Vector2Int(cellPosition.x, cellPosition.y);
    }

    private bool isMoving; // Flaga, �eby unikn�� nadmiernych wywo�a� MoveToNextPoint

    // Funkcja, aby NPC szed� do losowej atrakcji
    public void MoveToRandomAttraction()
    {
        // Losowanie atrakcji
        Attraction randomAttraction = Player.instance.GetRandomAttraction();

        if (randomAttraction != null)
        {
            Vector2Int target = randomAttraction.entrance.coordinates[0]; // Wej�cie atrakcji
            Debug.Log("Moving to target: " + target);

            // Ustal startow� i docelow� pozycj�
            Vector2Int start = GetCurrentGridPosition();
            MoveNPC(start, target);
            Debug.Log($"Visitor current grid position: {start}");
        }
        else
        {
            Debug.LogError("Brak atrakcji w grze!");
        }
    }

    public void MoveNPC(Vector2Int start, Vector2Int target)
    {
        Pathfinding pathfinding = new Pathfinding();
        List<Vector2Int> path = pathfinding.FindPath(start, target);

        if (path.Count > 0)
        {
            Debug.Log("Path found, moving NPC.");
            SetPath(path);
        }
        else
        {
            Debug.LogError("No path found to target!");
        }
    }


    public void SetPath(List<Vector2Int> path)
    {
        pathPoints.Clear();

        foreach (var tilePosition in path)
        {
            Vector3 worldPosition = tilemap.GetCellCenterWorld(new Vector3Int(tilePosition.x, tilePosition.y, 0));
            pathPoints.Enqueue(worldPosition);
        }

        if (pathPoints.Count > 0) MoveToNextPoint();
    }

    private void MoveToNextPoint()
    {
        if (pathPoints.Count > 0)
        {
            Vector3 nextPoint = pathPoints.Dequeue();
            Debug.Log("Moving to next point: " + nextPoint);
            StartCoroutine(MoveToPosition(nextPoint));
        }
    }


    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        isMoving = true;
        Debug.Log("Start moving towards: " + targetPosition);

        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }

        Debug.Log("Arrived at target: " + targetPosition);
        isMoving = false;
        MoveToNextPoint();
    }

}
