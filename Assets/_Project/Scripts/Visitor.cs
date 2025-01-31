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

    private bool isLeaving = false;
    public Tilemap tilemap;
    float gameTime;
    public float speed;
    private Queue<Vector3> pathPoints = new Queue<Vector3>();
    private bool isMoving;
    private Attraction currentAttraction;
    private float lastRecordedTime = 0f;
    private void Awake()
    {
        tilemap = AttractionPlacer.instance?.tilemap;
    }

    private void Start()
    {
        MoveToRandomAttraction();
    }

    private void Update()
    {
        speed = 2f * (ClockUI.instance?.getAcceleration() ?? 1f);
        gameTime = ClockUI.instance.GetGameTime();

        if (gameTime - lastRecordedTime >= 60f)
        {
            RecordCurrentPosition();
            lastRecordedTime = gameTime;
        }

        // Sprawdzenie, czy jest godzina 22:00 lub póŸniej
        if (gameTime / 3600f >= 22f)
        {
            Destroy(gameObject);
        }

        if (pathPoints.Count > 0 && !isMoving)
        {
            MoveToNextPoint();
        }
    }

    public Vector2Int GetCurrentGridPosition()
    {
        if (tilemap == null)
        {
            Debug.LogError("Tilemap is not assigned!");
            return new Vector2Int(-1, -1);
        }
        Vector3Int cellPosition = tilemap.WorldToCell(transform.position);
        return new Vector2Int(cellPosition.x, cellPosition.y);
    }

    public void MoveToRandomAttraction()
    {
        currentAttraction = Player.instance.GetRandomAttraction();
        if (currentAttraction != null)
        {
            Vector2Int target = currentAttraction.entrance.coordinates[0];
            MoveNPC(GetCurrentGridPosition(), target);
        }
        else
        {
            Debug.LogError("Brak atrakcji w grze!");
        }
    }
    private void RecordCurrentPosition()
    {
        Vector2Int currentGridPosition = GetCurrentGridPosition();

            HeatMapManager.instance.RecordData(currentGridPosition);
            Debug.Log($"Visitor recorded position: {currentGridPosition}");

    }

    public void MoveNPC(Vector2Int start, Vector2Int target)
    {
        Pathfinding pathfinding = new Pathfinding();
        List<Vector2Int> path = pathfinding.FindPath(start, target);
        if (path.Count > 0)
        {
            SetPath(path);
        }
        else
        {
            Debug.LogError("No path found to target!");
        }
    }
    public void Pay(float money)
    {
        Player.instance.getMoney(money);
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
            StartCoroutine(MoveToPosition(nextPoint));
        }
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        isMoving = true;
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
        isMoving = false;

        if (isLeaving && Player.instance.gate != null && GetCurrentGridPosition() == Player.instance.gate.coordinates[0])
        {
            Destroy(gameObject);
            yield break;
        }

        if (currentAttraction != null && GetCurrentGridPosition() == currentAttraction.entrance.coordinates[0])
        {
            StartCoroutine(VisitAttraction());
        }
        else
        {
            MoveToNextPoint();
        }
    }



    private IEnumerator VisitAttraction()
    {
        Debug.Log("Visitor is enjoying the attraction...");

        float targetGameTime = ClockUI.instance.GetGameTime() + (currentAttraction.timeRequired * 60f);
        currentAttraction.visit();
        Pay(currentAttraction.ticketCost);
        while (ClockUI.instance.GetGameTime() < targetGameTime)
        {
            yield return null; // Czekamy do momentu, a¿ gra osi¹gnie docelowy czas
        }

        // Po zakoñczeniu atrakcji teleportujemy do wyjœcia
        transform.position = tilemap.GetCellCenterWorld(new Vector3Int(
            currentAttraction.exit.coordinates[0].x,
            currentAttraction.exit.coordinates[0].y,
            0
        ));

        MoveToRandomAttraction(); // Wybieramy now¹ atrakcjê
    }


}