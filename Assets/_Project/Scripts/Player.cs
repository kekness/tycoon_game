
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : BaseManager<Player>
{
    public int numberOfVisitors = 0;
    public int DayNumber = 1;
    public float balance = 9999999;
    public float todaysEarnings = 0;
    public float todaysExpenses=0;
    public TextMeshProUGUI balanceText;
    public List<Attraction> attractionList = new List<Attraction>();
    public void Awake()
    {
        base.InitializeManager();
    }
    void Update()
    {

        if (balanceText != null)
        {
            balanceText.text = "$" + balance.ToString();
        }

    }
    public Attraction GetRandomAttraction()
    {
        if (attractionList.Count == 0)
        {
            return null; // Nie ma atrakcji w grze
        }
        return attractionList[Random.Range(0, attractionList.Count)]; // Losowy wybór atrakcji
    }
   public void pay(float money)
    {
        balance -=money;
        todaysExpenses +=money;
    }
    public void getMoney(float money)
    {
        balance += money;
        todaysEarnings += money;
    }
    public Attraction mostPopularAttraction()
    {
        if (attractionList == null || attractionList.Count == 0)
        {
            Debug.LogError("Attraction list is empty or null!");
            return null; 
        }

        Attraction mostPopular = null;
        int maxVisitations = int.MinValue;

        foreach (var attraction in attractionList)
            if (attraction.todaysVisitations > maxVisitations)
            {
                mostPopular = attraction;
                maxVisitations = attraction.todaysVisitations;
            }
        
        return mostPopular;
    }

}
