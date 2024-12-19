using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PathBuilder : MonoBehaviour
{
    public Tilemap tilemap; // Odniesienie do Tilemap
    public TileBase pathTile; // Kafelek œcie¿ki
    public TileBase attraction1Tile; // Kafelek atrakcji 1
    public TileBase attraction2Tile; // Kafelek atrakcji 2
    public TileBase attraction3Tile; // Kafelek atrakcji 3

    private TileBase selectedTile; // Wybrany kafelek do umieszczenia

    void Start()
    {
        // Domyœlnie wybrana œcie¿ka
        selectedTile = pathTile;
    }

    void Update()
    {
        if (Input.GetMouseButton(0)) // Klikniêcie lewym przyciskiem
        {
            // Pobierz pozycjê myszy w œwiecie
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Przekszta³æ pozycjê œwiata na pozycjê w siatce Tilemap
            Vector3Int cellPosition = tilemap.WorldToCell(new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0));

            // Debug: Poka¿ pozycjê klikniêtej komórki w konsoli
            Debug.Log("Klikniêta komórka: " + cellPosition);

            // Ustaw wybrany kafelek w klikniêtej pozycji
            tilemap.SetTile(cellPosition, selectedTile);
        }
    }

    // Funkcje wyboru kafelka
    public void SelectPathTile()
    {
        selectedTile = pathTile;
    }

    public void SelectAttraction1Tile()
    {
        selectedTile = attraction1Tile;
    }

    public void SelectAttraction2Tile()
    {
        selectedTile = attraction2Tile;
    }

    public void SelectAttraction3Tile()
    {
        selectedTile = attraction3Tile;
    }
}
