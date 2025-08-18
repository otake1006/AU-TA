using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputSystemCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("Card Data")]
    public ConditionalSkillCard associatedCard;

    [Header("UI Components")]
    public Image cardImage;
    public Text cardNameText;
    public Text manaCostText;
    public Text descriptionText;
    public Button cardButton;

    [Header("Visual States")]
    public GameObject normalState;
    public GameObject hoveredState;
    public GameObject selectedState;
    public GameObject disabledState;

    [Header("Input Feedback")]
    public AudioClip hoverSound;
    public AudioClip selectSound;
    public AudioClip disabledSound;

    private CardInteractionHandler interactionHandler;
    private bool isInteractable = true;
    private bool isHovered = false;
    private bool isSelected = false;

    void Start()
    {
        SetupCardUI();
        SetupInputHandler();
        SetupInputListeners();
    }

    void SetupCardUI()
    {
        if (cardButton != null)
        {
            cardButton.onClick.AddListener(OnCardClicked);
        }

        UpdateCardDisplay();
        UpdateVisualState();
    }

    void SetupInputHandler()
    {
        interactionHandler = GetComponent<CardInteractionHandler>();
        if (interactionHandler == null)
        {
            interactionHandler = gameObject.AddComponent<CardInteractionHandler>();
        }

        interactionHandler.SetCard(associatedCard);
        interactionHandler.OnCardClicked += OnCardInteractionClicked;
        interactionHandler.OnCardHovered += OnCardInteractionHovered;
        interactionHandler.OnCardUnhovered += OnCardInteractionUnhovered;
    }

    void SetupInputListeners()
    {
        // キーボードナビゲーション用
        InputManager.OnUINavigate += OnNavigateInput;
        InputManager.OnSubmit += OnSubmitInput;
        InputManager.OnCancel += OnCancelInput;
    }

    void UpdateCardDisplay()
    {
        if (associatedCard == null) return;

        if (cardImage != null && associatedCard.cardImage != null)
            cardImage.sprite = associatedCard.cardImage;

        if (cardNameText != null)
            cardNameText.text = associatedCard.cardName;

        if (manaCostText != null)
            manaCostText.text = associatedCard.manaCost.ToString();

        if (descriptionText != null)
            descriptionText.text = associatedCard.description;
    }

    void UpdateVisualState()
    {
        // 全状態を無効化
        if (normalState != null) normalState.SetActive(false);
        if (hoveredState != null) hoveredState.SetActive(false);
        if (selectedState != null) selectedState.SetActive(false);
        if (disabledState != null) disabledState.SetActive(false);

        // 現在の状態に応じて表示
        if (!isInteractable)
        {
            if (disabledState != null) disabledState.SetActive(true);
        }
        else if (isSelected)
        {
            if (selectedState != null) selectedState.SetActive(true);
        }
        else if (isHovered)
        {
            if (hoveredState != null) hoveredState.SetActive(true);
        }
        else
        {
            if (normalState != null) normalState.SetActive(true);
        }
    }

    // =============================================================================
    // Input Event Handlers
    // =============================================================================
    void OnNavigateInput(Vector2 navigate)
    {
        // キーボード/ゲームパッドナビゲーション
        if (EventSystem.current.currentSelectedGameObject == gameObject)
        {
            // 隣接するカードにフォーカス移動
            NavigateToAdjacentCard(navigate);
        }
    }

    void OnSubmitInput()
    {
        if (EventSystem.current.currentSelectedGameObject == gameObject && isInteractable)
        {
            OnCardClicked();
        }
    }

    void OnCancelInput()
    {
        if (isSelected)
        {
            DeselectCard();
        }
    }

    void NavigateToAdjacentCard(Vector2 direction)
    {
        // 方向に基づいて隣接するカードを探す
        var allCards = FindObjectsOfType<InputSystemCardUI>();
        var currentRect = GetComponent<RectTransform>();

        InputSystemCardUI closestCard = null;
        float closestDistance = float.MaxValue;

        foreach (var card in allCards)
        {
            if (card == this || !card.isInteractable) continue;

            var cardRect = card.GetComponent<RectTransform>();
            Vector2 directionToCard = (cardRect.position - currentRect.position).normalized;

            // 指定された方向との類似度をチェック
            float dot = Vector2.Dot(direction.normalized, directionToCard);
            if (dot > 0.5f) // 同じ方向内
            {
                float distance = Vector2.Distance(currentRect.position, cardRect.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestCard = card;
                }
            }
        }

        if (closestCard != null)
        {
            EventSystem.current.SetSelectedGameObject(closestCard.gameObject);
        }
    }

    // =============================================================================
    // UI Event System Handlers
    // =============================================================================
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isInteractable) return;

        isHovered = true;
        UpdateVisualState();

        // ホバーサウンド
        if (hoverSound != null)
            GameEvents.OnSFXPlay?.Invoke(hoverSound.name);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        UpdateVisualState();
    }

    public void OnSelect(BaseEventData eventData)
    {
        // キーボード/ゲームパッドでの選択
        isHovered = true;
        UpdateVisualState();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isHovered = false;
        UpdateVisualState();
    }

    // =============================================================================
    // Card Interaction Handlers
    // =============================================================================
    void OnCardClicked()
    {
        if (!isInteractable)
        {
            // 使用不可サウンド
            if (disabledSound != null)
                GameEvents.OnSFXPlay?.Invoke(disabledSound.name);
            return;
        }

        if (isSelected)
        {
            // 既に選択済みの場合は使用
            ExecuteCard();
        }
        else
        {
            // 選択
            SelectCard();
        }
    }

    void OnCardInteractionClicked(ConditionalSkillCard card)
    {
        OnCardClicked();
    }

    void OnCardInteractionHovered(ConditionalSkillCard card)
    {
        // ツールチップ表示
        if (TooltipManager.Instance != null)
        {
            TooltipManager.Instance.ShowTooltip(card.GetTooltipText(), transform.position);
        }
    }

    void OnCardInteractionUnhovered(ConditionalSkillCard card)
    {
        // ツールチップ非表示
        if (TooltipManager.Instance != null)
        {
            TooltipManager.Instance.HideTooltip();
        }
    }

    void SelectCard()
    {
        // 他のカードの選択解除
        var allCards = FindObjectsOfType<InputSystemCardUI>();
        foreach (var card in allCards)
        {
            if (card != this)
                card.DeselectCard();
        }

        isSelected = true;
        UpdateVisualState();

        // 選択サウンド
        if (selectSound != null)
            GameEvents.OnSFXPlay?.Invoke(selectSound.name);

        GameEvents.OnDebugMessage?.Invoke($"Card selected: {associatedCard?.cardName}");
    }

    void DeselectCard()
    {
        isSelected = false;
        UpdateVisualState();
    }

    void ExecuteCard()
    {
        if (associatedCard == null) return;

        var cardManager = FindObjectOfType<CardManager>();
        var battleManager = FindObjectOfType<BattleManager>();

        if (cardManager != null && battleManager != null)
        {
            bool success = cardManager.UseCard(associatedCard, battleManager.player, battleManager.enemy);
            if (success)
            {
                DeselectCard();
                GameEvents.OnDebugMessage?.Invoke($"Card executed: {associatedCard.cardName}");
            }
        }
    }

    // =============================================================================
    // Public Methods
    // =============================================================================
    public void SetCard(ConditionalSkillCard card)
    {
        associatedCard = card;
        if (interactionHandler != null)
            interactionHandler.SetCard(card);
        UpdateCardDisplay();
        UpdateInteractability();
    }

    public void UpdateInteractability()
    {
        if (associatedCard == null)
        {
            isInteractable = false;
        }
        else
        {
            var battleManager = FindObjectOfType<BattleManager>();
            if (battleManager != null)
            {
                isInteractable = associatedCard.CanUse(battleManager.player, battleManager.enemy);
            }
        }

        UpdateVisualState();

        if (cardButton != null)
            cardButton.interactable = isInteractable;
    }

    public void ForceDeselect()
    {
        isSelected = false;
        isHovered = false;
        UpdateVisualState();
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
        InputManager.OnUINavigate -= OnNavigateInput;
        InputManager.OnSubmit -= OnSubmitInput;
        InputManager.OnCancel -= OnCancelInput;

        if (interactionHandler != null)
        {
            interactionHandler.OnCardClicked -= OnCardInteractionClicked;
            interactionHandler.OnCardHovered -= OnCardInteractionHovered;
            interactionHandler.OnCardUnhovered -= OnCardInteractionUnhovered;
        }
    }
}