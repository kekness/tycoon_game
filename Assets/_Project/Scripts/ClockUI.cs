using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockUI : MonoBehaviour
{
    public RectTransform hourHand;
    public RectTransform minuteHand;

    private float gameTime = 0.0f; // Czas gry w sekundach
    private float gameTimeScale = 10000.0f; // Skala czasu gry

    private void Start()
    {
        newDay(); // Ustaw zegar na 8:00 na pocz¹tku gry
    }

    void Update()
    {
        // Aktualizuj czas gry
        gameTime += Time.deltaTime * gameTimeScale;

        // Oblicz godziny i minuty
        float gameHours = (gameTime / 3600.0f) % 12;
        float gameMinutes = (gameTime / 60.0f) % 60;

        // Oblicz k¹ty dla wskazówek
        float hourAngle = gameHours * 30.0f; // 360 stopni / 12 godzin = 30 stopni na godzinê
        float minuteAngle = gameMinutes * 6.0f; // 360 stopni / 60 minut = 6 stopni na minutê

        // Ustaw obroty wskazówek
        hourHand.localRotation = Quaternion.Euler(0, 0, -hourAngle);
        minuteHand.localRotation = Quaternion.Euler(0, 0, -minuteAngle);

        // Przyk³ad: Wywo³aj newDay() po naciœniêciu klawisza "N"
        if (Input.GetKeyDown(KeyCode.N))
        {
            newDay();
        }
    }

    void newDay()
    {
        // Ustawienie czasu na 8:00
        float targetHours = 8.0f; // Godzina 8
        float targetMinutes = 0.0f; // Minuta 0

        // Oblicz czas w sekundach, który odpowiada 8:00
        gameTime = (targetHours * 3600.0f) + (targetMinutes * 60.0f);

        // Ustawienie wskazówek na 8:00
        float hourAngle = targetHours * 30.0f; // 8 godzin * 30 stopni na godzinê
        float minuteAngle = targetMinutes * 6.0f; // 0 minut * 6 stopni na minutê

        hourHand.localRotation = Quaternion.Euler(0, 0, -hourAngle);
        minuteHand.localRotation = Quaternion.Euler(0, 0, -minuteAngle);
    }
}