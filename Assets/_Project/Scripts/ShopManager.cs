using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopManager : PopUpWindow
{
    [Header("Shop Elements")]
    public Transform shopContent; // Kontener na przedmioty sklepu
    public GameObject shopItemPrefab; // Prefab pojedynczego przedmiotu
    public Button closeButton; // Przycisk zamykaj¹cy sklep



    public void InitializeShop()
    {
        if (shopContent == null)
        {
            shopContent = transform.Find("Panel/Scroll View/Viewport/Content");

            if (shopContent == null)
            {
                Debug.LogError("ShopContent nie znaleziony w ShopManager!");
                return;
            }
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(() => gameObject.SetActive(false));
        }

        PopulateShop();
        gameObject.SetActive(false); // Domyœlnie ukryj sklep
    }




    public void PopulateShop()
    {
        // Czyszczenie poprzednich elementów
        foreach (Transform child in shopContent)
        {
            Destroy(child.gameObject);
        }

        // Dodawanie nowych elementów
        for (int i = 0; i < UIManager.instance.availableShopItems.Count; i++)
        {
            GameObject newItem = Instantiate(shopItemPrefab, shopContent);
            ShopItemUI itemUI = newItem.GetComponent<ShopItemUI>();

            if (itemUI != null)
            {
                itemUI.Setup(UIManager.instance.availableShopItems[i], i); // Przekazujemy obiekt i indeks
            }
            else
            {
                Debug.LogError("ShopItemUI nie znaleziony w prefabrykacie!");
            }
        }
    }


}
