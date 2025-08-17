using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform originalParent;
    private CanvasGroup canvasGroup;
    private Canvas canvas; // UI座標変換用

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        canvasGroup.blocksRaycasts = false; // ドロップ判定を通す
        transform.SetParent(canvas.transform); // 最前面に
    }

    public void OnDrag(PointerEventData eventData)
    {
        // マウス位置に追従
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        // ドロップされなかった場合は元に戻す
        if (transform.parent == canvas.transform)
        {
            transform.SetParent(originalParent);
        }
    }
}