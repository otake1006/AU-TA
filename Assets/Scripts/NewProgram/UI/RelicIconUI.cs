using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class RelicIconUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    public Image relicIcon;
    public Image rarityBorder;
    public TextMeshProUGUI stackCountText;
    public GameObject stackCountObject;

    [Header("Rarity Colors")]
    public Color commonColor = Color.white;
    public Color uncommonColor = Color.green;
    public Color rareColor = Color.blue;
    public Color epicColor = Color.magenta;
    public Color legendaryColor = Color.yellow;

    private RelicEffect currentRelic;
    private TooltipManager tooltipManager;

    void Start()
    {
        tooltipManager = FindObjectOfType<TooltipManager>();
        stackCountObject?.SetActive(false);
    }

    public void SetupRelicIcon(RelicEffect relic)
    {
        if (relic == null)
        {
            gameObject.SetActive(false);
            return;
        }

        currentRelic = relic;
        gameObject.SetActive(true);

        // アイコン設定
        if (relicIcon != null && relic.buffIcon != null)
        {
            relicIcon.sprite = relic.buffIcon;
        }

        // レアリティボーダー色設定
        if (rarityBorder != null)
        {
            rarityBorder.color = GetRarityColor(relic.rarity);
        }

        // スタック数表示
        UpdateStackDisplay();
    }

    void UpdateStackDisplay()
    {
        if (currentRelic == null) return;

        bool shouldShowStack = currentRelic.isStackable && currentRelic.stackCount > 1;
        
        if (stackCountObject != null)
        {
            stackCountObject.SetActive(shouldShowStack);
        }

        if (stackCountText != null && shouldShowStack)
        {
            stackCountText.text = currentRelic.stackCount.ToString();
        }
    }

    Color GetRarityColor(RelicRarity rarity)
    {
        switch (rarity)
        {
            case RelicRarity.Common: return commonColor;
            case RelicRarity.Uncommon: return uncommonColor;
            case RelicRarity.Rare: return rareColor;
            case RelicRarity.Epic: return epicColor;
            case RelicRarity.Legendary: return legendaryColor;
            default: return commonColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentRelic != null && tooltipManager != null)
        {
            tooltipManager.ShowTooltip(currentRelic.GetDetailedDescription(), transform.position);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipManager != null)
        {
            tooltipManager.HideTooltip();
        }
    }

    public void RefreshIcon()
    {
        if (currentRelic != null)
        {
            UpdateStackDisplay();
            
            // エフェクト値が変わった場合の更新処理
            //if (tooltipManager != null && tooltipManager.IsTooltipActive())
            //{
            //    tooltipManager.UpdateTooltipContent(currentRelic.GetDetailedDescription());
            //}
        }
    }

    void OnDestroy()
    {
        if (tooltipManager != null)
        {
            tooltipManager.HideTooltip();
        }
    }
}