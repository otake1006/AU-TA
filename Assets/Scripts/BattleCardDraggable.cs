using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BattleDraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Transform originalParent;
    private int originalSiblingIndex;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = FindFirstObjectByType<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 元の位置を記録
        originalParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();

        // ドラッグ中の見た目を変更
        canvasGroup.alpha = 0.8f;
        canvasGroup.blocksRaycasts = false;

        // 一番上に表示するためにCanvasの直下に移動
        transform.SetParent(canvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // マウスに追従
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 見た目を元に戻す
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // ドロップ位置を確認
        GameObject hitObject = eventData.pointerCurrentRaycast.gameObject;

        if (hitObject != null)
        {
            // DropZone（Content）にドロップされた場合
            DropZone dropZone = hitObject.GetComponentInParent<DropZone>();
            if (dropZone != null)
            {
                // 新しい位置に配置
                transform.SetParent(dropZone.transform, false);

                // 他のアイテムとの位置関係を調整
                AdjustPosition(eventData.position);
                return;
            }
        }

        // 有効なドロップ先がない場合は元の位置に戻す
        transform.SetParent(originalParent, false);
        transform.SetSiblingIndex(originalSiblingIndex);
    }

    private void AdjustPosition(Vector2 dropPosition)
    {
        DropZone dropZone = GetComponentInParent<DropZone>();
        if (dropZone == null) return;

        int newSiblingIndex = 0;

        // 他の子オブジェクトと比較して適切な位置を見つける
        for (int i = 0; i < dropZone.transform.childCount; i++)
        {
            RectTransform child = dropZone.transform.GetChild(i) as RectTransform;
            if (child == rectTransform) continue;

            Vector2 childWorldPos = child.TransformPoint(child.rect.center);

            // Y座標で比較（垂直スクロール用）
            if (dropPosition.y > childWorldPos.y)
            {
                newSiblingIndex = i;
                break;
            }
            else
            {
                newSiblingIndex = i + 1;
            }
        }

        transform.SetSiblingIndex(newSiblingIndex);
    }
}