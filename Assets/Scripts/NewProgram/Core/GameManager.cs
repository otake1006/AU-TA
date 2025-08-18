using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Configuration")]
    public GameConfig gameConfig;

    [Header("Managers")]
    public BattleManager battleManager;
    public UIManager uiManager;
    public AudioManager audioManager;
    public EffectManager effectManager;

    [Header("Scene References")]
    public Character player;
    public Character enemy;

    // ゲーム状態
    public GameState CurrentGameState { get; private set; }
    public bool IsPaused { get; private set; }

    // イベント
    public System.Action<GameState> OnGameStateChanged;

    void Awake()
    {
        // シングルトン設定
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeGame()
    {
        LoadGameConfig();
        SetupManagers();
        SetupEventListeners();

        ChangeGameState(GameState.MainMenu);
    }

    void LoadGameConfig()
    {
        if (gameConfig == null)
        {
            gameConfig = Resources.Load<GameConfig>(GameConstants.CONFIG_PATH);
            if (gameConfig == null)
            {
                Debug.LogError("GameConfig not found! Creating default config.");
                gameConfig = ScriptableObject.CreateInstance<GameConfig>();
            }
        }
    }

    void SetupManagers()
    {
        // マネージャーの初期化順序が重要
        if (audioManager != null)
            audioManager.Initialize(gameConfig);

        if (effectManager != null)
            effectManager.Initialize(gameConfig);

        if (uiManager != null)
            uiManager.Initialize(gameConfig);

        if (battleManager != null)
            battleManager.Initialize(gameConfig, player, enemy);
    }

    void SetupEventListeners()
    {
        GameEvents.OnMatchEnd += OnMatchEnd;
        GameEvents.OnLogMessage += OnLogMessage;

        // デバッグモードの設定
        if (gameConfig.enableDebugMode)
        {
            var debugManager = FindObjectOfType<InputSystemDebugManager>();
            if (debugManager == null)
            {
                var debugObj = new GameObject("InputSystemDebugManager");
                debugObj.AddComponent<InputSystemDebugManager>();
            }
        }
    }

    public void ChangeGameState(GameState newState)
    {
        if (CurrentGameState == newState) return;

        CurrentGameState = newState;
        OnGameStateChanged?.Invoke(newState);

        // 状態に応じた処理
        switch (newState)
        {
            case GameState.MainMenu:
                HandleMainMenuState();
                break;
            case GameState.Battle:
                HandleBattleState();
                break;
            case GameState.Paused:
                HandlePausedState();
                break;
            case GameState.GameOver:
                HandleGameOverState();
                break;
        }

        GameEvents.OnDebugMessage?.Invoke($"Game State: {newState}");
    }

    void HandleMainMenuState()
    {
        Time.timeScale = 1f;
        if (audioManager != null)
            audioManager.PlayBGM("MainMenu");
    }

    void HandleBattleState()
    {
        Time.timeScale = 1f;
        if (audioManager != null)
            audioManager.PlayBGM("Battle");

        if (battleManager != null)
            battleManager.StartBattle();
    }

    void HandlePausedState()
    {
        Time.timeScale = 0f;
        IsPaused = true;
    }

    void HandleGameOverState()
    {
        if (audioManager != null)
            audioManager.PlayBGM("GameOver");
    }

    public void StartBattle()
    {
        ChangeGameState(GameState.Battle);
    }

    public void PauseGame()
    {
        if (CurrentGameState == GameState.Battle)
        {
            ChangeGameState(GameState.Paused);
        }
    }

    public void ResumeGame()
    {
        if (CurrentGameState == GameState.Paused)
        {
            IsPaused = false;
            Time.timeScale = 1f;
            ChangeGameState(GameState.Battle);
        }
    }

    public void RestartBattle()
    {
        if (battleManager != null)
        {
            battleManager.RestartBattle();
        }
        ChangeGameState(GameState.Battle);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        ChangeGameState(GameState.MainMenu);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    void OnMatchEnd(Character winner)
    {
        StartCoroutine(HandleMatchEndSequence(winner));
    }

    IEnumerator HandleMatchEndSequence(Character winner)
    {
        yield return new WaitForSeconds(2f);
        ChangeGameState(GameState.GameOver);

        if (uiManager != null)
        {
            uiManager.ShowGameOverScreen(winner);
        }
    }

    void OnLogMessage(string message, LogLevel level)
    {
        switch (level)
        {
            case LogLevel.Error:
                Debug.LogError(message);
                break;
            case LogLevel.Warning:
                Debug.LogWarning(message);
                break;
            case LogLevel.Debug:
                if (gameConfig.enableVerboseLogging)
                    Debug.Log($"[DEBUG] {message}");
                break;
            default:
                Debug.Log(message);
                break;
        }
    }

    void OnDestroy()
    {
        // イベントリスナー解除
        GameEvents.OnMatchEnd -= OnMatchEnd;
        GameEvents.OnLogMessage -= OnLogMessage;
    }

    //void Update()
    //{
    //    // ESCキーでポーズ
    //    if (Input.GetKeyDown(KeyCode.Escape))
    //    {
    //        if (CurrentGameState == GameState.Battle)
    //            PauseGame();
    //        else if (CurrentGameState == GameState.Paused)
    //            ResumeGame();
    //    }
    //}
}

// ゲーム状態
public enum GameState
{
    MainMenu,          // メインメニュー
    Battle,            // バトル中
    Paused,            // ポーズ中
    GameOver,          // ゲーム終了
    Settings,          // 設定画面
    Loading            // ロード中
}