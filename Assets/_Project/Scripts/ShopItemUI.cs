using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    public Image itemImage;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemPrice;
    public Button buyButton;

    private int attractionIndex;

    public void Setup(GameObject structure, int index)
    {
        attractionIndex = index;
        itemImage.sprite = structure.GetComponent<SpriteRenderer>().sprite;
        Structure str = structure.GetComponent<Structure>();
        if (str != null)
        {
            itemName.text = str.structureName;
            itemPrice.text = $"{str.cost}$";
        }


        buyButton.onClick.AddListener(() => BuyItem());
        buyButton.onClick.AddListener(() => CloseShopPanel());
    }
    private void CloseShopPanel()
    {
        ShopManager shopManager = FindObjectOfType<ShopManager>();
        if (shopManager != null)
            shopManager.CloseWindow(); // Zamyka sklep
        
    }
        private void BuyItem()
    {

        AttractionPlacer.instance.SelectAttraction(attractionIndex);
        
    }
}
