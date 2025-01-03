using System.Collections;
using UnityEngine;

public class VisitorSpawner : MonoBehaviour
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
            // Stwórz odwiedzającego w pozycji bramy
            GameObject visitor = Instantiate(visitorPrefab, transform.position, Quaternion.identity);
            currentVisitors++;
        }
        else
        {
            Debug.LogError("Visitor prefab is not assigned!");
        }
    }
}
