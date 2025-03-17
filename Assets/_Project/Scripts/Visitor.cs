using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum VisitorStates
{
    BOIDS,
    EXITING,
    TARGETING,
    IDLE
}

public class Visitor : MonoBehaviour
{

    public System.Random rand = new System.Random();

    [Header("Personality Settings")]
    [SerializeField] private float separation = 1f;
    [SerializeField] private float alignment = .3f;
    [SerializeField] private float cohesion = .3f;
    [SerializeField] private float maxSpeed = 2f;
    [SerializeField] private float maxForce = 1.5f;
    [SerializeField] private List<Visitor> neighbors = new List<Visitor>();
    [SerializeField] private float neighborRadius = 1.5f;
    [SerializeField] private float fovAngle = 120f;
    [SerializeField] private float fovDistance = 5f;
    [SerializeField] private int money;
    [SerializeField] private int gunger = 0;

    public float gameTime;
    public Rigidbody2D rb;


    [SerializeField] public Attraction favoriteAttraction;
    [SerializeField] public Attraction targetAttraction;
    public Queue<Vector2Int> pathPoints = new Queue<Vector2Int>();

    [Header("STATE")]
    [SerializeField] public VisitorStates currentState;
    private VisitorStates previousState; // Œledzenie poprzedniego stanu
    private float timeUntilCanTarget = 0f;

    private float lastRecordedTime = 0f;

    #region - References -
    private Tilemap tilemap;
    #endregion

    private void RecordCurrentPosition()
    {
        Vector2Int currentGridPosition = GetCurrentGridPosition();
        HeatMapManager.instance.RecordData(currentGridPosition);
    }

    private void Awake() => tilemap = AttractionPlacer.instance?.tilemap;

    private void Start()
    {
        money = rand.Next(500, 1001);
        rb = GetComponent<Rigidbody2D>();
        this.currentState = VisitorStates.BOIDS;
        while (true)
        {
            favoriteAttraction = Player.instance.GetRandomAttraction();
            if (favoriteAttraction.isOpen)
                break;
        }
    }

    private void Update()
    {
        previousState = currentState;

        UpdateStats();
        HandleStates();

        gameTime = ClockUI.instance.GetGameTime();
        maxSpeed = 2f * (ClockUI.instance?.getAcceleration() ?? 2f);

        if (gameTime - lastRecordedTime >= 60f)
        {
            RecordCurrentPosition();
            lastRecordedTime = gameTime;
        }

        if (gameTime / 3600f >= 22f)
        {
            Destroy(gameObject);
        }

        timeUntilCanTarget -= Time.deltaTime; // Aktualizuj cooldown

        // Jeœli opuœciliœmy stan TARGETING, ustaw cooldown i wyczyœæ cel
        if (previousState == VisitorStates.TARGETING && currentState != VisitorStates.TARGETING)
        {
            timeUntilCanTarget = 60f; // Np. 60 sekund cooldown
            targetAttraction = null;
        }

    }

    public void HandleStates()
    {

        switch (currentState)
        {
            case VisitorStates.BOIDS:
                FindNeighbors();
                ApplyBoidsBehavior();
                break;

            case VisitorStates.TARGETING:
                rb.velocity = new Vector2(0, 0);
                if (this.gunger >= 70 && FindShop())
                    ChooseRandomShop();
                else if (this.gunger >= 70)
                    ChooseRandomExit();
                else if (this.gunger < 70)
                    targetAttraction.visit(this);

                if (this.money < 50)
                    ChooseRandomExit();
                break;

            case VisitorStates.IDLE:
                rb.velocity = new Vector2(0,0);
                break;
        }
    }

    private void ChooseRandomExit()
    {
        Gate gate = Player.instance.GetRandomExit();
        if (gate != null)
            gate.visit(this);
        else
            currentState = VisitorStates.IDLE; 
    }

    private void ChooseRandomShop()
    {
        Shop shop = Player.instance.GetRandomShop();
        Debug.LogError(shop.coordinates + "XDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD");
        if (shop != null)
            shop.visit(this);
        else
            ChooseRandomExit();
        
            
    }

