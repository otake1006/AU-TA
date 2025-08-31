using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.GPUSort;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Main UI References")]
    public Canvas mainCanvas;
    public GameObject gameOverPanel;
    public GameObject pausePanel;
    public GameObject settingsPanel;

    [Header("Battle UI")]
    public TurnIndicatorUI turnIndicator;
    public MatchStatusUI matchStatus;
    public GameObject playerHandArea;
    public GameObject enemyHandArea;
    public Transform playerBuffArea;
    public Transform enemyBuffArea;

    [Header("Character UI")]
    public HealthBarUI playerHealthBar;
    public HealthBarUI enemyHealthBar;
    public ManaBarUI playerManaBar;
    public ManaBarUI enemyManaBar;

    [Header("Notification")]
    public GameObject notificationPanel;
    public Text notificationText;
    public float notificationDuration = 3f;

    [Header("Card UI")]
    public GameObject cardUIPrefab;
    public GameObject cardAndDescriptionUIPrefab;
    public GameObject buffIconPrefab;

    [Header("Game Over UI")]
    public Text winnerText;
    public Text finalScoreText;
    public Button restartButton;
    public Button mainMenuButton;

    [Header("Debug UI")]
    public Transform DebugCardArea;
    public Transform DebugCardConditionArea;

    private GameConfig gameConfig;
    private List<GameObject> activeCardUIs = new List<GameObject>();
    private Coroutine notificationCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(GameConfig config)
    {
        gameConfig = config;
        SetupEventListeners();
        SetupUI();
    }

    void SetupEventListeners()
    {
        GameEvents.OnUIUpdate += UpdateAllUI;
        GameEvents.OnBattleStateChanged += OnBattleStateChanged;
        GameEvents.OnNotificationShow += ShowNotification;
        GameEvents.OnHandUpdated += UpdateHandUI;
        GameEvents.OnBuffListChanged += UpdateBuffUI;
        GameEvents.OnScoreChanged += UpdateMatchStatus;
        GameEvents.OnTurnChanged += UpdateTurnIndicator;
    }

    void SetupUI()
    {
        // 初期UI状態設定
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (notificationPanel != null) notificationPanel.SetActive(false);

        // ボタンイベント設定
        if (restartButton != null)
            restartButton.onClick.AddListener(() => GameManager.Instance?.RestartBattle());
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(() => GameManager.Instance?.ReturnToMainMenu());
        if (DebugCardArea != null)
            CreateDebugCards();
    }

    public void UpdateAllUI()
    {
        UpdateHealthBars();
        UpdateManaBars();
        //UpdateBuffUI();
    }

    void UpdateHealthBars()
    {
        var battleManager = FindObjectOfType<BattleManager>();
        if (battleManager == null) return;

        if (playerHealthBar != null)
            playerHealthBar.UpdateHealthBar(battleManager.player);
        if (enemyHealthBar != null)
            enemyHealthBar.UpdateHealthBar(battleManager.enemy);
    }

    void UpdateManaBars()
    {
        var battleManager = FindFirstObjectByType<BattleManager>();
        if (battleManager == null) return;

        if (playerManaBar != null)
            playerManaBar.UpdateManaBar(battleManager.player);
        if (enemyManaBar != null)
            enemyManaBar.UpdateManaBar(battleManager.enemy);
    }

    void UpdateHandUI(Character character)
    {
        var cardManager = FindFirstObjectByType<CardManager>();
        if (cardManager == null) return;

        Transform handArea = null;
        List<ConditionalSkillCard> hand = null;
        bool showFront = false;

        var battleManager = FindFirstObjectByType<BattleManager>();
        if (character == battleManager?.player)
        {
            handArea = playerHandArea?.transform;
            hand = cardManager.PlayerHand;
            showFront = true;
        }
        else if (character == battleManager?.enemy)
        {
            handArea = enemyHandArea?.transform;
            hand = cardManager.EnemyHand;
            showFront = false;
        }

        if (handArea != null && hand != null)
        {
            UpdateCardDisplay(handArea, hand, showFront);
        }
    }

    void UpdateCardDisplay(Transform handArea, List<ConditionalSkillCard> cards, bool showFront)
    {
        // 既存のカードUIを削除
        foreach (Transform child in handArea)
        {
            Destroy(child.gameObject);
        }

        // 新しいカードUIを作成
        foreach (var card in cards)
        {
            GameObject cardObj = CreateCardUI(card, showFront);
            cardObj.transform.SetParent(handArea, false);

            if (showFront)
            {
                var cardUI = cardObj.GetComponent<ConditionalCardUI>();
                if (cardUI != null)
                {
                    cardUI.OnCardClicked += () => UseCard(card);
                }
            }
        }
    }

    GameObject CreateCardUI(ConditionalSkillCard card, bool showFront)
    {
        if (cardUIPrefab == null)
        {
            // プレハブがない場合の簡易UI作成
            GameObject cardObj = new GameObject($"Card_{card.cardName}");
            var cardU = cardObj.AddComponent<ConditionalCardUI>();
            cardU.SetCard(card, showFront);
            return cardObj;
        }

        GameObject cardInstance = Instantiate(cardUIPrefab);
        var cardUI = cardInstance.GetComponent<ConditionalCardUI>();
        cardUI?.SetCard(card, showFront);

        return cardInstance;
    }

    GameObject CreateConditonCardUI(SkillCondition card)
    {
        if (cardAndDescriptionUIPrefab == null)
        {
            // プレハブがない場合の簡易UI作成
            GameObject cardObj = new GameObject($"Card_{card.cardName}");
            var cardU = cardObj.AddComponent<CardUI>();
            cardU.SetCard(card);
            return cardObj;
        }

        GameObject cardInstance = Instantiate(cardAndDescriptionUIPrefab);
        var cardUI = cardInstance.GetComponent<CardUI>();
        cardUI?.SetCard(card);

        return cardInstance;
    }

    void CreateDebugCards()
    {
        var cardManager = FindFirstObjectByType<CardManager>();
        if (cardManager != null)
        {
            // generate Skill Card
            foreach (var card in cardManager.skillCard)
            {
                GameObject cardObj = CreateCardUI(card, true);
                cardObj.transform.SetParent(DebugCardArea, false);

                var cardUI = cardObj.GetComponent<ConditionalCardUI>();
                if (cardUI != null)
                {
                    cardUI.OnCardClicked += () => UseCard(card);
                }
            }

            // generate Condition Card
            foreach (var card in cardManager.conditionCard)
            {
                GameObject cardObj = CreateConditonCardUI(card);
                cardObj.transform.SetParent(DebugCardConditionArea, false);

                //var cardUI = cardObj.GetComponent<ConditionalCardUI>();
                //if (cardUI != null)
                //{
                //    //cardUI.OnCardClicked += () => UseCard(card);
                //}
            }
        }
    }

    void UseCard(ConditionalSkillCard card)
    {
        var battleManager = FindFirstObjectByType<BattleManager>();
        var cardManager = FindFirstObjectByType<CardManager>();

        if (battleManager != null && cardManager != null)
        {
            bool success = cardManager.UseCard(card, battleManager.player, battleManager.enemy);
            if (success)
            {
                StartCoroutine(ExecuteCardEffect(card, battleManager.player, battleManager.enemy));
            }
        }
    }

    IEnumerator ExecuteCardEffect(ConditionalSkillCard card, Character caster, Character target)
    {
        // アニメーション設定
        yield return new WaitForSeconds(card.animationDuration * gameConfig.animationSpeedMultiplier);

        // エフェクト実行
        var effects = card.GetEffectsToUse(caster, target);
        foreach (var effect in effects)
        {
            ExecuteEffect(effect, caster, target);
            yield return new WaitForSeconds(0.3f);
        }
    }

    void ExecuteEffect(TurnBasedSkillEffect effect, Character caster, Character target)
    {
        var targetChar = effect.targetType == TargetType.Self ? caster : target;

        switch (effect.effectType)
        {
            case SkillEffectType.Damage:
                targetChar.TakeDamage(effect.value);
                break;
            case SkillEffectType.Heal:
                targetChar.Heal(effect.value);
                break;
            case SkillEffectType.Shield:
                targetChar.AddShield(effect.value);
                break;
        }
    }


    void UpdateBuffUI(Character character)
    {
        var buffManager = character.GetComponent<TurnBasedBuffManager>();
        if (buffManager == null) return;

        Transform buffArea = null;
        var battleManager = FindObjectOfType<BattleManager>();

        if (character == battleManager?.player)
            buffArea = playerBuffArea;
        else if (character == battleManager?.enemy)
            buffArea = enemyBuffArea;

        if (buffArea != null)
        {
            UpdateBuffIcons(buffArea, buffManager.GetActiveBuffs());
        }
    }

    void UpdateBuffIcons(Transform buffArea, List<TurnBasedBuffEffect> buffs)
    {
        // 既存のバフアイコンを削除
        foreach (Transform child in buffArea)
        {
            Destroy(child.gameObject);
        }

        // 新しいバフアイコンを作成
        foreach (var buff in buffs)
        {
            GameObject iconObj = CreateBuffIcon(buff);
            iconObj.transform.SetParent(buffArea, false);
        }
    }

    GameObject CreateBuffIcon(TurnBasedBuffEffect buff)
    {
        if (buffIconPrefab == null)
        {
            GameObject iconObj = new GameObject($"Buff_{buff.buffName}");
            var iconU = iconObj.AddComponent<TurnBasedBuffIconUI>();
            iconU.SetBuff(buff);
            return iconObj;
        }

        GameObject iconInstance = Instantiate(buffIconPrefab);
        var iconUI = iconInstance.GetComponent<TurnBasedBuffIconUI>();
        iconUI?.SetBuff(buff);

        return iconInstance;
    }

    void UpdateMatchStatus(int playerWins, int enemyWins)
    {
        if (matchStatus != null)
        {
            var roundManager = FindObjectOfType<RoundManager>();
            if (roundManager != null)
            {
                matchStatus.UpdateMatchStatus(playerWins, enemyWins, roundManager.CurrentRound, gameConfig.winsNeeded);
            }
        }
    }

    void UpdateTurnIndicator(int turnNumber)
    {
        if (turnIndicator != null)
        {
            var turnManager = FindObjectOfType<TurnManager>();
            if (turnManager != null)
            {
                turnIndicator.SetCurrentPlayer(turnManager.IsPlayerTurn);
                turnIndicator.SetTurnNumber(turnNumber);
            }
        }
    }

    void OnBattleStateChanged(BattleState newState)
    {
        switch (newState)
        {
            case BattleState.GameOver:
                // ゲームオーバー時は少し遅延してから表示
                StartCoroutine(ShowGameOverDelayed());
                break;
        }
    }

    IEnumerator ShowGameOverDelayed()
    {
        yield return new WaitForSeconds(2f);
        ShowGameOverScreen();
    }

    public void ShowGameOverScreen(Character winner = null)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            var roundManager = GetComponent<RoundManager>();
            if (roundManager != null)
            {
                var actualWinner = winner ?? roundManager.GetMatchWinner();

                if (winnerText != null)
                    winnerText.text = $"{actualWinner?.characterName ?? "Nobody"} Wins!";

                if (finalScoreText != null)
                    finalScoreText.text = $"Final Score: {roundManager.PlayerWins} - {roundManager.EnemyWins}";
            }
        }
    }

    public void ShowNotification(string message)
    {
        if (notificationCoroutine != null)
        {
            StopCoroutine(notificationCoroutine);
        }
        notificationCoroutine = StartCoroutine(ShowNotificationCoroutine(message));
    }

    IEnumerator ShowNotificationCoroutine(string message)
    {
        if (notificationPanel != null && notificationText != null)
        {
            notificationText.text = message;
            notificationPanel.SetActive(true);

            yield return new WaitForSeconds(notificationDuration);

            notificationPanel.SetActive(false);
        }
    }

    public void ShowPausePanel()
    {
        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    public void HidePausePanel()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    void OnDestroy()
    {
        // イベントリスナー解除
        GameEvents.OnUIUpdate -= UpdateAllUI;
        GameEvents.OnBattleStateChanged -= OnBattleStateChanged;
        GameEvents.OnNotificationShow -= ShowNotification;
        GameEvents.OnHandUpdated -= UpdateHandUI;
        GameEvents.OnBuffListChanged -= UpdateBuffUI;
        GameEvents.OnScoreChanged -= UpdateMatchStatus;
        GameEvents.OnTurnChanged -= UpdateTurnIndicator;
    }
}