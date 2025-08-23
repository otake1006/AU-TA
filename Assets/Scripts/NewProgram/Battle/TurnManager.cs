using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

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

    public void Initialize(BattleManager manager)
    {
        battleManager = manager;
        config = manager.gameConfig;

        // 死亡イベント監視
        GameEvents.OnCharacterDeath += OnCharacterDeath;
    }

    public void StartPlayerTurn()
    {
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
        StartCoroutine(ExecutePlayer());

        GameEvents.OnTurnChanged?.Invoke(CurrentTurn);
    }

    public void StartEnemyTurn()
    {
        IsPlayerTurn = false;
        battleManager.ChangeState(BattleState.EnemyTurn);

        GameEvents.OnDebugMessage?.Invoke($"Turn {CurrentTurn} - Enemy");

        ProcessTurnStart(battleManager.enemy);

        // AI行動
        StartCoroutine(ExecuteEnemyAI());
    }

    void ProcessTurnStart(Character character)
    {
        // バフ処理
        battleManager.ChangeState(BattleState.BuffProcessing);
        var buffManager = character.GetComponent<TurnBasedBuffManager>();
        buffManager?.OnTurnStart();

        // マナ回復
        character.RestoreMana(config.manaRegenPerTurn);

        // カードドロー
        //battleManager.cardManager.DrawCards(character, config.drawPerTurn);

        // 状態を戻す
        battleManager.ChangeState(IsPlayerTurn ? BattleState.PlayerTurn : BattleState.EnemyTurn);
    }

    public void EndPlayerTurn()
    {
        //GameEvents.OnDebugMessage?.Invoke("プレイヤーターン終了");

        ProcessTurnEnd(battleManager.player);

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
        //GameEvents.OnDebugMessage?.Invoke("相手ターン終了");

        ProcessTurnEnd(battleManager.enemy);

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

    IEnumerator ExecutePlayer()
    {
        yield return new WaitForSeconds(config.aiThinkingTime);

        // Player処理
        var player = battleManager.player.GetComponent<Player>();
        if (player != null)
        {
            yield return StartCoroutine(player.ExecuteTurn());
        }

        EndPlayerTurn();
    }

    IEnumerator ExecuteEnemyAI()
    {
        yield return new WaitForSeconds(config.aiThinkingTime);

        // AI処理
        var ai = battleManager.enemy.GetComponent<EnemyAI>();
        if (ai != null)
        {
            yield return StartCoroutine(ai.ExecuteTurn());
        }

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
            return true;
        }
        else if (playerDiedThisTurn && !enemyDiedThisTurn)
        {
            battleManager.roundManager.EndRound(RoundResult.EnemyWin);
            return true;
        }
        else if (!playerDiedThisTurn && enemyDiedThisTurn)
        {
            battleManager.roundManager.EndRound(RoundResult.PlayerWin);
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

        battleManager.roundManager.EndRound(result);
    }

    public void ResetTurn()
    {
        CurrentTurn = 1;
        IsPlayerTurn = true;
        FirstToAct = null;
        playerDiedThisTurn = false;
        enemyDiedThisTurn = false;
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
    }
}
