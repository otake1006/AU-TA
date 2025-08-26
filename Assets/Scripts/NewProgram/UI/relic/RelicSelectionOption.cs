using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RelicSelectionOption : MonoBehaviour
{
    [Header("UI Components")]
    public Image relicIcon;
    public TextMeshProUGUI relicNameText;
    public TextMeshProUGUI relicDescriptionText;
    public Button selectButton;

    private RelicEffect relic;
    private System.Action<RelicEffect> onSelectCallback;

    public void SetupOption(RelicEffect relicEffect, System.Action<RelicEffect> onSelect)
    {
        relic = relicEffect;
        onSelectCallback = onSelect;

        UpdateDisplay();
        SetupButton();
    }

    void UpdateDisplay()
    {
        if (relic == null) return;

        if (relicIcon != null && relic.relicIcon != null)
        {
            relicIcon.sprite = relic.relicIcon;
        }

        if (relicNameText != null)
        {
            relicNameText.text = relic.relicName;
        }

        if (relicDescriptionText != null)
        {
            relicDescriptionText.text = relic.description;
        }
    }

    void SetupButton()
    {
        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => onSelectCallback?.Invoke(relic));
        }
    }
}