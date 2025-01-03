using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class Attraction_placer : MonoBehaviour
{
    public Tilemap tilemap;
    public List<GameObject> attractionPrefabs;
    private GameObject selectedAttractionPrefab;
    private GameObject ghostAttraction;
    private SpriteRenderer ghostRenderer;
    private Vector2Int currentGridPosition;
    public Player player;
    public GameObject floatingTextPrefab;
    private bool deleteMode = false;
    private Attraction lastPlacedAttraction;
    private bool isPlacingEntrance = false;
    private bool isPlacingExit = false;
    public GameObject entrancePrefab;
    public GameObject exitPrefab;

    private GameObject ghostEntrance; // Ghost for entrance
    private GameObject ghostExit; // Ghost for exit
    private SpriteRenderer entranceRenderer;
    private SpriteRenderer exitRenderer;

    private void Start()
    {
        if (attractionPrefabs.Count > 0)
        {
            SelectAttraction(0); // Domy�lnie wybierz pierwsz� atrakcj�
        }
    }

    private void Update()
    {
        if (isPlacingEntrance)
        {
            UpdateGhostEntrance();
            if (Input.GetMouseButtonDown(0))
            {
                PlaceEntrance();
            }
        }
        else if (isPlacingExit)
        {
            UpdateGhostExit();
            if (Input.GetMouseButtonDown(0))
            {
                PlaceExit();
            }
        }
        else if (!deleteMode)
        {
            UpdateGhostAttraction();
            if (Input.GetMouseButtonDown(0))
            {
                TryPlaceAttraction();
            }
        }
        else
        {
            RemoveAttraction();
        }
    }

    private void PlaceEntrance()
    {
        if (Grid_manager.Instance.IsSpaceAvailable(currentGridPosition, new Vector2Int(1, 1)) && IsAdjacentToAttraction(currentGridPosition))
        {
            Vector3 placementPosition = tilemap.GetCellCenterWorld(new Vector3Int(currentGridPosition.x, currentGridPosition.y, 0));
            GameObject entranceObject = Instantiate(entrancePrefab, placementPosition, Quaternion.identity);

            ExitEntry entrance = entranceObject.GetComponent<ExitEntry>();
            lastPlacedAttraction.entrance = entrance;
            entrance.coordinates = new List<Vector2Int> { currentGridPosition };

            Grid_manager.Instance.OccupySpace(currentGridPosition, new Vector2Int(1, 1));

            Destroy(ghostEntrance); // Destroy ghost entrance after placement
            isPlacingEntrance = false;
            isPlacingExit = true;
        }
        else
        {
            Debug.Log("Nie mo�na umie�ci� wej�cia! Musi by� ustawione obok atrakcji.");
        }
    }

    private void PlaceExit()
    {
        if (Grid_manager.Instance.IsSpaceAvailable(currentGridPosition, new Vector2Int(1, 1)) && IsAdjacentToAttraction(currentGridPosition))
        {
            Vector3 placementPosition = tilemap.GetCellCenterWorld(new Vector3Int(currentGridPosition.x, currentGridPosition.y, 0));
            GameObject exitObject = Instantiate(exitPrefab, placementPosition, Quaternion.identity);

            ExitEntry exit = exitObject.GetComponent<ExitEntry>();
            lastPlacedAttraction.exit = exit;
            exit.coordinates = new List<Vector2Int> { currentGridPosition };

            Grid_manager.Instance.OccupySpace(currentGridPosition, new Vector2Int(1, 1));

            Destroy(ghostExit); // Destroy ghost exit after placement
            isPlacingExit = false;
        }
        else
        {
            Debug.Log("Nie mo�na umie�ci� wyj�cia! Musi by� ustawione obok atrakcji.");
        }
    }

    private bool IsAdjacentToAttraction(Vector2Int position)
    {
        // Przeszukaj wszystkie atrakcje i sprawd�, czy s�siaduj� z danym punktem
        foreach (var attraction in player.attractionList)
        {
            foreach (var coord in attraction.coordinates)
            {
                // Sprawd� s�siedztwo w czterech kierunkach
                if (Vector2Int.Distance(coord, position) == 1) // Sprawdzamy odleg�o�� o 1
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

            if (ghostAttraction == null)
            {
                ghostAttraction = new GameObject("GhostAttraction");
                ghostRenderer = ghostAttraction.AddComponent<SpriteRenderer>();
            }

            ghostRenderer.sprite = selectedAttractionPrefab.GetComponent<SpriteRenderer>().sprite;
            ghostRenderer.sortingOrder = 10;
        }
    }

    private void UpdateGhostAttraction()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = 0;

        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
        currentGridPosition = new Vector2Int(cellPosition.x, cellPosition.y);

        ghostAttraction.transform.position = tilemap.GetCellCenterWorld(cellPosition);

        Attraction attraction = selectedAttractionPrefab.GetComponent<Attraction>();
        if (Grid_manager.Instance.IsSpaceAvailable(currentGridPosition, attraction.size))
        {
            ghostRenderer.color = Color.green;
        }
        else
        {
            ghostRenderer.color = Color.red;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RotateAttraction();
        }
    }

    private void UpdateGhostEntrance()
    {
        if (ghostEntrance == null)
        {
            ghostEntrance = new GameObject("GhostEntrance");
            entranceRenderer = ghostEntrance.AddComponent<SpriteRenderer>();
            entranceRenderer.sprite = entrancePrefab.GetComponent<SpriteRenderer>().sprite;
            entranceRenderer.sortingOrder = 10;
        }

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = 0;

        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
        currentGridPosition = new Vector2Int(cellPosition.x, cellPosition.y);

        ghostEntrance.transform.position = tilemap.GetCellCenterWorld(cellPosition);

        if (Grid_manager.Instance.IsSpaceAvailable(currentGridPosition, new Vector2Int(1, 1)))
        {
            entranceRenderer.color = Color.green;
        }
        else
        {
            entranceRenderer.color = Color.red;
        }
    }

    private void UpdateGhostExit()
    {
        if (ghostExit == null)
        {
            ghostExit = new GameObject("GhostExit");
            exitRenderer = ghostExit.AddComponent<SpriteRenderer>();
            exitRenderer.sprite = exitPrefab.GetComponent<SpriteRenderer>().sprite;
            exitRenderer.sortingOrder = 10;
        }

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = 0;

        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
        currentGridPosition = new Vector2Int(cellPosition.x, cellPosition.y);

        ghostExit.transform.position = tilemap.GetCellCenterWorld(cellPosition);

        if (Grid_manager.Instance.IsSpaceAvailable(currentGridPosition, new Vector2Int(1, 1)))
        {
            exitRenderer.color = Color.green;
        }
        else
        {
            exitRenderer.color = Color.red;
        }
    }

    private void TryPlaceAttraction()
    {
        Attraction attraction = selectedAttractionPrefab.GetComponent<Attraction>();

        if (Grid_manager.Instance.IsSpaceAvailable(currentGridPosition, attraction.size))
        {
            if (player.balance >= attraction.cost)
            {
                Vector3Int cellPosition = new Vector3Int(currentGridPosition.x, currentGridPosition.y, 0);
                Vector3 placementPosition = tilemap.GetCellCenterWorld(cellPosition);

                GameObject attractionObject = Instantiate(selectedAttractionPrefab, placementPosition, Quaternion.identity);
                attractionObject.transform.localScale = ghostAttraction.transform.localScale;
                attractionObject.transform.rotation = ghostAttraction.transform.rotation;

                lastPlacedAttraction = attractionObject.GetComponent<Attraction>();
                lastPlacedAttraction.coordinates = CalculateCoordinates(currentGridPosition, attraction.size);

                player.attractionList.Add(lastPlacedAttraction);
                Grid_manager.Instance.OccupySpace(currentGridPosition, attraction.size);

                player.balance -= attraction.cost;

                ShowFloatingText($"-{attraction.cost}$", placementPosition);

                isPlacingEntrance = true; // Prze��cz na tryb ustawiania wej�cia
            }
            else
            {
                Debug.Log("Nie masz wystarczaj�cej ilo�ci pieni�dzy!");
            }
        }
        else
        {
            Debug.Log("Nie mo�na umie�ci� atrakcji tutaj!");
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
                Attraction attraction = hit.collider.GetComponent<Attraction>();
                if (attraction != null)
                {
                    // Usuwamy wej�cie
                    if (attraction.entrance != null)
                    {
                        Grid_manager.Instance.ReleaseSpace(attraction.entrance.coordinates);
                        Destroy(attraction.entrance.gameObject);
                    }

                    // Usuwamy wyj�cie
                    if (attraction.exit != null)
                    {
                        Grid_manager.Instance.ReleaseSpace(attraction.exit.coordinates);
                        Destroy(attraction.exit.gameObject);
                    }

                    // Usuwamy atrakcj�
                    Grid_manager.Instance.ReleaseSpace(attraction.coordinates);
                    player.attractionList.Remove(attraction);
                    Destroy(attraction.gameObject);

                    Debug.Log("Atrakcja i powi�zane obiekty zosta�y usuni�te!");
                }
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

    private void RotateAttraction()
    {
        selectedAttractionPrefab.GetComponent<Attraction>().size =
            new Vector2Int(selectedAttractionPrefab.GetComponent<Attraction>().size.y,
                           selectedAttractionPrefab.GetComponent<Attraction>().size.x);

        Vector3 scale = ghostAttraction.transform.localScale;
        scale.x *= -1;
        ghostAttraction.transform.localScale = scale;
    }

    #region buttons
    // Buttony dla atrakcji
    public void SelectPathTile() { SelectAttraction(0); }
    public void SelectAttraction1Tile() { SelectAttraction(1); }
    public void SelectAttraction2Tile() { SelectAttraction(2); }
    public void SelectAttraction3Tile() { SelectAttraction(3); }
    public void SelectGate() { SelectAttraction(4); }
    public void RemoveObject() { deleteMode = !deleteMode; }
    #endregion
}
