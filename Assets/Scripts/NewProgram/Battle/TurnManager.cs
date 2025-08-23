// 修正されたTurnManager
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
    private Coroutine playerTurnCoroutine;
    private Coroutine enemyTurnCoroutine;
    private bool isTurnProcessing = false;

    public void Initialize(BattleManager manager)
    {
        battleManager = manager;
        config = manager.gameConfig;

        // 死亡イベント監視
        GameEvents.OnCharacterDeath += OnCharacterDeath;
    }

    public void StartPlayerTurn()
    {
        // 既に処理中のターンがあれば停止
        StopAllTurnCoroutines();

        if (isTurnProcessing)
        {
            GameEvents.OnDebugMessage?.Invoke($"Turn {CurrentTurn} is already being processed");
            return;
        }

        isTurnProcessing = true;
        IsPlayerTurn = true;
        FirstToAct = battleManager.player;
        battleManager.ChangeState(BattleState.PlayerTurn);

        // 同時撃破フラグリセット
        playerDiedThisTurn = false;
        enemyDiedThisTurn = false;

        GameEvents.OnDebugMessage?.Invoke($"Turn {CurrentTurn} - Player");

        // ターン制限チェック
        if (CurrentTurn > config.maxTurnsPerRound)
        {
            EndRoundByTimeout();
            return;
        }

        ProcessTurnStart(battleManager.player);

        // プレイヤー行動
        playerTurnCoroutine = StartCoroutine(ExecutePlayerWithSafety());

        GameEvents.OnTurnChanged?.Invoke(CurrentTurn);
    }

    public void StartEnemyTurn()
    {
        // 既に処理中のターンがあれば停止
        StopAllTurnCoroutines();

        if (isTurnProcessing)
        {
            GameEvents.OnDebugMessage?.Invoke($"Enemy turn {CurrentTurn} is already being processed");
            return;
        }

        isTurnProcessing = true;
        IsPlayerTurn = false;
        battleManager.ChangeState(BattleState.EnemyTurn);

        GameEvents.OnDebugMessage?.Invoke($"Turn {CurrentTurn} - Enemy");

        ProcessTurnStart(battleManager.enemy);

        // AI行動
        enemyTurnCoroutine = StartCoroutine(ExecuteEnemyAIWithSafety());
    }

    private void StopAllTurnCoroutines()
    {
        if (playerTurnCoroutine != null)
        {
            StopCoroutine(playerTurnCoroutine);
            playerTurnCoroutine = null;
        }

        if (enemyTurnCoroutine != null)
        {
            StopCoroutine(enemyTurnCoroutine);
            enemyTurnCoroutine = null;
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

        // 状態を戻す
        battleManager.ChangeState(IsPlayerTurn ? BattleState.PlayerTurn : BattleState.EnemyTurn);
    }

    public void EndPlayerTurn()
    {
        if (!isTurnProcessing || !IsPlayerTurn)
        {
            GameEvents.OnDebugMessage?.Invoke("Player turn end ignored - not in player turn");
            return;
        }

        ProcessTurnEnd(battleManager.player);

        // コルーチン参照をクリア
        playerTurnCoroutine = null;
        isTurnProcessing = false;

        // 同時撃破チェック
        if (CheckForSimultaneousDefeat()) return;

        if (CurrentTurn >= config.maxTurnsPerRound)
        {
            EndRoundByTimeout();
        }
        else
        {
            StartEnemyTurn();
        }
    }

    public void EndEnemyTurn()
    {
        if (!isTurnProcessing || IsPlayerTurn)
        {
            GameEvents.OnDebugMessage?.Invoke("Enemy turn end ignored - not in enemy turn");
            return;
        }

        ProcessTurnEnd(battleManager.enemy);

        // コルーチン参照をクリア
        enemyTurnCoroutine = null;
        isTurnProcessing = false;

        // 同時撃破チェック
        if (CheckForSimultaneousDefeat()) return;

        CurrentTurn++;
        StartPlayerTurn();
    }

    void ProcessTurnEnd(Character character)
    {
        battleManager.ChangeState(BattleState.BuffProcessing);
        var buffManager = character.GetComponent<TurnBasedBuffManager>();
        buffManager?.OnTurnEnd();
    }

    IEnumerator ExecutePlayerWithSafety()
    {
        yield return new WaitForSeconds(config.aiThinkingTime);

        // 途中でキャンセルされた場合の確認
        if (playerTurnCoroutine == null) yield break;

        // Player処理
        var player = battleManager.player.GetComponent<Player>();
        if (player != null)
        {
            yield return StartCoroutine(player.ExecuteTurn());
        }

        // 途中でキャンセルされた場合の確認
        if (playerTurnCoroutine == null) yield break;

        EndPlayerTurn();
    }

    IEnumerator ExecuteEnemyAIWithSafety()
    {
        yield return new WaitForSeconds(config.aiThinkingTime);

        // 途中でキャンセルされた場合の確認
        if (enemyTurnCoroutine == null) yield break;

        // AI処理
        var ai = battleManager.enemy.GetComponent<EnemyAI>();
        if (ai != null)
        {
            yield return StartCoroutine(ai.ExecuteTurn());
        }

        // 途中でキャンセルされた場合の確認
        if (enemyTurnCoroutine == null) yield break;

        EndEnemyTurn();
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
            StopAllTurnCoroutines();
            isTurnProcessing = false;
            return true;
        }
        else if (playerDiedThisTurn && !enemyDiedThisTurn)
        {
            battleManager.roundManager.EndRound(RoundResult.EnemyWin);
            StopAllTurnCoroutines();
            isTurnProcessing = false;
            return true;
        }
        else if (!playerDiedThisTurn && enemyDiedThisTurn)
        {
            battleManager.roundManager.EndRound(RoundResult.PlayerWin);
            StopAllTurnCoroutines();
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

        StopAllTurnCoroutines();
        isTurnProcessing = false;
        battleManager.roundManager.EndRound(result);
    }

    public void ResetTurn()
    {
        StopAllTurnCoroutines();
        CurrentTurn = 1;
        IsPlayerTurn = true;
        FirstToAct = null;
        playerDiedThisTurn = false;
        enemyDiedThisTurn = false;
        isTurnProcessing = false;
    }

    public void SkipCurrentTurn()
    {
        if (battleManager.gameConfig.enableDebugMode)
        {
            if (IsPlayerTurn)
                EndPlayerTurn();
            else
                EndEnemyTurn();
        }
    }

    void OnDestroy()
    {
        GameEvents.OnCharacterDeath -= OnCharacterDeath;
        StopAllTurnCoroutines();
    }
}