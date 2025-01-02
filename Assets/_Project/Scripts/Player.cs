
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int balance = 9999999;
    public TextMeshProUGUI balanceText;
    public List<Attraction> attractionList = new List<Attraction>();
    

    void Update()
    {
        if (balanceText != null)
        {
            balanceText.text = "$" + balance.ToString();
        }
    }
}
