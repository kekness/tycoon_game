using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inspector : PopUpWindow
{
    public TextMeshProUGUI attractionNameText;
    public TextMeshProUGUI visitationsCount;
    public InputField ticketCostInput;
    public Image attractionImage; // Dodane pole do wyœwietlania sprite'a atrakcji

    private Attraction selectedAttraction;
    private System.Action onCloseCallback;

    public void SetupInspector(Attraction attraction, System.Action onClose)
    {
        selectedAttraction = attraction;
        attractionNameText.text = attraction.structureName;
        visitationsCount.text = $"Dzisiaj odwiedzono {attraction.todaysVisitations} razy";
        ticketCostInput.text = attraction.ticketCost.ToString();

        // Ustawienie sprite'a atrakcji w komponencie Image
        if (attractionImage != null && attraction.attractionSprite != null)
        {
            attractionImage.sprite = attraction.attractionSprite;

            // Powiêkszenie sprite'a
            RectTransform imageRectTransform = attractionImage.GetComponent<RectTransform>();
            if (imageRectTransform != null)
            {
                imageRectTransform.localScale = new Vector3(1.5f, 1.5f, 1f); // Powiêkszenie o 50%
            }
            else
            {
                Debug.LogError("RectTransform not found on attractionImage!");
            }
        }
        else
        {
            Debug.LogError("Attraction Image or Sprite is not assigned!");
        }

        onCloseCallback = onClose;
    }

    public void Update()
    {
        if (selectedAttraction != null)
        {
            visitationsCount.text = $"Dzisiaj odwiedzono {selectedAttraction.todaysVisitations} razy";
        }
    }

    public void UpdateTicketCost()
    {
        if (selectedAttraction != null && int.TryParse(ticketCostInput.text, out int newCost))
        {
            selectedAttraction.ticketCost = newCost;
        }
        CloseWindow();
    }
}