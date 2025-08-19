using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InputSystemDebugManager : MonoBehaviour
{
    public static InputSystemDebugManager Instance { get; private set; }

    [Header("Debug UI")]
    public GameObject debugPanel;
    public Text debugLogText;
    public Text inputStatusText;
    public ScrollRect logScrollRect;

    [Header("Debug Buttons")]
    public Button playerWinButton;
    public Button enemyWinButton;
    public Button drawButton;
    public Button skipTurnButton;
    public Button restartButton;
    public Button clearLogButton;

    [Header("Input Debug")]
    public Toggle showInputStatus;
    public Text mousePositionText;
    public Text inputModeText;
    public Text activeActionsText;

    [Header("Settings")]
    public int maxLogLines = 100;
    public bool autoScroll = true;

    private List<string> debugLogs = new List<string>();
    private InputManager inputManager;
    private BattleManager battleManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        inputManager = FindObjectOfType<InputManager>();
        battleManager = FindObjectOfType<BattleManager>();
        SetupDebugUI();
        SetupInputListeners();

        if (debugPanel != null)
            debugPanel.SetActive(false);
    }

    void SetupDebugUI()
    {
        if (playerWinButton != null)
            playerWinButton.onClick.AddListener(() => ForceRoundEnd(RoundResult.PlayerWin));
        if (enemyWinButton != null)
            enemyWinButton.onClick.AddListener(() => ForceRoundEnd(RoundResult.EnemyWin));
        if (drawButton != null)
            drawButton.onClick.AddListener(() => ForceRoundEnd(RoundResult.Draw));
        if (skipTurnButton != null)
            skipTurnButton.onClick.AddListener(SkipTurn);
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartBattle);
        if (clearLogButton != null)
            clearLogButton.onClick.AddListener(ClearLog);
    }

    void SetupInputListeners()
    {
        // Input System イベント
        InputManager.OnToggleDebugUI += ToggleDebugUI;
        // ゲームイベント
        GameEvents.OnDebugMessage += AddDebugLog;

        // Input状態監視
        if (inputManager != null)
        {
            InputManager.OnMouseMove += OnMouseMove;
            InputManager.OnCardSelect += () => AddDebugLog("INPUT: Card Select");
            InputManager.OnCancelSelection += () => AddDebugLog("INPUT: Cancel Selection");
            InputManager.OnConfirmAction += () => AddDebugLog("INPUT: Confirm Action");
            InputManager.OnScroll += (delta) => AddDebugLog($"INPUT: Scroll {delta}");
        }
    }

    void Update()
    {
        if (showInputStatus != null && showInputStatus.isOn)
        {
            UpdateInputStatus();
        }
    }

    void UpdateInputStatus()
    {
        if (inputManager == null) return;

        // マウス位置表示
        if (mousePositionText != null)
        {
            mousePositionText.text = $"Mouse: {inputManager.MousePosition}";
        }

        // 入力モード表示
        if (inputModeText != null)
        {
            var gameManager = FindObjectOfType<InputSystemGameManager>();
            inputModeText.text = $"Input Mode: {gameManager?.currentInputMode ?? InputMode.Battle}";
        }

        // アクティブなアクション表示
        if (activeActionsText != null)
        {
            var activeActions = GetActiveInputActions();
            activeActionsText.text = $"Active Actions: {string.Join(", ", activeActions)}";
        }
    }

    List<string> GetActiveInputActions()
    {
        var activeActions = new List<string>();

        if (inputManager == null) return activeActions;

        // よく使われるアクションの状態をチェック
        if (inputManager.IsActionPressed("SelectCard"))
            activeActions.Add("SelectCard");
        if (inputManager.IsActionPressed("Navigate"))
            activeActions.Add("Navigate");
        if (inputManager.IsActionPressed("ConfirmAction"))
            activeActions.Add("ConfirmAction");

        return activeActions;
    }

    void OnMouseMove(Vector2 mousePosition)
    {
        if (showInputStatus != null && showInputStatus.isOn && mousePositionText != null)
        {
            var worldPos = inputManager.GetWorldMousePosition();
            mousePositionText.text = $"Mouse: {mousePosition} | World: {worldPos}";
        }
    }

    public void ToggleDebugUI()
    {
        if (debugPanel != null)
        {
            bool newState = !debugPanel.activeSelf;
            debugPanel.SetActive(newState);

            //AddDebugLog($"Debug UI: {(newState ? "Shown" : "Hidden")}");

            // 入力モード変更
            var gameManager = FindObjectOfType<InputSystemGameManager>();
            if (gameManager != null)
            {
                if (newState)
                    gameManager.SetInputMode(InputMode.UI);
                else
                    gameManager.SetInputMode(InputMode.Battle);
            }
        }
    }

    public void AddDebugLog(string message)
    {
        string timestamp = System.DateTime.Now.ToString("HH:mm:ss.fff");
        string logEntry = $"[{timestamp}] {message}";

        debugLogs.Add(logEntry);

        // ログ行数制限
        if (debugLogs.Count > maxLogLines)
        {
            debugLogs.RemoveAt(0);
        }

        UpdateDebugLogDisplay();
        Debug.Log(logEntry);
    }

    void UpdateDebugLogDisplay()
    {
        if (debugLogText != null)
        {
            debugLogText.text = string.Join("\n", debugLogs);

            // 自動スクロール
            if (autoScroll && logScrollRect != null)
            {
                Canvas.ForceUpdateCanvases();
                logScrollRect.verticalNormalizedPosition = 0f;
            }
        }
    }

    void ForceRoundEnd(RoundResult result)
    {
        if (battleManager != null)
        {
            battleManager.DebugForceRoundEnd(result);
            AddDebugLog($"DEBUG: Forced round end with result: {result}");
        }
    }

    void SkipTurn()
    {
        if (battleManager != null)
        {
            battleManager.DebugSkipTurn();
            AddDebugLog("DEBUG: Turn skipped");
        }
    }

    void RestartBattle()
    {
        if (battleManager != null)
        {
            battleManager.RestartBattle();
            AddDebugLog("DEBUG: Battle restarted");
        }
    }

    void ClearLog()
    {
        debugLogs.Clear();
        UpdateDebugLogDisplay();
        AddDebugLog("DEBUG: Log cleared");
    }

    void OnDestroy()
    {
        // イベントリスナー解除
        InputManager.OnToggleDebugUI -= ToggleDebugUI;
        InputManager.OnMouseMove -= OnMouseMove;
        GameEvents.OnDebugMessage -= AddDebugLog;
    }
}