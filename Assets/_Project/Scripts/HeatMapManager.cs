using System.Collections.Generic;
using UnityEngine;

public class HeatMapManager : BaseManager<HeatMapManager>
{
    private Dictionary<Vector2Int, int> heatData = new Dictionary<Vector2Int, int>();

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
        int width = 20;  // Dostosuj do rozmiaru mapy
        int height = 20;
        Texture2D texture = new Texture2D(width, height);

        foreach (var entry in heatData)
        {
            Vector2Int pos = entry.Key;
            int visits = entry.Value;

            Color heatColor = Color.Lerp(Color.blue, Color.red, Mathf.Clamp01(visits / 10f)); // Gradient: niebieski -> czerwony
            texture.SetPixel(pos.x, pos.y, heatColor);
        }

        texture.Apply();
        return texture;
    }

    private void ShowHeatmapUI(Texture2D texture)
    {
        GameObject heatmapObject = new GameObject("HeatmapUI");
        SpriteRenderer renderer = heatmapObject.AddComponent<SpriteRenderer>();
        renderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        renderer.sortingOrder = 100; // Na wierzchu

        heatmapObject.transform.position = new Vector3(0, 0, 0);
    }

    public void ResetHeatData()
    {
        heatData.Clear();
    }
}
