using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class UIManager : BaseManager<UIManager>
{
    public GameObject inspectorPrefab;
    public Transform parentCanvas;
    GameObject ghostObject;
    SpriteRenderer ghostRenderer = new SpriteRenderer();

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

    public void UpdateGhostStructure(Structure structure, bool isAvailable)
    {
        if (ghostObject == null)
        {
            ghostObject = new GameObject("Ghost");
            ghostRenderer = ghostObject.AddComponent<SpriteRenderer>();
            ghostRenderer.sortingOrder = 10;
            ghostRenderer.sortingLayerName = "UI";
        }

        ghostRenderer.sprite = structure.GetComponent<SpriteRenderer>().sprite;

        Vector2Int currentGridPosition = GridManager.instance.GetCurrentGridPosition();
        Vector3Int cellPosition = new Vector3Int(currentGridPosition.x, currentGridPosition.y, 0);

        ghostObject.transform.position = GridManager.instance.tilemap.GetCellCenterWorld(cellPosition);

        ghostRenderer.color = isAvailable ? Color.green : Color.red;
    }

}
