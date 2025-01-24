using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockUI : MonoBehaviour
{
    public RectTransform hourHand;
    public RectTransform minuteHand;

    private float gameTime = 0.0f; 
    private float gameTimeScale = 100.0f;
    private List<int> accList = new List<int> { 1, 2, 4, 8, 16, 32, 64};
    private int accIndex=0;

    private void Start()
    {
        newDay(); 
    }

    void Update()
    {

        gameTime += Time.deltaTime * gameTimeScale*accList[accIndex];

        float gameHours = (gameTime / 3600.0f) % 12;
        float gameMinutes = (gameTime / 60.0f) % 60;

  
        float hourAngle = gameHours * 30.0f; 
        float minuteAngle = gameMinutes * 6.0f; 

        
        hourHand.localRotation = Quaternion.Euler(0, 0, -hourAngle);
        minuteHand.localRotation = Quaternion.Euler(0, 0, -minuteAngle);


        if (Input.GetKeyDown(KeyCode.N))
        {
            newDay();
        }
    }

    void newDay()
    {
      
        float targetHours = 8.0f; // Godzina 8
        float targetMinutes = 0.0f; // Minuta 0

        gameTime = (targetHours * 3600.0f) + (targetMinutes * 60.0f);

        float hourAngle = targetHours * 30.0f; 
        float minuteAngle = targetMinutes * 6.0f; 

        hourHand.localRotation = Quaternion.Euler(0, 0, -hourAngle);
        minuteHand.localRotation = Quaternion.Euler(0, 0, -minuteAngle);
    }
    public void accelerate()
    {
        Debug.Log(accList.Count);
        if (accIndex < accList.Count-1)
            accIndex++;
        else
            accIndex = 0;
    }
    public void reset()
    {
        accIndex = 0;
    }
}