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
        gameConfig = config;
        player = playerChar;
        enemy = enemyChar;

        SetupManagers();
        SetupEventListeners();
        GameEvents.OnDebugMessage?.Invoke("BattleManager initialized");
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
        GameEvents.OnCharacterDeath += OnCharacterDeath;
        GameEvents.OnSimultaneousDefeat += OnSimultaneousDefeat;
        GameEvents.OnRoundEnd += OnRoundEnd;
        GameEvents.OnCardUsed += OnCardUsed;
    }

    public void StartBattle()
    {
        StartCoroutine(StartBattleSequence());
    }

    IEnumerator StartBattleSequence()
    {
        ChangeState(BattleState.Initializing);

        // 初期化処理
        yield return new WaitForSeconds(1f);

        // バトル開始
        GameEvents.OnDebugMessage?.Invoke("Battle Start!");
        ChangeState(BattleState.PlayerTurn);
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
            GameEvents.OnDebugMessage?.Invoke($"Battle State: {previousState} → {newState}");
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
        GameEvents.OnDebugMessage?.Invoke($"{character.characterName} defeated!");
    }

    void OnSimultaneousDefeat(Character player, Character enemy)
    {
        GameEvents.OnDebugMessage?.Invoke("Simultaneous defeat occurred!");

        RoundResult result = SimultaneousDefeat.DetermineResult(
            gameConfig.simultaneousDefeatRule,
            player,
            enemy,
            turnManager.FirstToAct
        );

        roundManager.EndRound(result);
    }

    void OnRoundEnd(RoundResult result, int round)
    {
        if (roundManager.IsMatchComplete())
        {
            EndMatch();
        }
    }

    void OnCardUsed(ConditionalSkillCard card, Character caster, Character target)
    {
        GameEvents.OnDebugMessage?.Invoke($"{caster.characterName} used {card.cardName}");
    }

    void EndMatch()
    {
        IsMatchOver = true;
        ChangeState(BattleState.GameOver);

        Character winner = roundManager.GetMatchWinner();
        GameEvents.OnMatchEnd?.Invoke(winner);

        GameEvents.OnDebugMessage?.Invoke($"Match ended! Winner: {winner?.characterName}");
    }

    public void RestartBattle()
    {
        IsMatchOver = false;
        roundManager.ResetMatch();
        turnManager.ResetTurn();
        cardManager.ResetAllCards();

        StartBattle();
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
        if (gameConfig.enableDebugMode && !IsAnimationPlaying)
        {
            turnManager.SkipCurrentTurn();
        }
    }

    void OnDestroy()
    {
        // イベントリスナー解除
        GameEvents.OnCharacterDeath -= OnCharacterDeath;
        GameEvents.OnSimultaneousDefeat -= OnSimultaneousDefeat;
        GameEvents.OnRoundEnd -= OnRoundEnd;
        GameEvents.OnCardUsed -= OnCardUsed;
    }
}