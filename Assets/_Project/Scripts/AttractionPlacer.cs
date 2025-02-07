using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class AttractionPlacer : BaseManager<GridManager>
{

    public Tilemap tilemap;
    public List<GameObject> attractionPrefabs;
    private GameObject selectedAttractionPrefab;

    public Vector2Int currentGridPosition;
    public Player player;
    public GameObject floatingTextPrefab;
    private bool deleteMode = false, inspectorMode = false;
    private Attraction lastPlacedAttraction;
    private bool isPlacingEntrance = false;
    private bool isPlacingExit = false;
    public GameObject entrancePrefab;
    public GameObject exitPrefab;
    public GameObject bridgePrefab;
    private Structure currentStructure;
    private GameObject ghostEntrance; // Ghost for entrance
    private GameObject ghostExit; // Ghost for exit

    public void Awake()
    {
        base.InitializeManager();
    }

    private void Start()
    {
        if (attractionPrefabs.Count > 0)
        {
            SelectAttraction(0); // Domyœlnie wybierz pierwsz¹ atrakcjê
        }
    }

    private void Update()
    {
        // Aktualizuj pozycjê kursora myszy na siatce
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = 0;
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
        currentGridPosition = new Vector2Int(cellPosition.x, cellPosition.y);

        if (isPlacingEntrance || isPlacingExit)
        {
            // U¿yj UpdateGhostStructure do obs³ugi wejœcia/wyjœcia
            Structure structure = isPlacingEntrance ? entrancePrefab.GetComponent<Structure>() : exitPrefab.GetComponent<Structure>();
            UIManager.instance.UpdateGhostStructure(structure, IsAdjacentToAttraction(currentGridPosition,lastPlacedAttraction)&&GridManager.instance.IsSpaceAvailable(currentGridPosition,structure.size));

            if (Input.GetMouseButtonDown(0))
            {
                if (isPlacingEntrance)
                    PlaceEntrance();
                else if (isPlacingExit)
                    PlaceExit();
            }
        }
        else if (!deleteMode && !inspectorMode)
        {
            // U¿yj UpdateGhostStructure do obs³ugi atrakcji
            UIManager.instance.UpdateGhostStructure(currentStructure, GridManager.instance.IsSpaceAvailable(currentGridPosition,currentStructure.size));

            if (Input.GetMouseButtonDown(0))
            {
                TryPlaceBuilding();
            }
        }
        else if (deleteMode)
        {
            RemoveAttraction();
        }
        else if (inspectorMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
                if (hit.collider != null)
                {
                    Attraction attraction = hit.collider.GetComponent<Attraction>();
                    if (attraction != null)
                    {
                        UIManager inspectorManager = FindObjectOfType<UIManager>();
                        if (inspectorManager == null)
                        {
                            Debug.LogError("InspectorManager not found in the scene!");
                            return;
                        }
                        inspectorManager.ShowInspector(attraction);
                    }
                }
            }
        }
    }

    private void PlaceEntrance()
    {
        if (GridManager.instance.IsSpaceAvailable(currentGridPosition, new Vector2Int(1, 1)) && IsAdjacentToAttraction(currentGridPosition,lastPlacedAttraction))
        {
            Vector3 placementPosition = tilemap.GetCellCenterWorld(new Vector3Int(currentGridPosition.x, currentGridPosition.y, 0));
            GameObject entranceObject = Instantiate(entrancePrefab, placementPosition, Quaternion.identity);
            SpriteRenderer renderer = entranceObject.GetComponent<SpriteRenderer>();
            renderer.sortingOrder = Mathf.RoundToInt(-entranceObject.transform.position.y * 100);

            ExitEntry entrance = entranceObject.GetComponent<ExitEntry>();
            lastPlacedAttraction.entrance = entrance;
            entrance.coordinates = new List<Vector2Int> { currentGridPosition };

            GridManager.instance.OccupySpace(currentGridPosition, new Vector2Int(1, 1));

            Destroy(ghostEntrance); // Destroy ghost entrance after placement
            isPlacingEntrance = false;
            isPlacingExit = true;
        }
        else
        {
            Debug.Log("Nie mo¿na umieœciæ wejœcia! Musi byæ ustawione obok atrakcji.");
        }
    }

    private void PlaceExit()
    {
        if (GridManager.instance.IsSpaceAvailable(currentGridPosition, new Vector2Int(1, 1)) && IsAdjacentToAttraction(currentGridPosition,lastPlacedAttraction))
        {
            Vector3 placementPosition = tilemap.GetCellCenterWorld(new Vector3Int(currentGridPosition.x, currentGridPosition.y, 0));
            GameObject exitObject = Instantiate(exitPrefab, placementPosition, Quaternion.identity);
            SpriteRenderer renderer = exitObject.GetComponent<SpriteRenderer>();
            renderer.sortingOrder = Mathf.RoundToInt(-exitObject.transform.position.y * 100);

            ExitEntry exit = exitObject.GetComponent<ExitEntry>();
            lastPlacedAttraction.exit = exit;
            exit.coordinates = new List<Vector2Int> { currentGridPosition };

            GridManager.instance.OccupySpace(currentGridPosition, new Vector2Int(1, 1));

            Destroy(ghostExit); // Destroy ghost exit after placement
            isPlacingExit = false;
        }
        else
        {
            Debug.Log("Nie mo¿na umieœciæ wyjœcia! Musi byæ ustawione obok atrakcji.");
        }
    }

    private bool IsAdjacentToAttraction(Vector2Int position,Attraction attraction)
    { 
            foreach (var coord in attraction.coordinates)
            {
                // SprawdŸ s¹siedztwo w czterech kierunkach
                if (Vector2Int.Distance(coord, position) == 1) // Sprawdzamy odleg³oœæ o 1
                {
                    return true;
                }
            }
        return false;
    }


    private void SelectAttraction(int index)
    {
        if (index >= 0 && index < attractionPrefabs.Count)
        {
            selectedAttractionPrefab = attractionPrefabs[index];
            currentStructure = selectedAttractionPrefab.GetComponent<Structure>();
        }
    }


    private void TryPlaceBuilding()
    {
        Structure structure = selectedAttractionPrefab.GetComponent<Structure>();

        if (structure is QueuePath)
        {
            TryPlaceQueuePath();
            return;
        }

        Vector2Int gridPosition = GridManager.instance.GetCurrentGridPosition();

        if (GridManager.instance.IsSpaceAvailable(gridPosition, structure.size))
        {
            if (player.balance >= structure.cost)
            {
                Vector3Int cellPosition = new Vector3Int(gridPosition.x, gridPosition.y, 0);
                Vector3 placementPosition = tilemap.GetCellCenterWorld(cellPosition);

                GameObject buildingObject = Instantiate(selectedAttractionPrefab, placementPosition, Quaternion.identity);
                SpriteRenderer renderer = buildingObject.GetComponent<SpriteRenderer>();
                renderer.sortingOrder = Mathf.RoundToInt(-buildingObject.transform.position.y * 100);

                Structure placedStructure = buildingObject.GetComponent<Structure>();
                placedStructure.coordinates = CalculateCoordinates(gridPosition, structure.size);

                GridManager.instance.OccupySpace(gridPosition, structure.size);
                player.pay(structure.cost);

                ShowFloatingText($"-{structure.cost}$", placementPosition);

                PathManager.instance.DisplayGridPath();

                if (placedStructure is Attraction attraction)
                {
                    player.attractionList.Add(attraction);
                    lastPlacedAttraction = attraction;
                    isPlacingEntrance = true;
                }

            }
            else
            {
                Debug.Log("Nie masz wystarczaj¹cej iloœci pieniêdzy!");
            }
        }
        else
        {
            // Sprawdy, czy na danym polu znajduje sie rzeka
            River river = GridManager.instance.GetStructureAt<River>(gridPosition);

            if (structure is Path && river != null && !river.isBridged)
            {
                Vector3 placementPosition = tilemap.GetCellCenterWorld(new Vector3Int(gridPosition.x, gridPosition.y, 0));
                GameObject bridgeObject = Instantiate(bridgePrefab, placementPosition, Quaternion.identity);

                Bridge bridge = bridgeObject.GetComponent<Bridge>();
                bridge.coordinates = CalculateCoordinates(gridPosition, structure.size);

                GridManager.instance.OccupySpace(gridPosition, structure.size);
                player.pay(bridge.cost);

                ShowFloatingText($"-{bridge.cost}$", placementPosition);

                Debug.Log("Most zostal postawiony!");
                river.isBridged = true;
                return;
            }
        }
    }

    private void TryPlaceQueuePath()
    {
        Vector2Int gridPosition = GridManager.instance.GetCurrentGridPosition();

        if (!GridManager.instance.IsSpaceAvailable(gridPosition, new Vector2Int(1, 1)))
        {
            Debug.Log("Nie mo¿na umieœciæ kolejki tutaj!");
            return;
        }

        Vector3Int cellPosition = new Vector3Int(gridPosition.x, gridPosition.y, 0);
        Vector3 placementPosition = tilemap.GetCellCenterWorld(cellPosition);

        GameObject queueObject = Instantiate(selectedAttractionPrefab, placementPosition, Quaternion.identity);
        QueuePath queuePath = queueObject.GetComponent<QueuePath>();
        queuePath.coordinates = new List<Vector2Int> { gridPosition };

        GridManager.instance.OccupySpace(gridPosition, new Vector2Int(1, 1));
        player.pay(queuePath.cost);

        ShowFloatingText($"-{queuePath.cost}$", placementPosition);

        Attraction nearbyAttraction = FindNearbyAttraction(gridPosition);
        if (nearbyAttraction != null)
        {
            nearbyAttraction.AddQueuePath(queuePath);
            queuePath.hostAttraction = nearbyAttraction;
            Debug.Log("Dodano pierwszy segment kolejki do atrakcji!");
            return;
        }

        QueuePath nearbyQueue = FindNearbyQueue(gridPosition, queuePath);
        if (nearbyQueue != null)
        {
            nearbyQueue.nextQueuePath = queuePath;
            queuePath.hostAttraction = nearbyQueue.hostAttraction;
            nearbyQueue.hostAttraction.AddQueuePath(queuePath);

            Debug.Log("Dodano segment do istniej¹cej kolejki!");
        }
    }

    private Attraction FindNearbyAttraction(Vector2Int position)
    {
        foreach (var attraction in player.attractionList)
        {
            if (Vector2Int.Distance(attraction.entrance.coordinates[0], position) == 1)
            {
                return attraction;
            }
        }
        return null;
    }

    private QueuePath FindNearbyQueue(Vector2Int position, QueuePath self)
    {
        Vector3 worldPosition = tilemap.GetCellCenterWorld(new Vector3Int(position.x, position.y, 0));

        Collider2D[] colliders = Physics2D.OverlapCircleAll(worldPosition, 1f);
        foreach (var collider in colliders)
        {
            QueuePath queuePath = collider.GetComponent<QueuePath>();

            // Sprawdzenie, czy znaleziony obiekt to nie ten sam, który wywo³uje funkcjê
            if (queuePath != null && queuePath != self)
            {
                return queuePath;
            }
        }
        return null;
    }


    private List<Vector2Int> CalculateCoordinates(Vector2Int startPosition, Vector2Int size)
    {
        List<Vector2Int> coordinates = new List<Vector2Int>();

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                coordinates.Add(new Vector2Int(startPosition.x + x, startPosition.y + y));
            }
        }

        return coordinates;
    }

    private void RemoveAttraction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log($"Klikniêto na: {hit.collider.gameObject.name}");

                // Usuwanie kamieni (Boulder)
                Boulder boulder = hit.collider.GetComponent<Boulder>();
                if (boulder != null)
                {
                    Debug.Log("Boulder zosta³ trafiony przez Raycast!");

                    float removalCost = boulder.GetBoulderRemovalCost(boulder.stage);
                    if (player.balance >= removalCost)
                    {
                        player.pay(removalCost);
                        boulder.decreaseStage();
                        ShowFloatingText($"-{removalCost}$", mousePosition);
                        Debug.Log($"Kamieñ obni¿ony do stage: {boulder.stage}");
                    }
                    else
                    {
                        Debug.Log("Nie masz wystarczaj¹co pieniêdzy na usuniêcie kamienia!");
                    }
                    return;
                }

                // Usuwanie lasów
                Forest forest = hit.collider.GetComponent<Forest>();
                if (forest != null && player.balance >= forest.removalCost)
                {
                    player.pay(forest.removalCost);
                    GridManager.instance.ReleaseSpace(forest.coordinates);
                    Destroy(forest.gameObject);
                    ShowFloatingText($"-{forest.removalCost}$", mousePosition);
                    Debug.Log("Las zosta³ usuniêty!");
                    return;
                }

                // Usuwanie atrakcji
                Structure structure = hit.collider.GetComponent<Structure>();
                if (structure is Attraction attraction)
                {
                    if (attraction.entrance != null)
                    {
                        GridManager.instance.ReleaseSpace(attraction.entrance.coordinates);
                        Destroy(attraction.entrance.gameObject);
                    }
                    if (attraction.exit != null)
                    {
                        GridManager.instance.ReleaseSpace(attraction.exit.coordinates);
                        Destroy(attraction.exit.gameObject);
                    }
                    player.attractionList.Remove(attraction);
                }

                // Usuwanie zwyk³ych struktur (nie rzek)
                if (structure != null && !(structure is River))
                {
                    GridManager.instance.ReleaseSpace(structure.coordinates);
                    Destroy(structure.gameObject);
                    Debug.Log("Obiekt zosta³ usuniêty!");
                }
            }
        }
    }


    // Funkcja zwracaj¹ca koszt usuniêcia kamienia w zale¿noœci od stage



    private void ShowFloatingText(string text, Vector3 position)
    {
        if (floatingTextPrefab != null)
        {
            GameObject floatingText = Instantiate(floatingTextPrefab, position, Quaternion.identity);
            floatingText.transform.SetParent(GameObject.Find("Canvas").transform, false);

            Vector2 screenPosition = Camera.main.WorldToScreenPoint(position);
            floatingText.transform.position = screenPosition;

            var textMesh = floatingText.GetComponent<TextMeshProUGUI>();
            if (textMesh != null)
            {
                textMesh.text = text;
            }
            else
            {
                Debug.LogError("Prefab floatingTextPrefab nie zawiera komponentu TextMeshProUGUI!");
            }
        }
        else
        {
            Debug.LogError("Prefab floatingTextPrefab nie jest przypisany!");
        }
    }


    #region buttons
    // Buttony dla atrakcji
    public void SelectPathTile() { SelectAttraction(0); }
    public void SelectQueueTile() { SelectAttraction(5); }
    public void SelectAttraction1Tile() { SelectAttraction(1); }
    public void SelectAttraction2Tile() { SelectAttraction(2); }
    public void SelectAttraction3Tile() { SelectAttraction(3); }
    public void SelectGate() { SelectAttraction(4); }
    public void RemoveObject() { deleteMode = !deleteMode; }
    public void InspectorMode()
    {
        inspectorMode = !inspectorMode;
    }
    public void selectFoodStand() { SelectAttraction(6); }
    #endregion
}
