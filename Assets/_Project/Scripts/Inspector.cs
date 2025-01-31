using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inspector : PopUpWindow
{
    public TextMeshProUGUI attractionNameText;
    public InputField ticketCostInput;
    private Attraction selectedAttraction;
    private System.Action onCloseCallback;

    public void SetupInspector(Attraction attraction, System.Action onClose)
    {
        selectedAttraction = attraction;
        attractionNameText.text = attraction.structureName;
        ticketCostInput.text = attraction.ticketCost.ToString();
        onCloseCallback = onClose;
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
