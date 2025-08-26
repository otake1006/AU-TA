using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class RelicUIManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform relicGridParent;        // レリック表示用のグリッド
    public GameObject relicUIPrefab;         // レリックUI表示用プレハブ
    public Button addRelicButton;            // レリック追加ボタン
    public RelicSelectionPanel relicSelectionPanel; // レリック選択パネル

    [Header("Player Reference")]
    public Character playerCharacter;        // プレイヤーキャラクター

    private RelicManager playerRelicManager;
    private List<RelicUIElement> relicUIElements = new List<RelicUIElement>();

    void Start()
    {
        InitializeUI();
    }

    void InitializeUI()
    {
        if (playerCharacter != null)
        {
            playerRelicManager = playerCharacter.GetComponent<RelicManager>();
            if (playerRelicManager != null)
            {
                playerRelicManager.OnRelicListChanged += UpdateRelicDisplay;
            }
        }

        if (addRelicButton != null)
        {
            addRelicButton.onClick.AddListener(OpenRelicSelection);
        }

        // 初期表示を更新
        UpdateRelicDisplay(playerRelicManager?.GetActiveRelics() ?? new List<RelicEffect>());
    }

    public void OpenRelicSelection()
    {
        if (relicSelectionPanel != null)
        {
            relicSelectionPanel.ShowPanel(OnRelicSelected);
        }
    }

    void OnRelicSelected(RelicEffect selectedRelic)
    {
        if (playerRelicManager != null && selectedRelic != null)
        {
            playerRelicManager.AcquireRelic(selectedRelic);
        }
    }

    void UpdateRelicDisplay(List<RelicEffect> activeRelics)
    {
        // 既存のUI要素をクリア
        ClearRelicDisplay();

        // 新しいレリック表示を作成
        foreach (var relic in activeRelics)
        {
            CreateRelicUIElement(relic);
        }
    }

    void ClearRelicDisplay()
    {
        foreach (var uiElement in relicUIElements)
        {
            if (uiElement != null && uiElement.gameObject != null)
            {
                Destroy(uiElement.gameObject);
            }
        }
        relicUIElements.Clear();
    }

    void CreateRelicUIElement(RelicEffect relic)
    {
        if (relicUIPrefab != null && relicGridParent != null)
        {
            GameObject uiObject = Instantiate(relicUIPrefab, relicGridParent);
            RelicUIElement uiElement = uiObject.GetComponent<RelicUIElement>();

            if (uiElement != null)
            {
                uiElement.SetupRelic(relic, OnRelicRemoveClicked);
                relicUIElements.Add(uiElement);
            }
        }
    }

    void OnRelicRemoveClicked(RelicEffect relic)
    {
        if (playerRelicManager != null)
        {
            playerRelicManager.RemoveRelic(relic.relicID);
        }
    }

    void OnDestroy()
    {
        if (playerRelicManager != null)
        {
            playerRelicManager.OnRelicListChanged -= UpdateRelicDisplay;
        }
    }
}