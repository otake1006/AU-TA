using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class CardInteractionHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Card Reference")]
    public ConditionalSkillCard associatedCard;

    [Header("Interaction Settings")]
    public float hoverScale = 1.1f;
    public float pressScale = 0.95f;
    public float animationSpeed = 8f;

    [Header("Visual Feedback")]
    public GameObject selectionIndicator;
    public GameObject hoverEffect;

    private Vector3 originalScale;
    private bool isHovered = false;
    private bool isPressed = false;
    private bool isSelected = false;

    // Events
    public System.Action<ConditionalSkillCard> OnCardClicked;
    public System.Action<ConditionalSkillCard> OnCardHovered;
    public System.Action<ConditionalSkillCard> OnCardUnhovered;

    void Start()
    {
        originalScale = transform.localScale;
        SetupInputListeners();
    }

    void SetupInputListeners()
    {
        // Input System イベントリスナー
        InputManager.OnCardSelect += OnCardSelectInput;
        InputManager.OnCancelSelection += OnCancelSelectionInput;
    }

    void Update()
    {
        UpdateScale();
        UpdateVisualFeedback();
    }

    void UpdateScale()
    {
        Vector3 targetScale = originalScale;

        if (isPressed)
            targetScale *= pressScale;
        else if (isHovered || isSelected)
            targetScale *= hoverScale;

        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, animationSpeed * Time.deltaTime);
    }

    void UpdateVisualFeedback()
    {
        if (selectionIndicator != null)
            selectionIndicator.SetActive(isSelected);

        if (hoverEffect != null)
            hoverEffect.SetActive(isHovered);
    }

    // =============================================================================
    // Input System Events
    // =============================================================================
    void OnCardSelectInput()
    {
        // マウスがこのカードの上にある場合のみ反応
        if (isHovered && CanUseCard())
        {
            SelectCard();
        }
    }

    void OnCancelSelectionInput()
    {
        if (isSelected)
        {
            DeselectCard();
        }
    }

    // =============================================================================
    // UI Event System (IPointer interfaces)
    // =============================================================================
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        OnCardHovered?.Invoke(associatedCard);

        // ツールチップ表示
        if (associatedCard != null)
        {
            ShowTooltip();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        OnCardUnhovered?.Invoke(associatedCard);
        HideTooltip();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && CanUseCard())
        {
            SelectCard();
        }
        else if (eventData.button == PointerEventData.InputButton.Right && isSelected)
        {
            DeselectCard();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isPressed = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }

    // =============================================================================
    // Card Interaction Logic
    // =============================================================================
    void SelectCard()
    {
        if (!CanUseCard()) return;

        isSelected = true;
        OnCardClicked?.Invoke(associatedCard);

        // 他のカードの選択を解除
        var otherCards = FindObjectsOfType<CardInteractionHandler>();
        foreach (var card in otherCards)
        {
            if (card != this && card.isSelected)
            {
                card.DeselectCard();
            }
        }

        // 音響フィードバック
        GameEvents.OnSFXPlay?.Invoke("CardSelect");

        Debug.Log($"Card selected: {associatedCard?.cardName}");
    }

    void DeselectCard()
    {
        isSelected = false;
        GameEvents.OnSFXPlay?.Invoke("CardDeselect");
        Debug.Log($"Card deselected: {associatedCard?.cardName}");
    }

    bool CanUseCard()
    {
        if (associatedCard == null) return false;

        var battleManager = FindObjectOfType<BattleManager>();
        if (battleManager == null) return false;

        // バトル状態チェック
        if (battleManager.CurrentState != BattleState.PlayerTurn) return false;
        if (battleManager.IsAnimationPlaying) return false;

        // カード使用可能チェック
        return associatedCard.CanUse(battleManager.player, battleManager.enemy);
    }

    void ShowTooltip()
    {
        if (associatedCard != null)
        {
            var tooltipManager = FindObjectOfType<TooltipManager>();
            tooltipManager?.ShowTooltip(associatedCard.GetTooltipText(), transform.position);
        }
    }

    void HideTooltip()
    {
        var tooltipManager = FindObjectOfType<TooltipManager>();
        tooltipManager?.HideTooltip();
    }

    // =============================================================================
    // Public Methods
    // =============================================================================
    public void SetCard(ConditionalSkillCard card)
    {
        associatedCard = card;
    }

    public void ForceDeselect()
    {
        isSelected = false;
        isHovered = false;
        isPressed = false;
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    // =============================================================================
    // Cleanup
    // =============================================================================
    void OnDestroy()
    {
        InputManager.OnCardSelect -= OnCardSelectInput;
        InputManager.OnCancelSelection -= OnCancelSelectionInput;
    }
}