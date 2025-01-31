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
        Player.instance.gate = this;
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
        if (visitorPrefab != null && !ClockUI.instance.isTimeStopped)
        {
            Player.instance.numberOfVisitors++;
            // Pobierz pozycjê bramy na siatce
            Vector3Int gateCellPosition = AttractionPlacer.instance.tilemap.WorldToCell(transform.position);

            // Umieœæ Visitora w œrodku komórki siatki
            Vector3 visitorPosition = AttractionPlacer.instance.tilemap.GetCellCenterWorld(gateCellPosition);
            GameObject visitorObject = Instantiate(visitorPrefab, visitorPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Visitor prefab is not assigned!");
        }
    }


}
