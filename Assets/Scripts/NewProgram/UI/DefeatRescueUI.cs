using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DefeatRescueUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject rescuePanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Transform cardContainer;
    public GameObject relicCardUIPrefab;
    public Button confirmButton;
    public Button skipButton;

    [Header("Animation Settings")]
    public float fadeInDuration = 1f;
    public float cardAppearDelay = 0.5f;
    public AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private List<RelicCardUI> displayedCards = new List<RelicCardUI>();
    private RelicCard selectedCard;
    private System.Action<RelicCard> onRelicSelected;
    private System.Action onRescueSkipped;

    void Start()
    {
        InitializeUI();
    }

    void InitializeUI()
    {
        if (rescuePanel != null)
            rescuePanel.SetActive(false);

        if (titleText != null)
            titleText.text = "敗北の救済";

        if (descriptionText != null)
            descriptionText.text = "古の神々があなたに慈悲を示している...\n一つのレリックを選び、再び立ち上がれ！";

        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(ConfirmSelection);
            confirmButton.interactable = false;
        }

        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipRescue);
        }
    }

    public void ShowRescueOptions(List<RelicCard> rescueCards, System.Action<RelicCard> onSelected, System.Action onSkipped)
    {
        onRelicSelected = onSelected;
        onRescueSkipped = onSkipped;

        StartCoroutine(ShowRescueSequence(rescueCards));
    }

    IEnumerator ShowRescueSequence(List<RelicCard> rescueCards)
    {
        // パネル表示
        if (rescuePanel != null)
        {
            rescuePanel.SetActive(true);
            yield return StartCoroutine(FadeInPanel());
        }

        // カードを順次表示
        ClearPreviousCards();
        for (int i = 0; i < rescueCards.Count; i++)
        {
            CreateRelicCardUI(rescueCards[i]);
            yield return new WaitForSeconds(cardAppearDelay);
        }

        // SEを再生
        GameEvents.OnSFXPlay?.Invoke("RescueAppear");
    }

    IEnumerator FadeInPanel()
    {
        CanvasGroup canvasGroup = rescuePanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = rescuePanel.AddComponent<CanvasGroup>();

        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float alpha = fadeInCurve.Evaluate(elapsedTime / fadeInDuration);
            canvasGroup.alpha = alpha;
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    void CreateRelicCardUI(RelicCard relicCard)
    {
        if (relicCardUIPrefab == null || cardContainer == null) return;

        GameObject cardObject = Instantiate(relicCardUIPrefab, cardContainer);
        RelicCardUI cardUI = cardObject.GetComponent<RelicCardUI>();

        if (cardUI != null)
        {
            cardUI.SetupCard(relicCard);
            cardUI.OnCardClicked += OnCardSelected;
            cardUI.SetSelectable(true);
            displayedCards.Add(cardUI);

            // 出現アニメーション
            StartCoroutine(AnimateCardAppearance(cardUI));
        }
    }

    IEnumerator AnimateCardAppearance(RelicCardUI cardUI)
    {
        if (cardUI == null) yield break;

        Transform cardTransform = cardUI.transform;
        Vector3 originalScale = cardTransform.localScale;
        cardTransform.localScale = Vector3.zero;

        float elapsedTime = 0f;
        float animDuration = 0.3f;

        while (elapsedTime < animDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float scale = fadeInCurve.Evaluate(elapsedTime / animDuration);
            cardTransform.localScale = originalScale * scale;
            yield return null;
        }

        cardTransform.localScale = originalScale;
    }

    void OnCardSelected(RelicCardUI cardUI)
    {
        selectedCard = cardUI.GetRelicCard();

        // 他のカードの選択を解除
        foreach (var card in displayedCards)
        {
            card.SetSelected(card == cardUI);
        }

        // 確認ボタンを有効化
        if (confirmButton != null)
        {
            confirmButton.interactable = true;
        }

        GameEvents.OnSFXPlay?.Invoke("CardSelect");
    }

    void ConfirmSelection()
    {
        if (selectedCard != null)
        {
            GameEvents.OnSFXPlay?.Invoke("RescueConfirm");
            onRelicSelected?.Invoke(selectedCard);
            HideRescueUI();
        }
    }

    void SkipRescue()
    {
        GameEvents.OnSFXPlay?.Invoke("RescueSkip");
        onRescueSkipped?.Invoke();
        HideRescueUI();
    }

    void HideRescueUI()
    {
        if (rescuePanel != null)
        {
            rescuePanel.SetActive(false);
        }

        ClearPreviousCards();
        selectedCard = null;

        if (confirmButton != null)
        {
            confirmButton.interactable = false;
        }
    }

    void ClearPreviousCards()
    {
        foreach (var cardUI in displayedCards)
        {
            if (cardUI != null)
            {
                cardUI.OnCardClicked -= OnCardSelected;
                DestroyImmediate(cardUI.gameObject);
            }
        }
        displayedCards.Clear();
    }

    void OnDestroy()
    {
        if (confirmButton != null)
            confirmButton.onClick.RemoveAllListeners();

        if (skipButton != null)
            skipButton.onClick.RemoveAllListeners();

        ClearPreviousCards();
    }
}

// レリックカードUI用コンポーネント
public class RelicCardUI : MonoBehaviour
{
    [Header("UI References")]
    public Image cardImage;
    public Image relicIcon;
    public Image rarityBorder;
    public TextMeshProUGUI cardName;
    public TextMeshProUGUI description;
    public Button cardButton;
    public GameObject selectedIndicator;

    [Header("Visual Settings")]
    public Color selectedColor = Color.yellow;
    public Color normalColor = Color.white;

    private RelicCard relicCard;
    private bool isSelected = false;

    public System.Action<RelicCardUI> OnCardClicked;

    void Start()
    {
        if (cardButton != null)
        {
            cardButton.onClick.AddListener(OnClick);
        }

        if (selectedIndicator != null)
        {
            selectedIndicator.SetActive(false);
        }
    }

    public void SetupCard(RelicCard card)
    {
        relicCard = card;

        if (card == null) return;

        // カード名設定
        if (cardName != null)
        {
            cardName.text = card.cardName;
        }

        // 説明設定
        if (description != null)
        {
            //description.text = card.GetDescription();
        }

        // レリックアイコン設定
        if (relicIcon != null && card.relicEffect?.buffIcon != null)
        {
            relicIcon.sprite = card.relicEffect.buffIcon;
        }

        // レアリティボーダー設定
        if (rarityBorder != null && card.relicEffect != null)
        {
            rarityBorder.color = GetRarityColor(card.relicEffect.rarity);
        }
    }

    Color GetRarityColor(RelicRarity rarity)
    {
        switch (rarity)
        {
            case RelicRarity.Common: return Color.white;
            case RelicRarity.Uncommon: return Color.green;
            case RelicRarity.Rare: return Color.blue;
            case RelicRarity.Epic: return Color.magenta;
            case RelicRarity.Legendary: return Color.yellow;
            default: return Color.white;
        }
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;

        if (selectedIndicator != null)
        {
            selectedIndicator.SetActive(selected);
        }

        if (cardImage != null)
        {
            cardImage.color = selected ? selectedColor : normalColor;
        }
    }

    public void SetSelectable(bool selectable)
    {
        if (cardButton != null)
        {
            cardButton.interactable = selectable;
        }
    }

    void OnClick()
    {
        OnCardClicked?.Invoke(this);
    }

    public RelicCard GetRelicCard()
    {
        return relicCard;
    }

    void OnDestroy()
    {
        if (cardButton != null)
        {
            cardButton.onClick.RemoveAllListeners();
        }
    }
}