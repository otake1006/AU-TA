// MP消費型ターンシステムのTurnManager
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private BattleManager battleManager;
    private GameConfig config;

    public int CurrentTurn { get; private set; } = 1;
    public bool IsPlayerTurn { get; private set; } = true;
    public Character FirstToAct { get; private set; }

    // 同時撃破判定用
    private bool playerDiedThisTurn = false;
    private bool enemyDiedThisTurn = false;

    // コルーチン管理
    private Coroutine turnCoroutine;
    private bool isTurnProcessing = false;

    // MP管理
    private bool playerOutOfMP = false;
    private bool enemyOutOfMP = false;

    public void Initialize(BattleManager manager)
    {
        battleManager = manager;
        config = manager.gameConfig;

        // 死亡イベント監視
        GameEvents.OnCharacterDeath += OnCharacterDeath;
    }

    public void StartTurn()
    {
        // 既に処理中のターンがあれば停止
        StopTurnCoroutine();

        if (isTurnProcessing)
        {
            GameEvents.OnDebugMessage?.Invoke($"Turn {CurrentTurn} is already being processed");
            return;
        }

        isTurnProcessing = true;
        battleManager.ChangeState(BattleState.PlayerTurn);

        // 同時撃破フラグリセット
        playerDiedThisTurn = false;
        enemyDiedThisTurn = false;

        // MPフラグリセット
        playerOutOfMP = false;
        enemyOutOfMP = false;

        GameEvents.OnDebugMessage?.Invoke($"Turn {CurrentTurn} - Start");

        // ターン制限チェック
        if (CurrentTurn > config.maxTurnsPerRound)
        {
            EndRoundByTimeout();
            return;
        }

        // ターン開始時の処理
        ProcessTurnStart(battleManager.player);
        ProcessTurnStart(battleManager.enemy);

        // 先攻後攻決定（先攻はプレイヤー固定、または速度で決定）
        IsPlayerTurn = true;
        FirstToAct = battleManager.player;

        // ターン実行開始
        turnCoroutine = StartCoroutine(ExecuteTurnCycle());

        GameEvents.OnTurnChanged?.Invoke(CurrentTurn);
    }

    private void StopTurnCoroutine()
    {
        if (turnCoroutine != null)
        {
            StopCoroutine(turnCoroutine);
            turnCoroutine = null;
        }
    }

    void ProcessTurnStart(Character character)
    {
        // バフ処理
        battleManager.ChangeState(BattleState.BuffProcessing);
        var buffManager = character.GetComponent<TurnBasedBuffManager>();
        buffManager?.OnTurnStart();

        // マナ回復
        character.RestoreMana(config.manaRegenPerTurn);
    }

    // 1ターン内で両方のキャラクターが交互に行動するメインループ
    IEnumerator ExecuteTurnCycle()
    {
        while (!playerOutOfMP || !enemyOutOfMP)
        {
            // 同時撃破チェック
            if (CheckForSimultaneousDefeat()) yield break;

            // 現在行動するキャラクターを決定
            Character currentActor = IsPlayerTurn ? battleManager.player : battleManager.enemy;
            bool isCurrentPlayerTurn = IsPlayerTurn;

            // そのキャラクターがMPを持っているかチェック
            bool hasMP = CheckHasMP(currentActor);

            if (hasMP)
            {
                // 行動実行
                yield return StartCoroutine(ExecuteCharacterAction(currentActor, isCurrentPlayerTurn));

                // 行動後にMPチェック
                UpdateMPStatus();
            }
            else
            {
                // MPがない場合はそのキャラクターをスキップ
                GameEvents.OnDebugMessage?.Invoke($"{currentActor.name} has no MP, skipping");
                if (IsPlayerTurn)
                    playerOutOfMP = true;
                else
                    enemyOutOfMP = true;
            }

            // 次のキャラクターに交代
            IsPlayerTurn = !IsPlayerTurn;

            // もし両方ともMPがなくなったらループ終了
            if (playerOutOfMP && enemyOutOfMP)
                break;

            // 少し間隔をあける
            yield return new WaitForSeconds(0.5f);
        }

        // ターン終了処理
        EndTurn();
    }

    IEnumerator ExecuteCharacterAction(Character character, bool isPlayer)
    {
        if (isPlayer)
        {
            battleManager.ChangeState(BattleState.PlayerTurn);
            yield return new WaitForSeconds(config.aiThinkingTime);

            var player = character.GetComponent<Player>();
            if (player != null)
            {
                yield return StartCoroutine(player.ExecuteTurn());
            }
        }
        else
        {
            battleManager.ChangeState(BattleState.EnemyTurn);
            yield return new WaitForSeconds(config.aiThinkingTime);

            var ai = character.GetComponent<EnemyAI>();
            if (ai != null)
            {
                yield return StartCoroutine(ai.ExecuteTurn());
            }
        }
    }

    bool CheckHasMP(Character character)
    {
        // キャラクターが何らかの行動を取るのに十分なMPを持っているかチェック
        // 最小限必要なMP量（設定可能）
        int minMPRequired = 1; // GameConfigに追加が必要

        return character.CurrentMana >= minMPRequired;
    }

    void UpdateMPStatus()
    {
        // 各キャラクターのMP状況を更新
        playerOutOfMP = !CheckHasMP(battleManager.player);
        enemyOutOfMP = !CheckHasMP(battleManager.enemy);

        if (playerOutOfMP)
            GameEvents.OnDebugMessage?.Invoke("Player out of MP");
        if (enemyOutOfMP)
            GameEvents.OnDebugMessage?.Invoke("Enemy out of MP");
    }

    public void EndTurn()
    {
        if (!isTurnProcessing)
        {
            GameEvents.OnDebugMessage?.Invoke("Turn end ignored - not in turn");
            return;
        }

        // ターン終了処理
        ProcessTurnEnd(battleManager.player);
        ProcessTurnEnd(battleManager.enemy);

        // コルーチン参照をクリア
        turnCoroutine = null;
        isTurnProcessing = false;

        // 同時撃破チェック
        if (CheckForSimultaneousDefeat()) return;

        if (CurrentTurn >= config.maxTurnsPerRound)
        {
            EndRoundByTimeout();
        }
        else
        {
            CurrentTurn++;
            StartTurn(); // 次のターン開始
        }
    }

    void ProcessTurnEnd(Character character)
    {
        battleManager.ChangeState(BattleState.BuffProcessing);
        var buffManager = character.GetComponent<TurnBasedBuffManager>();
        buffManager?.OnTurnEnd();
    }

    void OnCharacterDeath(Character character)
    {
        if (character == battleManager.player)
        {
            playerDiedThisTurn = true;
        }
        else if (character == battleManager.enemy)
        {
            enemyDiedThisTurn = true;
        }

        // 即座に同時撃破チェック
        CheckForSimultaneousDefeat();
    }

    bool CheckForSimultaneousDefeat()
    {
        if (config.enableSimultaneousDefeat && playerDiedThisTurn && enemyDiedThisTurn)
        {
            GameEvents.OnSimultaneousDefeat?.Invoke(battleManager.player, battleManager.enemy);
            StopTurnCoroutine();
            isTurnProcessing = false;
            return true;
        }
        else if (playerDiedThisTurn && !enemyDiedThisTurn)
        {
            battleManager.roundManager.EndRound(RoundResult.EnemyWin);
            StopTurnCoroutine();
            isTurnProcessing = false;
            return true;
        }
        else if (!playerDiedThisTurn && enemyDiedThisTurn)
        {
            battleManager.roundManager.EndRound(RoundResult.PlayerWin);
            StopTurnCoroutine();
            isTurnProcessing = false;
            return true;
        }

        return false;
    }

    void EndRoundByTimeout()
    {
        GameEvents.OnDebugMessage?.Invoke("Round ended by turn limit");

        // HP割合で勝者決定
        var player = battleManager.player;
        var enemy = battleManager.enemy;

        float playerHP = (float)player.CurrentHealth / player.maxHealth;
        float enemyHP = (float)enemy.CurrentHealth / enemy.maxHealth;

        RoundResult result = RoundResult.Timeout;
        if (playerHP > enemyHP)
            result = RoundResult.PlayerWin;
        else if (enemyHP > playerHP)
            result = RoundResult.EnemyWin;
        else
            result = RoundResult.Draw;

        StopTurnCoroutine();
        isTurnProcessing = false;
        battleManager.roundManager.EndRound(result);
    }

    public void ResetTurn()
    {
        StopTurnCoroutine();
        CurrentTurn = 1;
        IsPlayerTurn = true;
        FirstToAct = null;
        playerDiedThisTurn = false;
        enemyDiedThisTurn = false;
        playerOutOfMP = false;
        enemyOutOfMP = false;
        isTurnProcessing = false;
    }

    public void SkipCurrentTurn()
    {
        if (battleManager.gameConfig.enableDebugMode)
        {
            EndTurn();
        }
    }

    // 強制的に現在の行動キャラクターのMPを0にする（デバッグ用）
    public void ForceCurrentCharacterOutOfMP()
    {
        if (battleManager.gameConfig.enableDebugMode)
        {
            Character current = IsPlayerTurn ? battleManager.player : battleManager.enemy;
            current.ConsumeMana(current.CurrentMana);
            UpdateMPStatus();
        }
    }

    void OnDestroy()
    {
        GameEvents.OnCharacterDeath -= OnCharacterDeath;
        StopTurnCoroutine();
    }
}