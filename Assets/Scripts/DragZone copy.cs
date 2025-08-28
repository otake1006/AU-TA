using UnityEngine;
using UnityEngine.EventSystems;

public class DropTarget : MonoBehaviour, IDropHandler
{
    [Header("Drop Target Settings")]
    [SerializeField] private bool acceptMultipleItems = false; // 複数のアイテムを受け入れるかどうか
    [SerializeField] private bool disableDragAfterDrop = true; // ドロップ後にドラッグを無効化するかどうか
    [SerializeField] private GameObject nextCard;

    public void OnDrop(PointerEventData eventData)
    {
    }

    public void ShowNextPlaceholder()
    {
        nextCard.SetActive(true);
    } 
    // 既に子オブジェクトがある場合の処理
    public bool CanAcceptDrop()
    {
        if (acceptMultipleItems)
        {
            return true;
        }

        // 子オブジェクトがない場合のみ受け入れる
        return transform.childCount == 0;
    }
    
    // ドラッグ無効化設定を取得
    public bool ShouldDisableDragAfterDrop()
    {
        return disableDragAfterDrop;
    }
    
    // DropTarget内のアイテムのドラッグ状態を切り替える
    public void SetChildrenDraggable(bool draggable)
    {
        foreach (Transform child in transform)
        {
            DraggableItem draggableItem = child.GetComponent<DraggableItem>();
            if (draggableItem != null)
            {
                draggableItem.enabled = draggable;
            }
        }
    }
}