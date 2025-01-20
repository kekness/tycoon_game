using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inspector : MonoBehaviour
{
    public GameObject inspectorPanel;
    public TextMeshProUGUI attractionNameText;
    public InputField ticketCostInput;
    private Image attractionImage;
    private Attraction selectedAttraction;

    private void Start()
    {
        inspectorPanel.SetActive(false);
    }

    public void ShowInspector(Attraction attraction)
    {
        selectedAttraction = attraction;
        attractionNameText.text = attraction.structureName;
        ticketCostInput.text = attraction.ticketCost.ToString();
        //attractionImage.sprite = attraction.sprite;
        inspectorPanel.SetActive(true);
    }

    public void HideInspector()
    {
        inspectorPanel.SetActive(false);
        selectedAttraction = null;
    }

    public void UpdateTicketCost()
    {
        if (selectedAttraction != null && int.TryParse(ticketCostInput.text, out int newCost))
        {
            selectedAttraction.ticketCost = newCost;
        }
        inspectorPanel.SetActive(false);
    }
}