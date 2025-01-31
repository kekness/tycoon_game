using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DaySummary : PopUpWindow
{
    public TextMeshProUGUI ExpensesText;
    public TextMeshProUGUI EarningsText;
    public TextMeshProUGUI TotalText;
    public RawImage heatmapRawImage;
    public Image mostPopularAttractionImage;
    public TextMeshProUGUI mostPopularText;
    public TextMeshProUGUI mostPopularCountText;

    public TextMeshProUGUI numberOfVisitorsText;
    public TextMeshProUGUI DayText;
    public void SetupDaySummary()
    {
        if (ExpensesText == null)
        {
            Debug.LogError("ExpensesText is not assigned in DaySummary!");
            return;
        }

        if (Player.instance == null)
        {
            Debug.LogError("Player instance is null!");
            return;
        }

        DayText.text = $"Podsumowanie dnia {Player.instance.DayNumber}";

        ExpensesText.text = "-" + Player.instance.todaysExpenses.ToString();
        EarningsText.text = "+" + Player.instance.todaysEarnings.ToString();
        float total = Player.instance.todaysEarnings - Player.instance.todaysExpenses;
        TotalText.text = (total >= 0 ? "+" : "-") + Mathf.Abs(total).ToString();
        TotalText.color = total >= 0 ? Color.green : Color.red;

        numberOfVisitorsText.text =$"Iloœæ odwiedzaj¹cych dzisiaj: {Player.instance.numberOfVisitors.ToString()}" ;
        DisplayHeatmap();

        mostPopularText.text = Player.instance.mostPopularAttraction().structureName;
        mostPopularCountText.text = $"Odwiedzono {Player.instance.mostPopularAttraction().todaysVisitations} razy";
        mostPopularAttractionImage.sprite = Player.instance.mostPopularAttraction().attractionSprite;





    }

    private void DisplayHeatmap()
    {
        Dictionary<Vector2Int, int> heatData = HeatMapManager.instance.GetHeatData();
        Debug.Log($"HeatData count: {heatData.Count}");

        Texture2D heatmapTexture = GenerateHeatmapTexture(heatData);

        if (heatmapRawImage != null)
        {
            heatmapRawImage.texture = heatmapTexture;
            Debug.Log("Heatmap texture applied to RawImage");
        }
        else
        {
            Debug.LogError("Heatmap RawImage is not assigned in DaySummary!");
        }
    }

    private Texture2D GenerateHeatmapTexture(Dictionary<Vector2Int, int> heatData)
    {
        int width = GridManager.instance.gridWidth;
        int height = GridManager.instance.gridHeight;

        if (width <= 0 || height <= 0)
        {
            Debug.LogError("Invalid grid dimensions! Check GridManager.");
            return new Texture2D(1, 1); // Zwróæ pust¹ teksturê w przypadku b³êdu
        }

        Texture2D texture = new Texture2D(width, height);

        // Wype³nij teksturê kolorem czarnym
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                texture.SetPixel(x, y, Color.black);
            }
        }

        // Jeœli heatData nie jest pusty, ustaw kolory na podstawie danych
        if (heatData != null && heatData.Count > 0)
        {
            foreach (var entry in heatData)
            {
                Vector2Int pos = entry.Key;
                int visits = entry.Value;

                // SprawdŸ, czy wspó³rzêdne mieszcz¹ siê w zakresie tekstury
                if (pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height)
                {
                    float maxVisits = 25f; // Maksymalna liczba odwiedzin dla gradientu
                    float intensity = Mathf.Clamp01(visits / maxVisits);
                    Color heatColor = Color.Lerp(Color.blue, Color.red, intensity); // Gradient: niebieski -> czerwony
                    texture.SetPixel(pos.x, pos.y, heatColor);
                }
                else
                {
                    Debug.LogWarning($"Invalid coordinates: {pos}. Skipping.");
                }
            }
        }
        else
        {
            Debug.LogWarning("HeatData is empty. Texture will be entirely black.");
        }

        texture.Apply();
        return texture;
    }

    public void ToggleHeatmap()
    {
        if (heatmapRawImage != null)
        {
            heatmapRawImage.gameObject.SetActive(!heatmapRawImage.gameObject.activeSelf);
        }
    }
}