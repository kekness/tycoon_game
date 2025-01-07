using System.Collections;
using UnityEngine;

public class Gate : Building
{
    public GameObject visitorPrefab;
    public float spawnInterval = 5f;
    public int maxVisitors = 10;
    private int currentVisitors = 0;

    void Start()
    {
        StartCoroutine(SpawnVisitors());
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
            // Stwórz odwiedzaj¹cego w pozycji bramy
            GameObject visitor = Instantiate(visitorPrefab, transform.position, Quaternion.identity);
            currentVisitors++;
        }
        else
        {
            Debug.LogError("Visitor prefab is not assigned!");
        }
    }
}
