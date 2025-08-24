using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RelicPanelUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform relicIconContainer;
    public GameObject relicIconPrefab;
    public ScrollRect scrollRect;
    public LayoutGroup layoutGroup;

    [Header("Settings")]
    public int maxDisplayedRelics = 12;
    public bool autoHideWhenEmpty = true;

    private List<RelicIconUI> relicIcons = new List<RelicIconUI>();
    private Character targetCharacter;
    private RelicManager relicManager;

    void Start()
    {
        relicManager = RelicManager.Instance;
        SetupEventListeners();
        
        if (autoHideWhenEmpty)
        {
            gameObject.SetActive(false);
        }
    }

    void SetupEventListeners()
    {
        GameEvents.OnRelicAcquired += OnRelicAcquired;
        GameEvents.OnRelicLost += OnRelicLost;
    }

    public void SetTargetCharacter(Character character)
    {
        targetCharacter = character;
        RefreshRelicDisplay();
    }

    void OnRelicAcquired(Character character, RelicEffect relic)
    {
        if (character == targetCharacter)
        {
            RefreshRelicDisplay();
        }
    }

    void OnRelicLost(Character character, RelicEffect relic)
    {
        if (character == targetCharacter)
        {
            RefreshRelicDisplay();
        }
    }

    public void RefreshRelicDisplay()
    {
        if (targetCharacter == null || relicManager == null) return;

        var characterRelics = relicManager.GetCharacterRelics(targetCharacter);
        
        // 表示/非表示の切り替え
        if (autoHideWhenEmpty)
        {
            gameObject.SetActive(characterRelics.Count > 0);
        }

        // 不要なアイコンを削除
        ClearExcessIcons(characterRelics.Count);

        // 必要なアイコンを作成/更新
        for (int i = 0; i < characterRelics.Count && i < maxDisplayedRelics; i++)
        {
            var relic = characterRelics[i];
            var iconUI = GetOrCreateRelicIcon(i);
            iconUI.SetupRelicIcon(relic);
        }

        // レイアウト更新
        if (layoutGroup != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(relicIconContainer as RectTransform);
        }
    }

    RelicIconUI GetOrCreateRelicIcon(int index)
    {
        // 既存のアイコンがある場合は再利用
        if (index < relicIcons.Count && relicIcons[index] != null)
        {
            return relicIcons[index];
        }

        // 新しいアイコンを作成
        if (relicIconPrefab != null && relicIconContainer != null)
        {
            var iconObject = Instantiate(relicIconPrefab, relicIconContainer);
            var iconUI = iconObject.GetComponent<RelicIconUI>();
            
            if (iconUI != null)
            {
                // リストのサイズを調整
                while (relicIcons.Count <= index)
                {
                    relicIcons.Add(null);
                }
                
                relicIcons[index] = iconUI;
                return iconUI;
            }
        }

        return null;
    }

    void ClearExcessIcons(int requiredCount)
    {
        // 必要以上のアイコンを削除
        for (int i = requiredCount; i < relicIcons.Count; i++)
        {
            if (relicIcons[i] != null)
            {
                DestroyImmediate(relicIcons[i].gameObject);
            }
        }

        // リストのサイズを調整
        if (relicIcons.Count > requiredCount)
        {
            relicIcons.RemoveRange(requiredCount, relicIcons.Count - requiredCount);
        }
    }

    public void RefreshAllIcons()
    {
        foreach (var icon in relicIcons)
        {
            if (icon != null)
            {
                icon.RefreshIcon();
            }
        }
    }

    // デバッグ用メソッド
    [ContextMenu("Force Refresh Display")]
    public void ForceRefreshDisplay()
    {
        RefreshRelicDisplay();
    }

    [ContextMenu("Clear All Icons")]
    public void DebugClearAllIcons()
    {
        ClearExcessIcons(0);
    }

    void OnDestroy()
    {
        // イベントリスナー削除
        GameEvents.OnRelicAcquired -= OnRelicAcquired;
        GameEvents.OnRelicLost -= OnRelicLost;
    }
}