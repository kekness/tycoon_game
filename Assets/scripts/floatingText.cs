using UnityEngine;
using TMPro;

public class floatingText : MonoBehaviour
{
    public float moveSpeed = 2f; // Szybko�� ruchu do g�ry
    public float fadeSpeed = 2f; // Szybko�� zanikania

    private TextMeshProUGUI textMesh;
    private Color originalColor;

    private void Start()
    {
        // Pobranie komponentu TextMeshPro
        textMesh = GetComponent<TextMeshProUGUI>();
        if (textMesh != null)
        {
            originalColor = textMesh.color; // Zapisanie pocz�tkowego koloru
        }
        else
        {
            Debug.LogError("Brak komponentu TextMeshPro na prefabrykacie!");
        }
    }

    private void Update()
    {
        // Ruch w g�r�
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        // Zanikanie tekstu
        if (textMesh != null)
        {
            // Pobierz aktualny kolor
            Color color = textMesh.color;
            // Zmniejsz alf�
            color.a -= fadeSpeed * Time.deltaTime;
            // Przypisz nowy kolor z powrotem do TextMeshPro
            textMesh.color = color;

            // Sprawd�, czy alfa osi�gn�a zero
            if (color.a <= 0)
            {
                Destroy(gameObject); // Usu� obiekt
            }
        }
    }
}
