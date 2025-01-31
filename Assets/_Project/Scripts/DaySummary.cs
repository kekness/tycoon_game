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

        ExpensesText.text ="-"+ Player.instance.todaysExpenses.ToString();
        EarningsText.text = "+" + Player.instance.todaysEarnings.ToString();
        float total = Player.instance.todaysEarnings - Player.instance.todaysExpenses;
        TotalText.text = (total >= 0 ? "+" : "-") + Mathf.Abs(total).ToString(); 
        TotalText.color = total >= 0 ? Color.green : Color.red;
        DisplayHeatmap();
    }

    private void DisplayHeatmap()
    {
        Dictionary<Vector2Int, int> heatData = HeatMapManager.instance.GetHeatData();
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
        Texture2D texture = new Texture2D(width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                texture.SetPixel(x, y, Color.black);
            }
        }

        foreach (var entry in heatData)
        {
            Vector2Int pos = entry.Key;
            int visits = entry.Value;

            float maxVisits = 250f; // Maksymalna liczba odwiedzin dla gradientu
            float intensity = Mathf.Clamp01(visits / maxVisits);
            Color heatColor = Color.Lerp(Color.blue, Color.red, intensity); // Gradient: niebieski -> czerwony
            texture.SetPixel(pos.x, pos.y, heatColor);
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