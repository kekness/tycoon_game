using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class UIManager : BaseManager<UIManager>
{
    public GameObject inspectorPrefab; // Prefab dla okna inspektora
    public GameObject daySummaryPrefab; // Prefab dla okna podsumowania dnia
    public Transform parentCanvas; // Rodzic dla okien UI
    GameObject ghostObject;
    SpriteRenderer ghostRenderer = new SpriteRenderer();

    public void Awake()
    {
        base.InitializeManager();
    }

    // Metoda do wyœwietlania okna inspektora
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

        // Ustawienie danych atrakcji w inspektorze
        inspector.SetupInspector(attraction, () => {
            Debug.Log("Inspector closed.");
        });
    }

    // Metoda do wyœwietlania okna podsumowania dnia
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

        // Tworzymy nowe okno podsumowania dnia
        GameObject newDaySummary = Instantiate(daySummaryPrefab, parentCanvas);
        DaySummary daySummary = newDaySummary.GetComponent<DaySummary>();

        if (daySummary == null)
        {
            Debug.LogError("DaySummary component not found on the instantiated prefab!");
            return;
        }

        // Ustawienie danych w podsumowaniu dnia
        daySummary.SetupDaySummary();
    }

    // Metoda do aktualizacji "ducha" struktury (np. podczas budowania)
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