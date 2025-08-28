using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private GameObject dragCopy; // ドラッグ用のコピー
    private CanvasGroup dragCopyCanvasGroup;

    

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // DropTarget内にいる場合はドラッグを許可しない
        DropTarget parentDropTarget = GetComponentInParent<DropTarget>();
        if (parentDropTarget != null && parentDropTarget.ShouldDisableDragAfterDrop())
        {
            return; // ドラッグを開始しない
        }
        
        // 元のオブジェクトのコピーを作成
        dragCopy = Instantiate(gameObject, canvas.transform);
        
        // コピーしたオブジェクトの設定
        dragCopyCanvasGroup = dragCopy.GetComponent<CanvasGroup>();
        if (dragCopyCanvasGroup == null)
        {
            dragCopyCanvasGroup = dragCopy.AddComponent<CanvasGroup>();
        }
        
        // ドロップ判定を通すために、コピーのレイキャストを無効化
        dragCopyCanvasGroup.blocksRaycasts = false;
        
        // コピーのDraggableItemコンポーネントを無効化（無限ループを防ぐ）
        DraggableItem copyDraggable = dragCopy.GetComponent<DraggableItem>();
        if (copyDraggable != null)
        {
            copyDraggable.enabled = false;
        }
        
        // コピーを最前面に移動
        dragCopy.transform.SetAsLastSibling();
        
        // コピーの初期位置をマウス位置に設定
        dragCopy.transform.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragCopy != null)
        {
            // コピーをマウス位置に追従させる
            dragCopy.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragCopy != null)
        {
            // ドロップターゲットを確認
            bool droppedOnValidTarget = false;
            
            // レイキャストでドロップターゲットを検出
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = eventData.position;
            
            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            
            foreach (var result in results)
            {
                // DropTargetコンポーネントを持つオブジェクトを探す
                DropTarget dropTarget = result.gameObject.GetComponent<DropTarget>();
                if (dropTarget != null && dropTarget.CanAcceptDrop())
                {
                    // 有効なドロップターゲットに配置
                    dragCopy.transform.SetParent(dropTarget.transform);
                    dragCopy.transform.localPosition = Vector3.zero;
                    
                    // CanvasGroupの設定を元に戻す
                    dragCopyCanvasGroup.blocksRaycasts = true;
                    
                    // DropTargetの設定に従ってドラッグ可能かどうかを決定
                    DraggableItem copyDraggable = dragCopy.GetComponent<DraggableItem>();
                    if (copyDraggable != null)
                    {
                        copyDraggable.enabled = !dropTarget.ShouldDisableDragAfterDrop();
                    }
                    
                    droppedOnValidTarget = true;

                    dropTarget.ShowNextPlaceholder();
                    dropTarget.GetComponentInParent<ActtiveDrag>().enabled = true;

                    break;
                }
            }
            
            // 有効なドロップターゲットに配置されなかった場合はコピーを削除
            if (!droppedOnValidTarget)
            {
                Destroy(dragCopy);
            }
            
            dragCopy = null;
        }
    }
}