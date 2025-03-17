/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public enum VisitorStates1
{
    BOIDS,
    EXITING,
    SEEKFOOD,
    A,
    INACTIVE
}

public class VisitorHelp1 : MonoBehaviour
{

    [Header("Decision Settings")]
    [SerializeField] private float satisfactionWeight = 0.7f;    // Waga satysfakcji w podejmowaniu decyzji
    [SerializeField] private float queueWeight = 0.3f;           // Waga kolejki w podejmowaniu decyzji
    [SerializeField] private float maxQueueConsidered = 20f;

    //BOIDS PARAMETERS
    [SerializeField] private float separation = .5f;
    [SerializeField] private float aligment = .5f;
    [SerializeField] private float cohasion = .5f;
    [SerializeField] private float maxSpeed = 3f;
    [SerializeField] private float maxForce = 1.5f;
    [SerializeField] private List<Visitor> neighbors = new List<Visitor>();
    [SerializeField] private float neighborRadius = 1.5f;

    //VISITOR STATS

    [SerializeField] public int hunger = 0;
    [SerializeField] public int happiness = 50;
    [SerializeField] public int fear = 0;
    [SerializeField] public float money = 100;
    [SerializeField] public Attraction bestAttraction = null;

    public VisitorStates currenrState;

    private bool isLeaving = false;
    public Tilemap tilemap;
    private float gameTime;
    public float speed;
    private Queue<Vector2Int> pathPoints = new Queue<Vector2Int>();
    private bool isMoving;
    private Attraction currentAttraction;
    private Vector2Int currentPoint;
    private QueuePath currentQueuePosition;
    private float lastRecordedTime = 0f;
    private bool isActive = true;

    private void Awake()
    {
        tilemap = AttractionPlacer.instance?.tilemap;
        currenrState = VisitorStates.BOIDS;
    }

    private void Start()
    {
        ChooseBestAttraction();
    }
    private float ChooseBestAttraction()
    {
        List<Attraction> availableAttractions = GetAvailableAttractions();

        if (availableAttractions.Count == 0)
        {
            ChangeState(VisitorStates.EXITING);
            return 0;
        }
        float bestScore = -Mathf.Infinity;

        foreach (Attraction attraction in availableAttractions)
        {
            float score = CalculateAttractionScore(attraction);

            if (score > bestScore)
            {
                bestScore = score;
                bestAttraction = attraction;
            }
        }

        if (bestAttraction != null)
        {
            SetAttraction(bestAttraction);
        }
        else
        {
            ChangeState(VisitorStates.EXITING);
        }

        return bestScore;
    }

    private float CalculateAttractionScore(Attraction attraction)
    {
        // Normalizacja wartoœci
        float normalizedSatisfaction = attraction.GetSatisfactionScore() / attraction.maxSatisfaction;
        float normalizedQueue = 1 - Mathf.Clamp01(attraction.visitorsInQueue / maxQueueConsidered);

        // Obliczanie koñcowego wyniku z wagami
        return (normalizedSatisfaction * satisfactionWeight) + (normalizedQueue * queueWeight);
    }

    private void SetAttraction(Attraction attraction)
    {
        currentAttraction = attraction;
        currentAttraction.queuePaths[0].EnqueueVisitor(this);
        MoveNPC(GetCurrentGridPosition(), currentAttraction.entrance.coordinates[0]);
    }


    private List<Attraction> GetAvailableAttractions()
    {
        List<Attraction> result = new List<Attraction>();
        foreach (Structure structure in Player.instance.attractionList)
        {
            if (structure is Attraction attraction && attraction.isOpen && !attraction.isBroken)
            {
                result.Add(attraction);
            }
        }
        return result;
    }

    public void ChangeState(VisitorStates state)
    {
        this.currenrState = state;
    }

    private bool FindShop()
    {
        Dictionary<Vector2Int, Structure> structures = GridManager.instance.structures;
        foreach (Structure str in structures.Values)
        {
            if (str is Shop shop)
                return true;
        }
        return false;
    }

    private void Update()
    {
        speed = 2f * (ClockUI.instance?.getAcceleration() ?? 2f);
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
        if (isActive)
        {
            UpdateStats();
            HandleState();
        }

    }

    private void HandleState()
    {
        switch (currenrState)
        {
            case VisitorStates.BOIDS:
                ApplyBoids();
                break;

            case VisitorStates.SEEKFOOD:
                if (FindShop(out Shop targetShop))
                    MoveNPC(GetCurrentGridPosition(), targetShop.coordinates[0]);
                else
                    ChangeState(VisitorStates.EXITING);
                break;

            case VisitorStates.EXITING:
                MoveToExit();
                break;
            case VisitorStates.A:
                if (pathPoints.Count > 0 && !isMoving)
                {
                    MoveToNextPoint();
                }
                break;

        }
    }

