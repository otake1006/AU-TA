using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private GameObject dragCopy;
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvas = FindObjectOfType<Canvas>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 元のオブジェクトは残すのでコピーを生成
        dragCopy = Instantiate(gameObject, canvas.transform);
        canvasGroup = dragCopy.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = dragCopy.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false; // ドロップ判定を通す
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragCopy != null)
        {
            RectTransform copyRect = dragCopy.GetComponent<RectTransform>();
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out pos);
            copyRect.anchoredPosition = pos;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragCopy != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            dragCopy = null; // コピーをそのまま残す
        }
    }
}
