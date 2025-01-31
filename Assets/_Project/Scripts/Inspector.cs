using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inspector : PopUpWindow
{
    public TextMeshProUGUI attractionNameText;
    public TextMeshProUGUI visitationsCount;
    public InputField ticketCostInput;
    private Attraction selectedAttraction;
    private System.Action onCloseCallback;

    public void SetupInspector(Attraction attraction, System.Action onClose)
    {
        selectedAttraction = attraction;
        attractionNameText.text = attraction.structureName;
        visitationsCount.text = $"Dzisiaj odwiedzono {attraction.todaysVisitations} razy";
        ticketCostInput.text = attraction.ticketCost.ToString();
        onCloseCallback = onClose;
    }
    public void Update()
    {
        visitationsCount.text = $"Dzisiaj odwiedzono {selectedAttraction.todaysVisitations} razy";
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
