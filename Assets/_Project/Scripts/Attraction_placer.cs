using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class Attraction_placer : MonoBehaviour
{
    public Tilemap tilemap; // Odniesienie do Tilemap
    public List<GameObject> attractionPrefabs;
    private GameObject selectedAttractionPrefab;
    private GameObject ghostAttraction;
    private SpriteRenderer ghostRenderer;
    private Vector2Int currentGridPosition;
    public Player player; // Referencja do gracza
    public GameObject floatingTextPrefab;
    private bool deleteMode = false;
    private void Start()
    {
        if (attractionPrefabs.Count > 0)
        {
            SelectAttraction(0); // Domy�lnie wybierz pierwsz� atrakcj�
        }
    }

    private void Update()
    {
        if (deleteMode)
            RemoveAttraction();
        else
        {
            UpdateGhostAttraction();
            if (Input.GetMouseButtonDown(0))
                TryPlaceAttraction();
        }
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
            ghostRenderer.sortingOrder = 10; // Ustaw wy�szy priorytet renderowania
        }
    }

    private void UpdateGhostAttraction()
    {
        // Convert mouse position to world position
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = 0; // Ensure it's on the same plane

        // Convert world position to grid cell position
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
        currentGridPosition = new Vector2Int(cellPosition.x, cellPosition.y);

        // Update ghost attraction position
        ghostAttraction.transform.position = tilemap.GetCellCenterWorld(cellPosition);

        // Check if placement is valid
        Attraction attraction = selectedAttractionPrefab.GetComponent<Attraction>();
        if (Grid_manager.Instance.IsSpaceAvailable(currentGridPosition, attraction.size))
        {
            ghostRenderer.color = Color.green;
        }
        else
        {
            ghostRenderer.color = Color.red;
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
                Attraction placedAttraction = attractionObject.GetComponent<Attraction>();

               
                placedAttraction.coordinates = CalculateCoordinates(currentGridPosition, attraction.size);

               
                Grid_manager.Instance.OccupySpace(currentGridPosition, attraction.size);

              
                player.balance -= attraction.cost;

               
                ShowFloatingText($"-{attraction.cost}$", placementPosition);
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
                 
                    Grid_manager.Instance.ReleaseSpace(attraction.coordinates);

                    Destroy(attraction.gameObject);

                    Debug.Log("Atrakcja usuni�ta!");
                }
            }
        }
    }



    private void ShowFloatingText(string text, Vector3 position)
    {
        if (floatingTextPrefab != null)
        {
            GameObject floatingText = Instantiate(floatingTextPrefab, position, Quaternion.identity);

            // Ustaw rodzica na Canvas
            floatingText.transform.SetParent(GameObject.Find("Canvas").transform, false);

            // Konwersja pozycji �wiata gry na pozycj� Canvas
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

    public void SelectPathTile()
    {
        SelectAttraction(0);
    }

    public void SelectAttraction1Tile()
    {
        SelectAttraction(1);
    }

    public void SelectAttraction2Tile()
    {
        SelectAttraction(2);
    }

    public void SelectAttraction3Tile()
    {
        SelectAttraction(3);
    }
    public void RemoveObject()
    {
        deleteMode = !deleteMode;
        RemoveAttraction();
    }

}
