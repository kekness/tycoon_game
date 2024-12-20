using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Attraction_placer : MonoBehaviour
{
    public Tilemap tilemap; // Odniesienie do Tilemap
    public List<GameObject> attractionPrefabs;
    private GameObject selectedAttractionPrefab;
    private GameObject ghostAttraction;
    private SpriteRenderer ghostRenderer;
    private Vector2Int currentGridPosition;

    private void Start()
    {
        if (attractionPrefabs.Count > 0)
        {
            SelectAttraction(0); // Domyœlnie wybierz pierwsz¹ atrakcjê
        }
    }

    private void Update()
    {
        SelectAttraction(1);
        UpdateGhostAttraction();

        if (Input.GetMouseButtonDown(0))
        {
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
            ghostRenderer.sortingOrder = 10; // Ustaw wy¿szy priorytet renderowania
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
            Vector3Int cellPosition = new Vector3Int(currentGridPosition.x, currentGridPosition.y, 0);
            Vector3 placementPosition = tilemap.GetCellCenterWorld(cellPosition);

            Instantiate(selectedAttractionPrefab, placementPosition, Quaternion.identity);
            Grid_manager.Instance.OccupySpace(currentGridPosition, attraction.size);
        }
        else
        {
            Debug.Log("Nie mo¿na umieœciæ atrakcji tutaj!");
        }
    }
}
