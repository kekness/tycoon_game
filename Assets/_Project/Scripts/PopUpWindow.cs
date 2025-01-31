using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpWindow : MonoBehaviour
{
    public void CloseWindow()
    {
        Destroy(gameObject);
    }
}
