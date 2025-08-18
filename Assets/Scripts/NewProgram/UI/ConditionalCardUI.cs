using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ConditionalCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("UI Components")]
    public Image cardImage;
    public Text cardNameText;
    public Text manaCostText;
    public Text descriptionText;
    public Button cardButton;
    public Image cardBack;

    [Header("Condition Display")]
    public GameObject conditionPanel;
    public Text condition1Text;
    public Text condition2Text;
    public Image condition1Icon;
    public Image condition2Icon;
    public GameObject enhancedIndicator;

    [Header("Visual Feedback")]
    public Image cardBorder;
    public Color normalBorderColor = Color.white;
    public Color enhancedBorderColor = Color.gold;
    public Color unavailableBorderColor = Color.red;

    [Header("Hover Effect")]
    public float hoverScale = 1.1f;
    public float animationSpeed = 5f;

    private ConditionalSkillCard associatedCard;
    private Vector3 originalScale;
    private bool isHovered = false;

    public System.Action OnCardClicked;

    void Start()
    {
        originalScale = transform.localScale;

        if (cardButton != null)
        {
            cardButton.onClick.AddListener(() => OnCardClicked?.Invoke());
        }
    }

    void Update()
    {
        // ホバー効果のスムーズなアニメーション
        Vector3 targetScale = isHovered ? originalScale * hoverScale : originalScale;
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, animationSpeed * Time.deltaTime);
    }

    public void SetCard(ConditionalSkillCard card, bool showFront)
    {
        associatedCard = card;

        if (showFront)
        {
            ShowCardFront();
        }
        else
        {
            ShowCardBack();
        }
    }

    void ShowCardFront()
    {
        if (cardBack != null) cardBack.gameObject.SetActive(false);

        if (cardImage != null && associatedCard.cardImage != null)
        {
            cardImage.sprite = associatedCard.cardImage;
            cardImage.gameObject.SetActive(true);
        }

        if (cardNameText != null)
        {
            cardNameText.text = associatedCard.cardName;
            cardNameText.gameObject.SetActive(true);
        }

        if (manaCostText != null)
        {
            manaCostText.text = associatedCard.manaCost.ToString();
            manaCostText.gameObject.SetActive(true);
        }

        if (descriptionText != null)
        {
            descriptionText.text = associatedCard.GetTooltipText();
            descriptionText.gameObject.SetActive(true);
        }

        if (cardButton != null)
        {
            cardButton.interactable = true;
        }

        // 条件表示
        ShowConditions();
    }

    void ShowCardBack()
    {
        if (cardBack != null) cardBack.gameObject.SetActive(true);

        // フロント要素を非表示
        if (cardImage != null) cardImage.gameObject.SetActive(false);
        if (cardNameText != null) cardNameText.gameObject.SetActive(false);
        if (manaCostText != null) manaCostText.gameObject.SetActive(false);
        if (descriptionText != null) descriptionText.gameObject.SetActive(false);
        if (conditionPanel != null) conditionPanel.SetActive(false);

        if (cardButton != null)
        {
            cardButton.interactable = false;
        }
    }

    void ShowConditions()
    {
        if (conditionPanel != null)
        {
            conditionPanel.SetActive(true);
        }

        // 条件1の表示
        if (condition1Text != null && associatedCard.usageConditions.Length > 0 && associatedCard.usageConditions[0] != null)
        {
            condition1Text.text = associatedCard.usageConditions[0].GetConditionDescription();
            condition1Text.gameObject.SetActive(true);
        }
        else if (condition1Text != null)
        {
            condition1Text.gameObject.SetActive(false);
        }

        // 条件2の表示
        if (condition2Text != null && associatedCard.usageConditions.Length > 1 && associatedCard.usageConditions[1] != null)
        {
            condition2Text.text = associatedCard.usageConditions[1].GetConditionDescription();
            condition2Text.gameObject.SetActive(true);
        }
        else if (condition2Text != null)
        {
            condition2Text.gameObject.SetActive(false);
        }
    }

    public void UpdateConditionStatus(Character player, Character enemy)
    {
        if (associatedCard == null) return;

        bool canUse = associatedCard.CanUse(player, enemy);
        bool isEnhanced = associatedCard.IsEnhanced(player, enemy);

        // ボーダー色の変更
        if (cardBorder != null)
        {
            if (!canUse)
            {
                cardBorder.color = unavailableBorderColor;
            }
            else if (isEnhanced)
            {
                cardBorder.color = enhancedBorderColor;
            }
            else
            {
                cardBorder.color = normalBorderColor;
            }
        }

        // 強化インジケーター
        if (enhancedIndicator != null)
        {
            enhancedIndicator.SetActive(isEnhanced);
        }

        // 条件アイコンの色変更
        UpdateConditionIcon(condition1Icon, 0, player, enemy);
        UpdateConditionIcon(condition2Icon, 1, player, enemy);

        // ボタンの有効/無効
        if (cardButton != null)
        {
            cardButton.interactable = canUse;
        }
    }

    void UpdateConditionIcon(Image conditionIcon, int conditionIndex, Character player, Character enemy)
    {
        if (conditionIcon == null || associatedCard.usageConditions.Length <= conditionIndex || associatedCard.usageConditions[conditionIndex] == null)
        {
            if (conditionIcon != null) conditionIcon.gameObject.SetActive(false);
            return;
        }

        conditionIcon.gameObject.SetActive(true);
        bool conditionMet = associatedCard.usageConditions[conditionIndex].IsMet(player, enemy);
        conditionIcon.color = conditionMet ? Color.green : Color.red;
    }

    // イベントハンドラー
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnCardClicked?.Invoke();
    }
}