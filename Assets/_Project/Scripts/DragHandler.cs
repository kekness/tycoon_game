using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Mo¿esz dodaæ dodatkow¹ logikê na pocz¹tku przeci¹gania
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Przesuwanie okienka zgodnie z ruchem myszy
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
}
