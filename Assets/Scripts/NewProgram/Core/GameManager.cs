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
    public RelicManager relicManager;
    public DefeatRescueSystem rescueSystem;

    [Header("Scene References")]
    public Character player;
    public Character enemy;

    // �Q�[�����
    public GameState CurrentGameState { get; private set; }
    public bool IsPaused { get; private set; }

    // �C�x���g
    public System.Action<GameState> OnGameStateChanged;

    void Awake()
    {
        // �V���O���g���ݒ�
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
        // �}�l�[�W���[�̏������������d�v
        if (audioManager != null)
            audioManager.Initialize(gameConfig);

        if (effectManager != null)
            effectManager.Initialize(gameConfig);

        if (uiManager != null)
            uiManager.Initialize(gameConfig);

        if (battleManager != null)
            battleManager.Initialize(gameConfig, player, enemy);

        // レリックマネージャーの初期化
        if (relicManager != null && gameConfig.enableRelicSystem)
            relicManager.gameConfig = gameConfig;

        // 救済システムの初期化
        if (rescueSystem != null && gameConfig.enableRelicSystem && gameConfig.enableDefeatRescue)
        {
            rescueSystem.gameConfig = gameConfig;
        }
    }

    void SetupEventListeners()
    {
        GameEvents.OnMatchEnd += OnMatchEnd;
        GameEvents.OnLogMessage += OnLogMessage;

        // 救済システムのイベント
        if (rescueSystem != null)
        {
            rescueSystem.OnRescueCompleted += OnPlayerRescued;
            rescueSystem.OnRescueSkipped += OnRescueSkipped;
        }

        // �f�o�b�O���[�h�̐ݒ�
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

        // ��Ԃɉ���������
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

    void OnPlayerRescued(Character rescuedPlayer)
    {
        GameEvents.OnDebugMessage?.Invoke($"{rescuedPlayer.characterName} has been rescued!");
        
        // 戦闘を再開
        if (battleManager != null)
        {
            battleManager.RestartBattle();
        }
        
        // UIを更新
        if (uiManager != null)
        {
            uiManager.ShowNotification("神の恵みにより復活しました！");
        }
    }

    void OnRescueSkipped(Character player)
    {
        GameEvents.OnDebugMessage?.Invoke($"{player.characterName} skipped rescue opportunity");
        
        // 通常のゲームオーバー処理を続行
        ChangeGameState(GameState.GameOver);
    }

    void OnDestroy()
    {
        // �C�x���g���X�i�[����
        GameEvents.OnMatchEnd -= OnMatchEnd;
        GameEvents.OnLogMessage -= OnLogMessage;

        // 救済システムのイベント削除
        if (rescueSystem != null)
        {
            rescueSystem.OnRescueCompleted -= OnPlayerRescued;
            rescueSystem.OnRescueSkipped -= OnRescueSkipped;
        }
    }

    //void Update()
    //{
    //    // ESC�L�[�Ń|�[�Y
    //    if (Input.GetKeyDown(KeyCode.Escape))
    //    {
    //        if (CurrentGameState == GameState.Battle)
    //            PauseGame();
    //        else if (CurrentGameState == GameState.Paused)
    //            ResumeGame();
    //    }
    //}
}

// �Q�[�����
public enum GameState
{
    MainMenu,          // ���C�����j���[
    Battle,            // �o�g����
    Paused,            // �|�[�Y��
    GameOver,          // �Q�[���I��
    Settings,          // �ݒ���
    Loading            // ���[�h��
}