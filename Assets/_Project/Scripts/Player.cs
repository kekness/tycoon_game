using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int balance = 9999999;
    public TextMeshProUGUI balanceText;
    void Update()
    {
        if (balanceText != null)
        {
            balanceText.text = "$" + balance.ToString();
        }
    }
}
