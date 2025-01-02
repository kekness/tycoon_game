using System.Collections;
using UnityEngine;

public class Visitor : MonoBehaviour
{
    public float speed = 2f; // Pr�dko�� poruszania si� odwiedzaj�cego
    private Attraction targetAttraction;
    public int hunger;
    public int thirst;
    public int happiness;
    public int disgust;
    public int fear;

    void Start()
    {
        ChooseRandomAttraction();
    }

    void Update()
    {
        if (targetAttraction != null)
        {
            MoveTowardsAttraction();
        }
    }

    private void ChooseRandomAttraction()
    {
        if (FindObjectOfType<Player>().attractionList.Count > 0)
        {
            int randomIndex = Random.Range(0, FindObjectOfType<Player>().attractionList.Count);
            targetAttraction = FindObjectOfType<Player>().attractionList[randomIndex];
        }
    }

    private void MoveTowardsAttraction()
    {
        Vector3 targetPosition = targetAttraction.transform.position;

        // Ruch w kierunku atrakcji
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Sprawd�, czy odwiedzaj�cy dotar� do atrakcji
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            StartCoroutine(StayAtAttraction());
        }
    }

    private IEnumerator StayAtAttraction()
    {
        // Odwiedzaj�cy sp�dza chwil� przy atrakcji
        yield return new WaitForSeconds(Random.Range(2, 5));

        // Wyb�r nowej atrakcji
        ChooseRandomAttraction();
    }
}
