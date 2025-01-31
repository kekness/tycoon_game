using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeatMapManager : BaseManager<HeatMapManager>
{
    private Dictionary<Vector2Int, int> heatData = new Dictionary<Vector2Int, int>();

    public GameObject heatmapPanelPrefab; // Przypisz prefab w inspektorze

    private GameObject heatmapPanelInstance; // Instancja prefabu
    private RawImage heatmapRawImage; // Komponent RawImage z instancji prefabu

    public void Awake()
    {
        base.InitializeManager();
    }
 
    public void RecordData(Vector2Int coordinates)
    {
        if (heatData.ContainsKey(coordinates))
        {
            heatData[coordinates]++;
        }
        else
        {
            heatData[coordinates] = 1;
        }
    }

    public Dictionary<Vector2Int, int> GetHeatData()
    {
        return heatData;
    }

    public void DisplayHeatmap()
    {

        Texture2D heatmapTexture = GenerateHeatmapTexture();
        ShowHeatmapUI(heatmapTexture);
    }

    private Texture2D GenerateHeatmapTexture()
    {
        int width = 100;  // Dostosuj do rozmiaru mapy
        int height = 100;
        Texture2D texture = new Texture2D(width, height);

        // Wype³nij teksturê pocz¹tkowym kolorem (np. czarnym)
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                texture.SetPixel(x, y, Color.black);
            }
        }

        // Ustaw kolory na podstawie danych heatmapy
        foreach (var entry in heatData)
        {
            Vector2Int pos = entry.Key;
            int visits = entry.Value;

            // Skalowanie kolorów na podstawie maksymalnej liczby odwiedzin
            float maxVisits = 250f; // Maksymalna liczba odwiedzin dla gradientu
            float intensity = Mathf.Clamp01(visits / maxVisits);
            Color heatColor = Color.Lerp(Color.blue, Color.red, intensity); // Gradient: niebieski -> czerwony
            texture.SetPixel(pos.x, pos.y, heatColor);
        }

        texture.Apply();
        return texture;
    }

    private void ShowHeatmapUI(Texture2D texture)
    {
        if (heatmapPanelInstance == null)
        {
            // ZnajdŸ Canvas na scenie
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("Canvas not found in the scene!");
                return;
            }

            // Utwórz instancjê prefabu jako dziecko Canvas
            heatmapPanelInstance = Instantiate(heatmapPanelPrefab, canvas.transform);
            Debug.Log("Heatmap panel instantiated as a child of Canvas");

            // ZnajdŸ komponent RawImage w instancji prefabu
            heatmapRawImage = heatmapPanelInstance.GetComponentInChildren<RawImage>();
            if (heatmapRawImage == null)
            {
                Debug.LogError("RawImage nie zosta³ znaleziony w prefabie HeatmapPanel!");
                return;
            }
        }

        // Ustaw teksturê w RawImage
        heatmapRawImage.texture = texture;

        // Upewnij siê, ¿e panel jest widoczny
        heatmapPanelInstance.SetActive(true);
        Debug.Log("Heatmap panel is active");
    }

    public void ResetHeatData()
    {
        heatData.Clear();
    }

    public void ToggleHeatmap()
    {
        if (heatmapPanelInstance != null)
        {
            heatmapPanelInstance.SetActive(!heatmapPanelInstance.activeSelf);
        }
    }
}