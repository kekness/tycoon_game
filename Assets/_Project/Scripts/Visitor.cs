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
    private float gameTime;
    public float speed;
    private Queue<Vector2Int> pathPoints = new Queue<Vector2Int>();
    private bool isMoving;
    private Attraction currentAttraction;
    private QueuePath currentQueuePosition;
    private float lastRecordedTime = 0f;
    private bool isActive = true;

    private void Awake()
    { 
        tilemap = AttractionPlacer.instance?.tilemap;
    }

    private void Start()
    {
        ChooseRandomAttraction();
        currentAttraction.queuePaths[0].EnqueueVisitor(this);
        Deactivate();
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

    private void RecordCurrentPosition()
    {
        Vector2Int currentGridPosition = GetCurrentGridPosition();
        HeatMapManager.instance.RecordData(currentGridPosition);
    }

    public void ChooseRandomAttraction()
    {
        if (isLeaving) return;

        currentAttraction = Player.instance.GetRandomAttraction();

    }


    

 
    public void MoveNPC(Vector2Int start, Vector2Int target)
    {
        Pathfinding pathfinding = new Pathfinding();
        List<Vector2Int> path = pathfinding.FindPath(start, target);
        if (path.Count > 0)
        {
            SetPath(path);
        }
    }

    public void SetPath(List<Vector2Int> path)
    {
        StopCurrentPath(); // Przerwij obecn¹ œcie¿kê
        pathPoints.Clear();
        foreach (var tilePosition in path)
        {
            pathPoints.Enqueue(tilePosition);
        }
        if (pathPoints.Count > 0) MoveToNextPoint();
    }
    private void MoveToNextPoint()
    {
        if (pathPoints.Count > 0)
        {
            Vector2Int nextPoint = pathPoints.Dequeue();
            StartCoroutine(MoveToPosition(nextPoint));
        }
    }

    private IEnumerator MoveToPosition(Vector2Int targetPosition)
    {
        isMoving = true;
        Vector3 worldTarget = tilemap.GetCellCenterWorld(new Vector3Int(targetPosition.x, targetPosition.y, 0));

        while (Vector3.Distance(transform.position, worldTarget) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, worldTarget, speed * Time.deltaTime);
            yield return null;
        }
        isMoving = false;

        MoveToNextPoint();
    }
    public void StopCurrentPath()
    {
        StopAllCoroutines(); // Zatrzymaj wszystkie coroutines (w tym MoveToPosition)
        pathPoints.Clear();  // Wyczyœæ kolejkê œcie¿ki
        isMoving = false;    // Zresetuj flagê isMoving
    }
    public void MoveToExit()
    {
        isLeaving = true;
        Vector2Int exitPosition = Player.instance.gate.coordinates[0];
        MoveNPC(GetCurrentGridPosition(), exitPosition);
    }
    public void Activate()
    {
        this.isActive = true;
        Start();
    }
    public void Deactivate()
    {
        this.isActive = false;
    }
    public void Pay(float money)
    {
        Player.instance.getMoney(money);
    }
}
