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
            UIManager.instance.UpdateGhostStructure(structure, IsAdjacentToAttraction(currentGridPosition));

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
            UIManager.instance.UpdateGhostStructure(currentStructure, true);

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
        if (GridManager.instance.IsSpaceAvailable(currentGridPosition, new Vector2Int(1, 1)) && IsAdjacentToAttraction(currentGridPosition))
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
        if (GridManager.instance.IsSpaceAvailable(currentGridPosition, new Vector2Int(1, 1)) && IsAdjacentToAttraction(currentGridPosition))
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

    private bool IsAdjacentToAttraction(Vector2Int position)
    {
        // Przeszukaj wszystkie atrakcje i sprawdŸ, czy s¹siaduj¹ z danym punktem
        foreach (var attraction in player.attractionList)
        {
            foreach (var coord in attraction.coordinates)
            {
                // SprawdŸ s¹siedztwo w czterech kierunkach
                if (Vector2Int.Distance(coord, position) == 1) // Sprawdzamy odleg³oœæ o 1
                {
                    return true;
                }
            }
        }
        return false;
    }


    private void SelectAttraction(int index)
    {
        if (index >= 0 && index < attractionPrefabs.Count)
        {
            selectedAttractionPrefab = attractionPrefabs[index];

        
            currentStructure = selectedAttractionPrefab.GetComponent<Structure>();//Tutaj próbowa³em robic odnosnie przenoszenia duszków do UIManagera

        }
    }


    private void TryPlaceBuilding()
    {
        Structure structure = selectedAttractionPrefab.GetComponent<Structure>();

        // Pobierz aktualn¹ pozycjê siatki z GridManager
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
                player.balance -= structure.cost;

                ShowFloatingText($"-{structure.cost}$", placementPosition);

                if (placedStructure is Attraction attraction)
                {
                    player.attractionList.Add(attraction);
                    lastPlacedAttraction = attraction;
                    isPlacingEntrance = true;
                }
                else
                {
                    Debug.Log("Budynek nie jest atrakcj¹, nie wymaga wejœcia i wyjœcia.");
                }
            }
            else
            {
                Debug.Log("Nie masz wystarczaj¹cej iloœci pieniêdzy!");
            }
        }
        else
        {
            // SprawdŸ, czy na danym polu znajduje siê rzeka
            River river = GridManager.instance.GetStructureAt<River>(gridPosition);

            if (structure is Path && river != null && !river.isBridged)
            {
                Vector3 placementPosition = tilemap.GetCellCenterWorld(new Vector3Int(gridPosition.x, gridPosition.y, 0));
                GameObject bridgeObject = Instantiate(bridgePrefab, placementPosition, Quaternion.identity);

                Bridge bridge = bridgeObject.GetComponent<Bridge>();
                bridge.coordinates = CalculateCoordinates(gridPosition, structure.size);

                GridManager.instance.OccupySpace(gridPosition, structure.size);
                player.balance -= bridge.cost;

                ShowFloatingText($"-{bridge.cost}$", placementPosition);

                Debug.Log("Most zosta³ postawiony!");
                river.isBridged = true;
                return;
            }
        }
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
                // Sprawdzamy, czy to Structure (atrakcja)
                Structure structure = hit.collider.GetComponent<Structure>();
                if (structure != null)
                {

                    // Sprawdzamy, czy to prefab lasu
                    Forest forest = hit.collider.GetComponent<Forest>();
            

                    if (forest != null && player.balance >= forest.removalCost)
                    {
      
                        GridManager.instance.ReleaseSpace(forest.coordinates);

                        Destroy(forest.gameObject);

                        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        Vector3 placementPosition = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, 0f);

                        ShowFloatingText($"-{forest.removalCost}$", placementPosition);
                        Debug.Log("Las zosta³ usuniêty!");

                    }
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
                    if (!(structure is River river))
                    {
                        GridManager.instance.ReleaseSpace(structure.coordinates);
                        Destroy(structure.gameObject);
                    }
                }   

                 

                Debug.Log("Atrakcja i powi¹zane obiekty zosta³y usuniête!");
                                                 
            }
        }
    }


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
    public void SelectAttraction1Tile() { SelectAttraction(1); }
    public void SelectAttraction2Tile() { SelectAttraction(2); }
    public void SelectAttraction3Tile() { SelectAttraction(3); }
    public void SelectGate() { SelectAttraction(4); }
    public void RemoveObject() { deleteMode = !deleteMode; }
    public void InspectorMode()
    {
        inspectorMode = !inspectorMode;
    }
    #endregion
}
