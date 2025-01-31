using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockUI : BaseManager<ClockUI>
{
    public RectTransform hourHand;
    public RectTransform minuteHand;

    private float gameTime = 0.0f;
    private float gameTimeScale = 100.0f;
    private List<int> accList = new List<int> { 1, 2, 4, 8, 16, 32, 64 };
    private int accIndex = 0;

    private bool hasDisplayedHeatmap = false; // Flaga, ¿eby nie wywo³ywaæ wielokrotnie

    private void Start()
    {
        NewDay();
    }

    private void Awake()
    {
        base.InitializeManager();
    }

    void Update()
    {
        // Aktualizacja czasu gry
        gameTime += Time.deltaTime * gameTimeScale * accList[accIndex];

        // Obliczanie godzin i minut
        float gameHours = (gameTime / 3600.0f) % 24; // Czas w godzinach (0-23)
        float gameMinutes = (gameTime / 60.0f) % 60; // Minuty

        // Obracanie wskazówek zegara
        hourHand.localRotation = Quaternion.Euler(0, 0, -gameHours * 30.0f);
        minuteHand.localRotation = Quaternion.Euler(0, 0, -gameMinutes * 6.0f);

        // Testowe wywo³anie nowego dnia (np. po naciœniêciu klawisza N)
        if (Input.GetKeyDown(KeyCode.N))
        {
            NewDay();
        }

        // Automatyczne wywo³anie podsumowania dnia o godzinie 22:00
        if (gameHours >= 22 && !hasDisplayedHeatmap)
        {
            hasDisplayedHeatmap = true;
            UIManager.instance.ShowDaySummary(); // Wyœwietl podsumowanie dnia (w tym heatmapê)
        }
    }

    // Resetowanie czasu do 8:00 rano
    void NewDay()
    {
        gameTime = 8 * 3600; // Reset do 8:00 rano
        hasDisplayedHeatmap = false; // Reset flagi
    }

    // Przyspieszenie czasu
    public void accelerate()
    {
        if (accIndex < accList.Count - 1)
            accIndex++;
        else
            accIndex = 0;
    }

    // Resetowanie przyspieszenia
    public void resetAcceleration()
    {
        accIndex = 0;
    }

    // Pobranie aktualnego czasu gry
    public float GetGameTime()
    {
        return gameTime;
    }

    // Pobranie aktualnego przyspieszenia
    public float getAcceleration()
    {
        return Time.deltaTime * gameTimeScale * accList[accIndex];
    }
}