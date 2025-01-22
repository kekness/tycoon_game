using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class UIManager : BaseManager<UIManager>
{
    public GameObject inspectorPrefab;
    public Transform parentCanvas;   

    private List<GameObject> openInspectors = new List<GameObject>();
    public void Awake()
    {
        base.InitializeManager();
    }

    public void ShowInspector(Attraction attraction)
    {
        GameObject newInspector = Instantiate(inspectorPrefab, parentCanvas);
        openInspectors.Add(newInspector);

        Inspector inspector = newInspector.GetComponent<Inspector>();
        inspector.SetupInspector(attraction, () => CloseInspector(newInspector));
    }

    public void CloseInspector(GameObject inspector)
    {
        openInspectors.Remove(inspector);
        Destroy(inspector);
    }
   /* public void updateGhostStructure(Structure str)
    {
        // Tworzymy obiekt ducha, jeœli jeszcze nie istnieje

            string ghostName = str.structureName;
            GameObject ghostObject = new GameObject(ghostName);
            SpriteRenderer ghostRenderer = new SpriteRenderer();
            ghostRenderer = ghostObject.AddComponent<SpriteRenderer>();
            ghostRenderer.sprite = str.GetComponent<SpriteRenderer>().sprite;
            ghostRenderer.sortingOrder = 10;


        // Aktualizujemy pozycjê ducha
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = 0;

           Vector2Int currentGridPosition;
         Vector3Int cellPosition = GridManager.instance.tilemap.WorldToCell(worldPosition);
        currentGridPosition = new Vector2Int(cellPosition.x, cellPosition.y);

        ghostObject.transform.position = GridManager.instance.tilemap.GetCellCenterWorld(cellPosition);

        // Sprawdzamy, czy przestrzeñ jest dostêpna
        if (GridManager.instance.IsSpaceAvailable(currentGridPosition, str.size))
            ghostRenderer.color = Color.green;
        else
            ghostRenderer.color = Color.red;
    }*/
   
}
