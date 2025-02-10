using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpreadController : MonoBehaviour
{
    public float spreadRadius = 2f; // Promieñ rozprzestrzeniania siê ognia
    public float spreadTime = 600f; // Czas do rozprzestrzenienia siê ognia w sekundach czasu gry
    private bool isSpreading = false;
    private float fireStartTime; // Moment rozpoczêcia po¿aru w czasie gry

    private Attraction attraction;

    private void Start()
    {
        attraction = GetComponent<Attraction>();
        fireStartTime = ClockUI.instance.GetGameTime(); // Pobieramy czas rozpoczêcia po¿aru w czasie gry
        StartCoroutine(SpreadFireRoutine());
    }

    private IEnumerator SpreadFireRoutine()
    {
        while (true)
        {
            yield return null; // Czekamy jedn¹ klatkê, ¿eby nie blokowaæ gry

            // Pobieramy aktualny czas gry
            float gameTime = ClockUI.instance.GetGameTime();

            // Jeœli min¹³ czas rozprzestrzeniania, wywo³ujemy SpreadFire()
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
                Debug.Log($"{nearbyAttraction.gameObject.name} zapali³a siê od {attraction.gameObject.name}!");
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
