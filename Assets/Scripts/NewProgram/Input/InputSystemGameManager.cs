using UnityEngine;

public class InputSystemGameManager : MonoBehaviour
{
    public static InputSystemGameManager Instance { get; private set; }

    [Header("Managers")]
    public InputManager inputManager;
    public BattleManager battleManager;
    public UIManager uiManager;
    public InputSystemDebugManager debugManager;

    [Header("Game Config")]
    public GameConfig gameConfig;

    public InputMode currentInputMode;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeManagers();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeManagers()
    {
        // InputManager初期化
        if (inputManager == null)
            inputManager = FindObjectOfType<InputManager>();

        SetupInputEventListeners();

        // ゲーム開始時はバトル入力モード
        SetInputMode(InputMode.Battle);
    }

    void SetupInputEventListeners()
    {
        // Debug入力イベント
        InputManager.OnToggleDebugUI += ToggleDebugUI;
        InputManager.OnForcePlayerWin += ForcePlayerWin;
        InputManager.OnForceEnemyWin += ForceEnemyWin;
        InputManager.OnForceDraw += ForceDraw;
        InputManager.OnSkipTurn += SkipTurn;
        InputManager.OnRestartBattle += RestartBattle;

        // UI入力イベント
        InputManager.OnPause += TogglePause;
        InputManager.OnCancel += OnCancelInput;
        InputManager.OnSubmit += OnSubmitInput;

        // Battle入力イベント
        InputManager.OnConfirmAction += OnConfirmAction;
        InputManager.OnCancelSelection += OnCancelSelection;
    }

    public void SetInputMode(InputMode mode)
    {
        if (currentInputMode == mode) return;

        currentInputMode = mode;
        inputManager?.SetInputMode(mode);

        Debug.Log($"Input mode changed to: {mode}");
    }

    // =============================================================================
    // Debug Input Handlers
    // =============================================================================
    void ToggleDebugUI()
    {
        debugManager?.ToggleDebugUI();
    }

    void ForcePlayerWin()
    {
        if (gameConfig?.enableDebugMode == true)
        {
            battleManager?.DebugForceRoundEnd(RoundResult.PlayerWin);
        }
    }

    void ForceEnemyWin()
    {
        if (gameConfig?.enableDebugMode == true)
        {
            battleManager?.DebugForceRoundEnd(RoundResult.EnemyWin);
        }
    }

    void ForceDraw()
    {
        if (gameConfig?.enableDebugMode == true)
        {
            battleManager?.DebugForceRoundEnd(RoundResult.Draw);
        }
    }

    void SkipTurn()
    {
        if (gameConfig?.enableDebugMode == true)
        {
            battleManager?.DebugSkipTurn();
        }
    }

    void RestartBattle()
    {
        if (gameConfig?.enableDebugMode == true)
        {
            battleManager?.RestartBattle();
        }
    }

    // =============================================================================
    // UI Input Handlers
    // =============================================================================
    void TogglePause()
    {
        if (currentInputMode == InputMode.Battle)
        {
            SetInputMode(InputMode.Paused);
            Time.timeScale = 0f;
            uiManager?.ShowPausePanel();
        }
        else if (currentInputMode == InputMode.Paused)
        {
            SetInputMode(InputMode.Battle);
            Time.timeScale = 1f;
            uiManager?.HidePausePanel();
        }
    }

    void OnCancelInput()
    {
        switch (currentInputMode)
        {
            case InputMode.Paused:
                TogglePause(); // ポーズ解除
                break;
            case InputMode.UI:
                // UIキャンセル処理
                break;
        }
    }

    void OnSubmitInput()
    {
        switch (currentInputMode)
        {
            case InputMode.UI:
                // UI確定処理
                break;
        }
    }

    // =============================================================================
    // Battle Input Handlers
    // =============================================================================
    void OnConfirmAction()
    {
        if (currentInputMode == InputMode.Battle)
        {
            // 選択されたカードの確定処理
            var selectedCard = GetSelectedCard();
            if (selectedCard != null)
            {
                ExecuteSelectedCard(selectedCard);
            }
        }
    }

    void OnCancelSelection()
    {
        if (currentInputMode == InputMode.Battle)
        {
            // カード選択キャンセル
            ClearCardSelection();
        }
    }

    ConditionalSkillCard GetSelectedCard()
    {
        var cardHandlers = FindObjectsOfType<CardInteractionHandler>();
        foreach (var handler in cardHandlers)
        {
            if (handler.IsSelected())
            {
                return handler.associatedCard;
            }
        }
        return null;
    }

    void ExecuteSelectedCard(ConditionalSkillCard card)
    {
        var cardManager = FindObjectOfType<CardManager>();
        if (cardManager != null && battleManager != null)
        {
            bool success = cardManager.UseCard(card, battleManager.player, battleManager.enemy);
            if (success)
            {
                ClearCardSelection();
            }
        }
    }

    void ClearCardSelection()
    {
        var cardHandlers = FindObjectsOfType<CardInteractionHandler>();
        foreach (var handler in cardHandlers)
        {
            handler.ForceDeselect();
        }
    }

    // =============================================================================
    // Game State Management
    // =============================================================================
    void OnBattleStateChanged(BattleState newState)
    {
        switch (newState)
        {
            case BattleState.PlayerTurn:
                SetInputMode(InputMode.Battle);
                break;
            case BattleState.EnemyTurn:
            case BattleState.SkillExecution:
            case BattleState.BuffProcessing:
                // プレイヤー入力を無効化（内部的に処理）
                break;
            case BattleState.GameOver:
                SetInputMode(InputMode.GameOver);
                break;
        }
    }

    void OnDestroy()
    {
        // イベントリスナー解除
        InputManager.OnToggleDebugUI -= ToggleDebugUI;
        InputManager.OnForcePlayerWin -= ForcePlayerWin;
        InputManager.OnForceEnemyWin -= ForceEnemyWin;
        InputManager.OnForceDraw -= ForceDraw;
        InputManager.OnSkipTurn -= SkipTurn;
        InputManager.OnRestartBattle -= RestartBattle;
        InputManager.OnPause -= TogglePause;
        InputManager.OnCancel -= OnCancelInput;
        InputManager.OnSubmit -= OnSubmitInput;
        InputManager.OnConfirmAction -= OnConfirmAction;
        InputManager.OnCancelSelection -= OnCancelSelection;
    }
}