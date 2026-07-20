using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Item ID")]
    public string itemID;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;

    private Vector2 dragStartPosition;
    private Transform dragStartParent;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>(true);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvas == null)
            return;

        dragStartPosition = rectTransform.anchoredPosition;
        dragStartParent = transform.parent;

        canvasGroup.blocksRaycasts = false;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null)
            return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        rectTransform.anchoredPosition = dragStartPosition;
        transform.SetParent(dragStartParent, false);
    }

    public void SnapTo(Transform target)
    {
        transform.SetParent(target, false);
        rectTransform.anchoredPosition = Vector2.zero;
        canvasGroup.blocksRaycasts = true;
    }
}