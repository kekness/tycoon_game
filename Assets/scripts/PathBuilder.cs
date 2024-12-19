using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PathBuilder : MonoBehaviour
{
    public Tilemap tilemap; // Odniesienie do Tilemap
    public TileBase pathTile; // Kafelek �cie�ki
    public TileBase attraction1Tile; // Kafelek atrakcji 1
    public TileBase attraction2Tile; // Kafelek atrakcji 2
    public TileBase attraction3Tile; // Kafelek atrakcji 3

    private TileBase selectedTile; // Wybrany kafelek do umieszczenia

    void Start()
    {
        // Domy�lnie wybrana �cie�ka
        selectedTile = pathTile;
    }

    void Update()
    {
        if (Input.GetMouseButton(0)) // Klikni�cie lewym przyciskiem
        {
            // Pobierz pozycj� myszy w �wiecie
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Przekszta�� pozycj� �wiata na pozycj� w siatce Tilemap
            Vector3Int cellPosition = tilemap.WorldToCell(new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0));

            // Debug: Poka� pozycj� klikni�tej kom�rki w konsoli
            Debug.Log("Klikni�ta kom�rka: " + cellPosition);

            // Ustaw wybrany kafelek w klikni�tej pozycji
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
