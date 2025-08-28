using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        // ドラッグされているオブジェクトを取得
        DraggableItem draggableItem = eventData.pointerDrag?.GetComponent<DraggableItem>();

        if (draggableItem != null)
        {
            // アイテムをこのDropZone（Content）の子にする
            draggableItem.transform.SetParent(transform, false);

            // Content Size Fitterの更新を強制
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }
    }
}