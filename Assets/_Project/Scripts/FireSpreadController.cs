using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpreadController : MonoBehaviour
{
    public float spreadRadius = 2f; // Promie� rozprzestrzeniania si� ognia
    public float spreadTime = 600f; // Czas do rozprzestrzenienia si� ognia w sekundach czasu gry
    private bool isSpreading = false;
    private float fireStartTime; // Moment rozpocz�cia po�aru w czasie gry

    private Attraction attraction;

    private void Start()
    {
        attraction = GetComponent<Attraction>();
        fireStartTime = ClockUI.instance.GetGameTime(); // Pobieramy czas rozpocz�cia po�aru w czasie gry
        StartCoroutine(SpreadFireRoutine());
    }

    private IEnumerator SpreadFireRoutine()
    {
        while (true)
        {
            yield return null; // Czekamy jedn� klatk�, �eby nie blokowa� gry

            // Pobieramy aktualny czas gry
            float gameTime = ClockUI.instance.GetGameTime();

            // Je�li min�� czas rozprzestrzeniania, wywo�ujemy SpreadFire()
            if (gameTime >= fireStartTime + spreadTime)
            {
                if (attraction.isBroken && attraction.fireInstance != null)
                {
                    SpreadFire();
                }
                fireStartTime = gameTime; // Resetujemy licznik czasu rozprzestrzeniania
            }
        }
    }

    private void SpreadFire()
    {
        Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(transform.position, spreadRadius);

        foreach (Collider2D obj in nearbyObjects)
        {
            Attraction nearbyAttraction = obj.GetComponent<Attraction>();

            if (nearbyAttraction != null && !nearbyAttraction.isBroken)
            {
                Debug.Log($"{nearbyAttraction.gameObject.name} zapali�a si� od {attraction.gameObject.name}!");
                RandomEventManager.instance.TriggerFire(nearbyAttraction);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spreadRadius);
    }
}
