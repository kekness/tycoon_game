using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class UIManager : BaseManager<UIManager>
{
    [Header("UI Elements")]
    public GameObject inspectorPrefab; // Prefab dla okna inspektora
    public GameObject daySummaryPrefab; // Prefab dla okna podsumowania dnia
    public Transform parentCanvas; // Rodzic dla okien UI

    [Header("Shop System")]
    public GameObject shopPrefab; // Prefab sklepu
    private ShopManager shopInstance; // Referencja do instancji sk
    public List<GameObject> availableShopItems = new List<GameObject>();

    private GameObject ghostObject; // "Duch" dla podgl¹du obiektów
    private SpriteRenderer ghostRenderer;

    public void Awake()
    {

        base.InitializeManager();
    }



    public void ToggleShop()
    {
        AttractionPlacer.instance.inspectorMode = false;
        AttractionPlacer.instance.deleteMode = false;
        if (shopInstance == null)
        {
            GameObject shopObject = Instantiate(shopPrefab, parentCanvas);
            shopInstance = shopObject.GetComponent<ShopManager>();

            if (shopInstance == null)
            {
                Debug.LogError("ShopManager nie znaleziony w shopPrefab!");
                return;
            }
        }

        // Pobieramy listê dostêpnych atrakcji i przekazujemy do sklepu
        AttractionPlacer attractionPlacer = FindObjectOfType<AttractionPlacer>();
        if (attractionPlacer == null)
        {
            Debug.LogError("AttractionPlacer nie znaleziony w scenie!");
            return;
        }

        shopInstance.PopulateShop();
     
    }




    public void ShowInspector(Attraction attraction)
    {
        if (inspectorPrefab == null)
        {
            Debug.LogError("Inspector prefab is not assigned in UIManager!");
            return;
        }

        GameObject newInspector = Instantiate(inspectorPrefab, parentCanvas);
        Inspector inspector = newInspector.GetComponent<Inspector>();

        if (inspector == null)
        {
            Debug.LogError("Inspector component not found on the instantiated prefab!");
            return;
        }

        inspector.SetupInspector(attraction, () => {
            Debug.Log("Inspector closed.");
        });
    }


    public void ShowDaySummary()
    {
        if (daySummaryPrefab == null)
        {
            Debug.LogError("DaySummary prefab is not assigned in UIManager!");
            return;
        }

        if (parentCanvas == null)
        {
            Debug.LogError("Parent canvas is not assigned in UIManager!");
            return;
        }

        GameObject newDaySummary = Instantiate(daySummaryPrefab, parentCanvas);
        DaySummary daySummary = newDaySummary.GetComponent<DaySummary>();

        if (daySummary == null)
        {
            Debug.LogError("DaySummary component not found on the instantiated prefab!");
            return;
        }

        daySummary.SetupDaySummary();
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
