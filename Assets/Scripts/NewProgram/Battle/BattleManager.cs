using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BattleManager : MonoBehaviour
{
    [Header("Configuration")]
    public GameConfig gameConfig;

    [Header("Managers")]
    public RoundManager roundManager;
    public TurnManager turnManager;
    public CardManager cardManager;

    [Header("Characters")]
    public Character player;
    public Character enemy;

    // プロパティ
    public BattleState CurrentState { get; private set; }
    public bool IsMatchOver { get; private set; }
    public bool IsAnimationPlaying { get; private set; }

    // コルーチン管理
    private Coroutine battleSequenceCoroutine;
    private bool isBattleInitialized = false;
    private bool isBattleStarting = false;

    // イベント
    public System.Action<BattleState> OnStateChanged;

    void Start()
    {
        if (gameConfig != null)
        {
            Initialize(gameConfig, player, enemy);
        }
    }

    public void Initialize(GameConfig config, Character playerChar, Character enemyChar)
    {
        // 既に初期化済みの場合は無視
        if (isBattleInitialized)
        {
            GameEvents.OnDebugMessage?.Invoke("BattleManager is already initialized");
            return;
        }

        gameConfig = config;
        player = playerChar;
        enemy = enemyChar;

        SetupManagers();
        SetupEventListeners();

        isBattleInitialized = true;
        GameEvents.OnDebugMessage?.Invoke("BattleManager initialized successfully");
    }

    void SetupManagers()
    {
        // マネージャーの取得/作成
        if (roundManager == null)
            roundManager = GetComponent<RoundManager>() ?? gameObject.AddComponent<RoundManager>();
        if (turnManager == null)
            turnManager = GetComponent<TurnManager>() ?? gameObject.AddComponent<TurnManager>();
        if (cardManager == null)
            cardManager = GetComponent<CardManager>() ?? gameObject.AddComponent<CardManager>();

        // マネージャーの初期化
        roundManager.Initialize(this);
        turnManager.Initialize(this);
        cardManager.Initialize(this);
    }

    void SetupEventListeners()
    {
        // 重複登録を防ぐために一度解除
        RemoveEventListeners();

        GameEvents.OnCharacterDeath += OnCharacterDeath;
        GameEvents.OnSimultaneousDefeat += OnSimultaneousDefeat;
        GameEvents.OnRoundEnd += OnRoundEnd;
        GameEvents.OnCardUsed += OnCardUsed;
    }

    void RemoveEventListeners()
    {
        GameEvents.OnCharacterDeath -= OnCharacterDeath;
        GameEvents.OnSimultaneousDefeat -= OnSimultaneousDefeat;
        GameEvents.OnRoundEnd -= OnRoundEnd;
        GameEvents.OnCardUsed -= OnCardUsed;
    }

    public void StartBattle()
    {
        // 既にバトルが開始中または終了している場合は無視
        if (isBattleStarting)
        {
            GameEvents.OnDebugMessage?.Invoke("Battle is already starting");
            return;
        }

        if (IsMatchOver)
        {
            GameEvents.OnDebugMessage?.Invoke("Match is over, use RestartBattle instead");
            return;
        }

        if (!isBattleInitialized)
        {
            GameEvents.OnDebugMessage?.Invoke("BattleManager not initialized");
            return;
        }

        // 既存のバトルシーケンスがある場合は停止
        StopBattleSequence();

        isBattleStarting = true;
        battleSequenceCoroutine = StartCoroutine(StartBattleSequenceWithSafety());
    }

    private void StopBattleSequence()
    {
        if (battleSequenceCoroutine != null)
        {
            StopCoroutine(battleSequenceCoroutine);
            battleSequenceCoroutine = null;
            GameEvents.OnDebugMessage?.Invoke("Previous battle sequence stopped");
        }
    }

    IEnumerator StartBattleSequenceWithSafety()
    {
        ChangeState(BattleState.Initializing);

        // 初期化処理
        yield return new WaitForSeconds(1f);

        // 途中でキャンセルされた場合のチェック
        if (battleSequenceCoroutine == null)
        {
            GameEvents.OnDebugMessage?.Invoke("Battle sequence was cancelled during initialization");
            yield break;
        }

        // バトル開始
        GameEvents.OnDebugMessage?.Invoke("バトルスタート!");
        ChangeState(BattleState.PlayerTurn);

        // コルーチン参照をクリア
        battleSequenceCoroutine = null;
        isBattleStarting = false;

        // ラウンド開始
        roundManager.StartNewRound();
    }

    public void ChangeState(BattleState newState)
    {
        if (CurrentState == newState) return;

        BattleState previousState = CurrentState;
        CurrentState = newState;

        OnStateChanged?.Invoke(newState);
        GameEvents.OnBattleStateChanged?.Invoke(newState);

        if (gameConfig.enableDebugMode)
        {
            //GameEvents.OnDebugMessage?.Invoke($"バトルステータス: {previousState} → {newState}");
        }
    }

    public void SetAnimationPlaying(bool playing)
    {
        IsAnimationPlaying = playing;
    }

    void OnCharacterDeath(Character character)
    {
        if (IsMatchOver) return;

        // 同時撃破チェックは SimultaneousDefeat で処理される
        GameEvents.OnDebugMessage?.Invoke($"{character.characterName} 敗北!");
    }

    void OnSimultaneousDefeat(Character player, Character enemy)
    {
        if (IsMatchOver) return; // 重複防止

        GameEvents.OnDebugMessage?.Invoke("Simultaneous defeat occurred!");

        RoundResult result = SimultaneousDefeat.DetermineResult(
            gameConfig.simultaneousDefeatRule,
            player,
            enemy,
            turnManager.FirstToAct
        );

        // 安全にラウンド終了
        if (roundManager != null)
        {
            roundManager.EndRound(result);
        }
    }

    void OnRoundEnd(RoundResult result, int round)
    {
        if (IsMatchOver) return; // 重複防止

        if (roundManager.IsMatchComplete())
        {
            EndMatch();
        }
    }

    void OnCardUsed(ConditionalSkillCard card, Character caster, Character target)
    {
        if (IsMatchOver) return;

        GameEvents.OnDebugMessage?.Invoke($"{caster.characterName} 使用 {card.cardName}");
    }

    void EndMatch()
    {
        if (IsMatchOver) return; // 重複防止

        IsMatchOver = true;

        // 進行中のシーケンスを停止
        StopBattleSequence();

        // 各マネージャーの処理も停止
        if (roundManager != null)
            roundManager.StopCurrentRoundProcess();
        if (turnManager != null)
            turnManager.ResetTurn();

        ChangeState(BattleState.GameOver);

        Character winner = roundManager.GetMatchWinner();
        GameEvents.OnMatchEnd?.Invoke(winner);

        GameEvents.OnDebugMessage?.Invoke($"試合終了! 勝者: {winner?.characterName}");
    }

    public void RestartBattle()
    {
        // 現在の処理を完全に停止
        StopBattleSequence();

        // 状態をリセット
        IsMatchOver = false;
        isBattleStarting = false;

        if (roundManager != null)
        {
            roundManager.ResetMatch();
        }

        if (turnManager != null)
        {
            turnManager.ResetTurn();
        }

        if (cardManager != null)
        {
            cardManager.ResetAllCards();
        }

        // 短い待機後に開始（安全性のため）
        StartCoroutine(RestartBattleWithDelay());
    }

    IEnumerator RestartBattleWithDelay()
    {
        yield return new WaitForSeconds(0.5f);
        StartBattle();
    }

    // 強制停止メソッド
    public void ForceStopBattle()
    {
        StopBattleSequence();

        if (roundManager != null)
            roundManager.StopCurrentRoundProcess();

        isBattleStarting = false;
        IsMatchOver = true;

        ChangeState(BattleState.GameOver);
        GameEvents.OnDebugMessage?.Invoke("Battle force stopped");
    }

    // デバッグ用メソッド
    public void DebugForceRoundEnd(RoundResult result)
    {
        if (gameConfig.enableDebugMode && !IsMatchOver)
        {
            roundManager.EndRound(result);
        }
    }

    public void DebugSkipTurn()
    {
        if (gameConfig.enableDebugMode && !IsAnimationPlaying && !IsMatchOver)
        {
            turnManager.SkipCurrentTurn();
        }
    }

    public void DebugCurrentState()
    {
        if (gameConfig.enableDebugMode)
        {
            Debug.Log($"=== BattleManager Debug State ===");
            Debug.Log($"Current State: {CurrentState}");
            Debug.Log($"Is Match Over: {IsMatchOver}");
            Debug.Log($"Is Battle Starting: {isBattleStarting}");
            Debug.Log($"Is Battle Initialized: {isBattleInitialized}");
            Debug.Log($"Battle Sequence Active: {(battleSequenceCoroutine != null)}");
            Debug.Log($"Is Animation Playing: {IsAnimationPlaying}");

            if (roundManager != null)
                Debug.Log($"Current Round: {roundManager.CurrentRound}, Player Wins: {roundManager.PlayerWins}, Enemy Wins: {roundManager.EnemyWins}");

            if (turnManager != null)
                Debug.Log($"Current Turn: {turnManager.CurrentTurn}, Is Player Turn: {turnManager.IsPlayerTurn}");
        }
    }

    // 安全な初期化確認
    public bool IsSafeToStart()
    {
        return isBattleInitialized && !isBattleStarting && !IsMatchOver &&
               gameConfig != null && player != null && enemy != null;
    }

    void OnDestroy()
    {
        // コルーチン停止
        StopBattleSequence();

        // イベントリスナー解除
        RemoveEventListeners();

        // 状態リセット
        isBattleInitialized = false;
        isBattleStarting = false;
    }

    void OnDisable()
    {
        // GameObject無効化時も安全に停止
        StopBattleSequence();
    }
}