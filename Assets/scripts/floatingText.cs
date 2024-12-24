using UnityEngine;
using TMPro;

public class floatingText : MonoBehaviour
{
    public float moveSpeed = 2f; // Szybkoœæ ruchu do góry
    public float fadeSpeed = 2f; // Szybkoœæ zanikania

    private TextMeshProUGUI textMesh;
    private Color originalColor;

    private void Start()
    {
        // Pobranie komponentu TextMeshPro
        textMesh = GetComponent<TextMeshProUGUI>();
        if (textMesh != null)
        {
            originalColor = textMesh.color; // Zapisanie pocz¹tkowego koloru
        }
        else
        {
            Debug.LogError("Brak komponentu TextMeshPro na prefabrykacie!");
        }
    }

    private void Update()
    {
        // Ruch w górê
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        // Zanikanie tekstu
        if (textMesh != null)
        {
            // Pobierz aktualny kolor
            Color color = textMesh.color;
            // Zmniejsz alfê
            color.a -= fadeSpeed * Time.deltaTime;
            // Przypisz nowy kolor z powrotem do TextMeshPro
            textMesh.color = color;

            // SprawdŸ, czy alfa osi¹gnê³a zero
            if (color.a <= 0)
            {
                Destroy(gameObject); // Usuñ obiekt
            }
        }
    }
}
