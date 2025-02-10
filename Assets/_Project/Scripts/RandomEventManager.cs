using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEventManager : BaseManager<RandomEventManager>
{
    public int fireChance = 2; // 1% szansy na podpalenie atrakcji w ka¿dej jednostce czasu gry
    public float gameTimeInterval = 1000f; // Sprawdzanie zdarzeñ co 10 minut czasu gry (600 sekund w grze)
    private float lastEventTime = 0f;

    public GameObject firePrefab; // Przypisz prefab ognia w Inspectorze

    private void Update()
    {
        float gameTime = ClockUI.instance.GetGameTime();
        if (gameTime - lastEventTime >= gameTimeInterval)
        {
            TryTriggerRandomEvent();
            lastEventTime = gameTime;
        }
    }

    private void TryTriggerRandomEvent()
    {
        List<Attraction> attractions = Player.instance.attractionList;

        if (attractions.Count == 0)
            return;

        foreach (Attraction attraction in attractions)
        {
            if (Random.Range(0,4000) < fireChance)
            {
                TriggerFire(attraction);
            }
        }
    }

    public void TriggerFire(Attraction attraction)
    {
        Debug.Log($" {attraction.gameObject.name} stanê³a w p³omieniach!");
        attraction.isBroken = true;
        attraction.isOpen = false;

        if (attraction.fireInstance == null)
        {
            GameObject fireEffect = Instantiate(firePrefab, attraction.transform.position + new Vector3(0, 1, 0), Quaternion.identity, attraction.transform);
            attraction.fireInstance = fireEffect;
        }

        // Dodajemy kontroler rozprzestrzeniania siê ognia, jeœli go nie ma
        if (attraction.GetComponent<FireSpreadController>() == null)
        {
            attraction.gameObject.AddComponent<FireSpreadController>();
        }

        StartCoroutine(HandleFireExtinguish(attraction));
    }


    private IEnumerator HandleFireExtinguish(Attraction attraction)
    {
        float fireDuration = Random.Range(1200, 2400); // Po¿ar trwa losowo od 20 do 40 minut czasu gry
        float endFireTime = ClockUI.instance.GetGameTime() + fireDuration;

        while (ClockUI.instance.GetGameTime() < endFireTime)
        {
            yield return null;
        }

        ExtinguishFire(attraction);
    }

    private void ExtinguishFire(Attraction attraction)
    {
        Debug.Log($" Po¿ar w {attraction.gameObject.name} zosta³ ugaszony!");
        attraction.PerformMaintenance(); // Naprawa po ugaszeniu

        // Usuniêcie efektu ognia po ugaszeniu
        if (attraction.fireInstance != null)
        {
            Destroy(attraction.fireInstance);
            attraction.fireInstance = null;
        }
    }
}