    #region - BOIDS -
    private void ApplyBoidsBehavior()
    {
        // Obliczanie si³ BOIDS
        Vector2 separationForce = CalculateSeparation() * separation;
        Vector2 alignmentForce = CalculateAlignment() * alignment;
        Vector3 cohesionForce = CalculateCohesion() * cohesion;
        Vector3 totalForce = (Vector3)(separationForce + alignmentForce) + cohesionForce;

        // Jeœli nie ma ¿adnych visitorów w pobli¿u, poruszaj siê domyœlnie (np. w kierunku transform.up)
        if (neighbors.Count == 0)
        {
            double xd = rand.NextDouble();
            if(xd < .25)
                rb.velocity = transform.up * maxSpeed;
            else if (xd < .5)
                rb.velocity = transform.right * maxSpeed;
            else if (xd < .75)
                rb.velocity = -transform.up * maxSpeed;
            else if (xd < 1)
                rb.velocity = -transform.right * maxSpeed;
        }
        else
        {
            rb.velocity = Vector2.ClampMagnitude(rb.velocity + (Vector2)totalForce, maxSpeed);
        }

        SnapToPathIfNeeded();

        if (timeUntilCanTarget <= 0f && GetBestAttractionBasedOnProbability() > .5f)
        {
            this.currentState = VisitorStates.TARGETING;

        }

        if (this.gunger >= 70)
            this.currentState = VisitorStates.TARGETING;
            
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
    #endregion

    #region - Œcie¿ki i pomocnicze funkcje gridu -
    private void SnapToPathIfNeeded()
    {
        Vector2Int gridPos = GetCurrentGridPosition();

        if (!IsCellWalkable(gridPos))
        {
            Vector3 nearestPathPos = FindNearestWalkablePosition();
            transform.position = Vector3.MoveTowards(
                transform.position,
                nearestPathPos,
                maxSpeed * Time.deltaTime * 2f
            );
        }
    }

    private Vector3 FindNearestWalkablePosition()
    {
        Vector2Int currentPos = GetCurrentGridPosition();

        for (int radius = 1; radius <= 10; radius++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    Vector2Int checkPos = currentPos + new Vector2Int(x, y);
                    if (IsCellWalkable(checkPos))
                    {
                        return tilemap.GetCellCenterWorld((Vector3Int)checkPos);
                    }
                }
            }
        }

        return transform.position;
    }

    private bool IsCellWalkable(Vector2Int gridPos)
    {
        if (!IsWithinBounds(gridPos)) return false;
        return PathManager.instance.gridPath[gridPos.x, gridPos.y];
    }

    private bool IsWithinBounds(Vector2Int position)
    {
        bool[,] grid = PathManager.instance.gridPath;
        return position.x >= 0 && position.x < grid.GetLength(0) &&
               position.y >= 0 && position.y < grid.GetLength(1);
    }

    public Vector2Int GetCurrentGridPosition()
    {
        return (Vector2Int)tilemap.WorldToCell(transform.position);
    }
    #endregion

    #region - Znajdowanie s¹siadów -
    private void FindNeighbors()
    {
        neighbors.Clear();
        Visitor[] allVisitors = FindObjectsOfType<Visitor>();

        foreach (Visitor visitor in allVisitors)
        {
            if (visitor != this)
            {
                float distance = Vector3.Distance(transform.position, visitor.transform.position);
                if (distance < neighborRadius)
                {
                    neighbors.Add(visitor);
                }
            }
        }
    }
    #endregion

    #region - Decision Making -

    private List<Attraction> GetValidAttractions()
    {
        List<Attraction> attractions = new List<Attraction>();
        foreach (Structure structure in Player.instance.attractionList)
        {
            if (structure is Attraction a && a.isOpen && !a.isBroken)
            {
                attractions.Add(a);
            }
        }
        return attractions;
    }
    private float GetBestAttractionBasedOnProbability()
    {
        List<Attraction> validAttractions = GetValidAttractions();
        Attraction bestAttraction = null;
        float bestScore = 0f;

        foreach (Attraction attraction in validAttractions)
        {
            float score = 0f;

            // Bonus, jeœli atrakcja jest ulubion¹
            if (attraction == favoriteAttraction)
            {
                score += 0.3f;
            }

            // Wp³yw spo³eczny – liczba s¹siadów, którzy wybieraj¹ tê atrakcjê
            int neighborCount = 0;
            foreach (Visitor neighbor in neighbors)
            {
                if (neighbor.targetAttraction == attraction)
                {
                    neighborCount++;
                }
            }
            score += neighborCount * 0.1f;

            // Dodatkowe kryteria (mo¿esz rozszerzyæ wg potrzeb)

            if (score > bestScore)
            {
                bestScore = score;
                targetAttraction = attraction;
            }
        }

            // Ustal próg, powy¿ej którego atrakcja zostaje wybrana
            return bestScore;

    }

    #endregion

    #region - MoveNPC -
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
/*        StopCurrentPath(); // Przerwij obecn¹ œcie¿kê*/
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
        Vector3 worldTarget = tilemap.GetCellCenterWorld(new Vector3Int(targetPosition.x, targetPosition.y, 0));

        while (Vector3.Distance(transform.position, worldTarget) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, worldTarget, maxSpeed * Time.deltaTime);
            yield return null;
        }

        MoveToNextPoint();
    }
    #endregion

    public void Pay(int money)
    {
        this.money -= money;
        Player.instance.getMoney(money);
    }
    public void Eat(int food)
    {
        gunger = Mathf.Max(gunger -= food, 0);
    }

    private void UpdateStats()
    {
        float timeScale = ClockUI.instance?.getAcceleration() ?? 1f;
        gunger += Mathf.FloorToInt(timeScale * Time.deltaTime * 30f); // Szybszy wzrost
        gunger = Mathf.Clamp(gunger, 0, 100);
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
}
