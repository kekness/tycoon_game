using System.Collections;
using UnityEngine;

public class Gate : Structure
{
    public GameObject visitorPrefab;
    public float spawnInterval = 5f;
    public int maxVisitors = 10;
    private int currentVisitors = 0;

    void Start()
    {
        StartCoroutine(SpawnVisitors());
        PathManager.instance.registerPath(coordinates[0]);
    }

    private IEnumerator SpawnVisitors()
    {
        while (currentVisitors < maxVisitors)
        {
            SpawnVisitor();
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    private void SpawnVisitor()
    {
        if (visitorPrefab != null)
        {
            // Pobierz pozycjê bramy na siatce
            Vector3Int gateCellPosition = AttractionPlacer.instance.tilemap.WorldToCell(transform.position);

            // Umieœæ Visitora w œrodku komórki siatki
            Vector3 visitorPosition = AttractionPlacer.instance.tilemap.GetCellCenterWorld(gateCellPosition);
            GameObject visitorObject = Instantiate(visitorPrefab, visitorPosition, Quaternion.identity);

            Visitor visitor = visitorObject.GetComponent<Visitor>();

            Debug.Log($"Visitor spawned at grid position: {gateCellPosition}, world position: {visitorPosition}");

            visitor.MoveToRandomAttraction();
        }
        else
        {
            Debug.LogError("Visitor prefab is not assigned!");
        }
    }


}
