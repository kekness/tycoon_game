using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inspector : PopUpWindow
{
    public TextMeshProUGUI attractionNameText;
    public TextMeshProUGUI visitationsCount;
    public InputField ticketCostInput;
    public Image attractionImage;
    public Toggle isOpenToggle; // Publiczny Toggle przypisany w edytorze
    public Button repairButton;

    private Attraction selectedAttraction;
    private System.Action onCloseCallback;

    public void SetupInspector(Attraction attraction, System.Action onClose)
    {
        selectedAttraction = attraction;
        attractionNameText.text = attraction.structureName;
        visitationsCount.text = $"Dzisiaj odwiedzono {attraction.todaysVisitations} razy";
        ticketCostInput.text = attraction.ticketCost.ToString();

        // Ustawienie wartoœci Toggle na podstawie stanu atrakcji
        if (isOpenToggle != null)
        {
            if (attraction.isBroken == false)
            {
                isOpenToggle.isOn = attraction.isOpen;
                isOpenToggle.onValueChanged.AddListener((value) => {
                    selectedAttraction.isOpen = value;
                });
            }
            else
            {
                isOpenToggle.isOn = false;
                isOpenToggle.enabled = false;
            }
        }
        else
        {
            Debug.LogError("Toggle isOpenToggle is not assigned in the Inspector!");
        }

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

        // Ustawienie widocznoœci przycisku naprawy
        if (repairButton != null)
        {
            repairButton.gameObject.SetActive(attraction.isBroken);
        }

        onCloseCallback = onClose;
    }

    public void Update()
    {
        if (selectedAttraction != null)
        {
            visitationsCount.text = $"Dzisiaj odwiedzono {selectedAttraction.todaysVisitations} razy";

            // Aktualizacja widocznoœci przycisku naprawy w czasie rzeczywistym
            if (repairButton != null)
            {
                repairButton.gameObject.SetActive(selectedAttraction.isBroken);
            }
        }
    }
    public void UpdateTicketCost()
    {
        if (selectedAttraction != null && int.TryParse(ticketCostInput.text, out int newCost))
        {
            selectedAttraction.ticketCost = newCost;
        }

        // Aktualizacja stanu otwarcia/zamkniêcia atrakcji
        if (isOpenToggle != null)
        {
            selectedAttraction.isOpen = isOpenToggle.isOn;
        }

        CloseWindow();
    }
    public void repair()
    {
        if (Player.instance.balance >= 500)
        {
            Player.instance.balance -= 500;
            selectedAttraction.PerformMaintenance();

            // Po naprawie ukryj przycisk
            if (repairButton != null)
            {
                repairButton.gameObject.SetActive(false);
            }
        }
    }

}
