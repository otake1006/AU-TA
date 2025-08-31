using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RelicUIElement : MonoBehaviour
{
    [Header("UI Components")]
    public Image relicIcon;
    public TextMeshProUGUI relicNameText;
    public TextMeshProUGUI stackCountText;
    public Button removeButton;
    public Button detailButton;

    [Header("Rarity Colors")]
    public Color commonColor = Color.white;
    public Color uncommonColor = Color.green;
    public Color rareColor = Color.blue;
    public Color epicColor = Color.magenta;
    public Color legendaryColor = Color.yellow;

    private RelicEffect currentRelic;
    private System.Action<RelicEffect> onRemoveCallback;

    public void SetupRelic(RelicEffect relic, System.Action<RelicEffect> onRemove = null)
    {
        currentRelic = relic;
        onRemoveCallback = onRemove;

        UpdateDisplay();
        SetupButtons();
    }

    void UpdateDisplay()
    {
        if (currentRelic == null) return;

        // アイコン設定
        if (relicIcon != null && currentRelic.relicIcon != null)
        {
            relicIcon.sprite = currentRelic.relicIcon;
            relicIcon.color = GetRarityColor(currentRelic.rarity);
        }

        // 名前設定
        if (relicNameText != null)
        {
            relicNameText.text = currentRelic.relicName;
        }

        // スタック数表示
        if (stackCountText != null)
        {
            if (currentRelic.canStack && currentRelic.stackCount > 1)
            {
                stackCountText.text = currentRelic.stackCount.ToString();
                stackCountText.gameObject.SetActive(true);
            }
            else
            {
                stackCountText.gameObject.SetActive(false);
            }
        }
    }

    void SetupButtons()
    {
        if (removeButton != null)
        {
            removeButton.onClick.RemoveAllListeners();
            removeButton.onClick.AddListener(() => onRemoveCallback?.Invoke(currentRelic));
        }

        if (detailButton != null)
        {
            detailButton.onClick.RemoveAllListeners();
            detailButton.onClick.AddListener(ShowRelicDetails);
        }
    }

    void ShowRelicDetails()
    {
        if (currentRelic != null)
        {
            // レリック詳細パネルを表示
            // RelicDetailPanel.Instance?.ShowRelic(currentRelic);
        }
    }

    Color GetRarityColor(RelicRarity rarity)
    {
        return rarity switch
        {
            RelicRarity.Common => commonColor,
            RelicRarity.Uncommon => uncommonColor,
            RelicRarity.Rare => rareColor,
            RelicRarity.Epic => epicColor,
            RelicRarity.Legendary => legendaryColor,
            _ => commonColor
        };
    }
}