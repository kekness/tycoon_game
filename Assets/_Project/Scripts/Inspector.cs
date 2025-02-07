using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inspector : PopUpWindow
{
    public TextMeshProUGUI attractionNameText;
    public TextMeshProUGUI visitationsCount;
    public InputField ticketCostInput;
    public Image attractionImage;
    public Toggle isOpenToggle;
    public Button repairButton;
    public Button mainenaceButton;
    private Attraction selectedAttraction;
    private System.Action onCloseCallback;

    public void SetupInspector(Attraction attraction, System.Action onClose)
    {
        selectedAttraction = attraction;
        attractionNameText.text = attraction.structureName;
        visitationsCount.text = $"Dzisiaj odwiedzono {attraction.todaysVisitations} razy";
        ticketCostInput.text = attraction.ticketCost.ToString();

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

        if (attractionImage != null && attraction.attractionSprite != null)
        {
            attractionImage.sprite = attraction.attractionSprite;
            RectTransform imageRectTransform = attractionImage.GetComponent<RectTransform>();
            if (imageRectTransform != null)
            {
                imageRectTransform.localScale = new Vector3(1.5f, 1.5f, 1f);
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

        if (repairButton != null)
        {
            mainenaceButton.gameObject.SetActive(!attraction.isBroken);
            repairButton.gameObject.SetActive(attraction.isBroken);
        }

        if (mainenaceButton != null)
        {
            repairButton.gameObject.SetActive(attraction.isBroken);
            mainenaceButton.gameObject.SetActive(!attraction.isBroken);
        }

        onCloseCallback = onClose;
    }

    public void Update()
    {
        if (selectedAttraction != null)
        {
            visitationsCount.text = $"Dzisiaj odwiedzono {selectedAttraction.todaysVisitations} razy";

            if (repairButton != null)
            {
                bool isBroken = selectedAttraction.isBroken;
                mainenaceButton.gameObject.SetActive(!selectedAttraction.isBroken);
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

            if (repairButton != null)
            {
                repairButton.gameObject.SetActive(false);
            }

            if (mainenaceButton != null)
            {
                mainenaceButton.gameObject.SetActive(true);
            }
        }
    }

    public void mainenance()
    {
        if (Player.instance.balance >= 100)
        {
            Player.instance.balance -= 100;
            selectedAttraction.PerformMaintenance();

            if (mainenaceButton != null)
            {
                mainenaceButton.gameObject.SetActive(false);
            }
        }
    }
}