    private bool FindShop(out Shop foundShop)
    {
        Dictionary<Vector2Int, Structure> structures = GridManager.instance.structures;
        foreach (Structure str in structures.Values)
        {
            if (str is Shop shop)
            {
                foundShop = shop;
                return true;
            }
        }
        foundShop = null;
        return false;
    }

    private void WanderAround()
    {
        if (!isMoving && pathPoints.Count == 0)
        {
            ChooseBestAttraction();
        }
    }
    private void UpdateStats()
    {
        float timeScale = ClockUI.instance?.getAcceleration() ?? 1f;

        // G³ód roœnie w zale¿noœci od czasu (np. o 1 co 30 sekund rzeczywistego czasu gry)
        hunger += Mathf.FloorToInt(timeScale * Time.deltaTime * 2f); // Mo¿esz dostosowaæ mno¿nik

        // Ograniczenie g³odu do maksymalnej wartoœci
        hunger = Mathf.Clamp(hunger, 0, 100);
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
        else
        {
            this.Deactivate();
        }

    }

    private IEnumerator MoveToPosition(Vector2Int targetPosition)
    {
        System.Random random = new System.Random();
        isMoving = true;
        Vector3 worldTarget = tilemap.GetCellCenterWorld(new Vector3Int(targetPosition.x, targetPosition.y, 0));

        worldTarget.x += (float)random.NextDouble() - .1f;
        worldTarget.y += (float)random.NextDouble() - .1f;

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
        Vector2Int exitPosition = Player.instance.gates[0].coordinates[0];
        MoveNPC(GetCurrentGridPosition(), exitPosition);

    }
    public void Activate()
    {
        this.isActive = true;
    }
    public void Deactivate()
    {
        this.isActive = false;
    }
    public void Pay(float money)
    {
        this.money -= money;
        Player.instance.getMoney(money);
    }
    public void Eat(int food)
    {
        this.hunger = Mathf.Max(hunger -= food, 0);
    }


    #region - BOIDS
    private void FindNeighbors()
    {
        neighbors.Clear();
        Visitor[] allVisitors = FindObjectsOfType<Visitor>();

        foreach (Visitor visitor in allVisitors)
        {
            if (visitor != this) // Nie dodawaj samego siebie
            {
                float distance = Vector3.Distance(transform.position, visitor.transform.position);
                if (distance < neighborRadius)
                {
                    neighbors.Add(visitor);
                }
            }
        }
    }

    private Vector3 CalculateSeparation()
    {
        Vector3 separationForce = Vector3.zero;
        int count = 0;

        foreach (Visitor neighbor in neighbors)
        {
            float distance = Vector3.Distance(transform.position, neighbor.transform.position);
            if (distance < separation)
            {
                Vector3 away = transform.position - neighbor.transform.position;
                separationForce += away.normalized / distance;
                count++;
            }
        }

        if (count > 0)
        {
            separationForce /= count;
        }

        return separationForce.normalized * maxForce;
    }

    private Vector3 CalculateAlignment()
    {
        Vector2 averageVelocity = Vector2.zero;
        int count = 0;

        foreach (Visitor neighbor in neighbors)
        {
            averageVelocity += neighbor.GetComponent<Rigidbody2D>().velocity;
            count++;
        }

        if (count > 0)
        {
            averageVelocity /= count;
            averageVelocity = averageVelocity.normalized * maxSpeed;
        }

        return (averageVelocity - GetComponent<Rigidbody2D>().velocity).normalized * maxForce;
    }

    private Vector3 CalculateCohesion()
    {
        Vector3 centerOfMass = Vector3.zero;
        int count = 0;

        foreach (Visitor neighbor in neighbors)
        {
            centerOfMass += neighbor.transform.position;
            count++;
        }

        if (count > 0)
        {
            centerOfMass /= count;
            Vector3 directionToCenter = (centerOfMass - transform.position).normalized * maxForce;
            return directionToCenter;
        }

        return Vector3.zero;
    }


    private void ApplyBoids()
    {
        FindNeighbors(); // ZnajdŸ s¹siadów

        Vector2 separationForce = CalculateSeparation() * separation;
        Vector2 alignmentForce = CalculateAlignment() * aligment;
        Vector3 cohesionForce = CalculateCohesion() * cohasion;

        Vector3 boidsForce = (Vector3)separationForce + (Vector3)alignmentForce + (Vector3)cohesionForce;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.ClampMagnitude((Vector3)rb.velocity + boidsForce, maxSpeed);

        if (ChooseBestAttraction() > .5f)
        {
            ChangeState(VisitorStates.A);
            MoveNPC(GetCurrentGridPosition(), this.bestAttraction.coordinates[0]);
        }
    }

    #endregion
}
*/