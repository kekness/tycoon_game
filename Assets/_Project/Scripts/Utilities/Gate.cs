using System.Collections;
using UnityEngine;

public class Gate : Structure
{
    public GameObject visitorPrefab;
    public float spawnInterval = 5f;
    public int maxVisitors = 1;
    public int currentVisitors = 0;

    void Start()
    {
        StartCoroutine(SpawnVisitors());
        PathManager.instance.registerPath(coordinates[0]);
        PathManager.instance.registerPath(coordinates[1]);
        Player.instance.gates.Add(this);
    }

    private IEnumerator SpawnVisitors()
    {
        while (currentVisitors < maxVisitors)
        {
            SpawnVisitor();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void visit(Visitor visitor)
    {
        visitor.currentState = VisitorStates.IDLE;
        Vector2Int targetPosition = this.coordinates[0];
        visitor.MoveNPC(visitor.GetCurrentGridPosition(), targetPosition);
        StartCoroutine(WaitForVisitorToArrive(visitor, targetPosition));
    }

    private IEnumerator WaitForVisitorToArrive(Visitor visitor, Vector2Int targetGridPos)
    {
        // Czekaj a¿ visitor dotrze do celu
        while (visitor.GetCurrentGridPosition() != targetGridPos)
        {
            yield return null;
        }
        Destroy(visitor.gameObject);
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
            Instantiate(visitorPrefab, visitorPosition, Quaternion.identity);
            currentVisitors++;
        }
        else
        {
            Debug.LogError("Visitor prefab is not assigned!");  
        }
    }

    public void newDay()
    {
        currentVisitors = 0;
    }
}
