using System.Collections;
using System.Linq;
using UnityEngine;

// 修正されたRoundManager
public class RoundManager : MonoBehaviour
{
    private BattleManager battleManager;
    private GameConfig config;

    // ラウンド状態
    public int CurrentRound { get; private set; } = 1;
    public int PlayerWins { get; private set; } = 0;
    public int EnemyWins { get; private set; } = 0;

    // コルーチン管理
    private Coroutine nextRoundCoroutine;
    private bool isProcessingRound = false;

    public void Initialize(BattleManager manager)
    {
        battleManager = manager;
        config = manager.gameConfig;
    }

    public void StartNewRound()
    {
        // 既に処理中なら無視
        if (isProcessingRound)
        {
            GameEvents.OnDebugMessage?.Invoke($"Round {CurrentRound} is already being processed");
            return;
        }

        isProcessingRound = true;
        GameEvents.OnDebugMessage?.Invoke($"=== Round {CurrentRound} Start ===");

        // キャラクターリセット
        ResetCharacters();

        // カードリセット
        battleManager.cardManager.ResetForNewRound();

        // todo レリックの効果を適応

        // ターン開始
        //battleManager.turnManager.StartPlayerTurn();
        battleManager.turnManager.StartTurn();

        // UI更新
        GameEvents.OnScoreChanged?.Invoke(PlayerWins, EnemyWins);

        isProcessingRound = false;
    }

    void ResetCharacters()
    {
        var player = battleManager.player;
        var enemy = battleManager.enemy;

        // HP・マナ全回復
        player.ResetToFullHealth();
        player.ResetToFullMana();
        enemy.ResetToFullHealth();
        enemy.ResetToFullMana();

        // シールドクリア
        player.ClearShield();
        enemy.ClearShield();

        // バフクリア（設定による）
        if (!config.persistBuffsBetweenRounds)
        {
            var playerBuffManager = player.GetComponent<TurnBasedBuffManager>();
            var enemyBuffManager = enemy.GetComponent<TurnBasedBuffManager>();

            playerBuffManager?.ClearAllBuffs();
            enemyBuffManager?.ClearAllBuffs();
        }

        GameEvents.OnDebugMessage?.Invoke("キャラクターがリセットされ、新しいラウンドが開始されます");
    }

    public void EndRound(RoundResult result)
    {
        // 既に次のラウンドが開始されている場合は無視
        if (nextRoundCoroutine != null)
        {
            GameEvents.OnDebugMessage?.Invoke($"Round end ignored - next round already scheduled");
            return;
        }

        GameEvents.OnDebugMessage?.Invoke($"=== Round {CurrentRound} End: {result} ===");

        // 勝利数更新
        switch (result)
        {
            case RoundResult.PlayerWin:
                PlayerWins++;
                GameEvents.OnNotificationShow?.Invoke($"Round {CurrentRound}: プレイヤーの勝利!");
                break;
            case RoundResult.EnemyWin:
                EnemyWins++;
                GameEvents.OnNotificationShow?.Invoke($"Round {CurrentRound}: 相手の勝利!");
                break;
            case RoundResult.Draw:
                GameEvents.OnNotificationShow?.Invoke($"Round {CurrentRound}: 引き分け!");
                break;
            case RoundResult.Timeout:
                GameEvents.OnNotificationShow?.Invoke($"Round {CurrentRound}: タイムアウト!");
                break;
        }

        GameEvents.OnRoundEnd?.Invoke(result, CurrentRound);
        GameEvents.OnScoreChanged?.Invoke(PlayerWins, EnemyWins);

        // 次ラウンドまたはマッチ終了
        if (!IsMatchComplete())
        {
            CurrentRound++;
            nextRoundCoroutine = StartCoroutine(StartNextRoundAfterDelay());
        }
        else
        {
            GameEvents.OnDebugMessage?.Invoke("Match Complete!");
        }
    }

    IEnumerator StartNextRoundAfterDelay()
    {
        GameEvents.OnDebugMessage?.Invoke($"Waiting 3 seconds before Round {CurrentRound}...");
        yield return new WaitForSeconds(3f);

        // コルーチン参照をクリア
        nextRoundCoroutine = null;

        StartNewRound();
    }

    public void StopCurrentRoundProcess()
    {
        if (nextRoundCoroutine != null)
        {
            StopCoroutine(nextRoundCoroutine);
            nextRoundCoroutine = null;
            GameEvents.OnDebugMessage?.Invoke("Next round process stopped");
        }
        isProcessingRound = false;
    }

    public bool IsMatchComplete()
    {
        return PlayerWins >= config.winsNeeded || EnemyWins >= config.winsNeeded;
    }

    public Character GetMatchWinner()
    {
        if (PlayerWins >= config.winsNeeded)
            return battleManager.player;
        if (EnemyWins >= config.winsNeeded)
            return battleManager.enemy;
        return null;
    }

    public void ResetMatch()
    {
        // 進行中のコルーチンを停止
        StopCurrentRoundProcess();

        CurrentRound = 1;
        PlayerWins = 0;
        EnemyWins = 0;
        GameEvents.OnScoreChanged?.Invoke(PlayerWins, EnemyWins);
    }

    // デバッグ用
    public void DebugAddPlayerWin()
    {
        if (battleManager.gameConfig.enableDebugMode)
        {
            PlayerWins++;
            GameEvents.OnScoreChanged?.Invoke(PlayerWins, EnemyWins);
        }
    }

    public void DebugAddEnemyWin()
    {
        if (battleManager.gameConfig.enableDebugMode)
        {
            EnemyWins++;
            GameEvents.OnScoreChanged?.Invoke(PlayerWins, EnemyWins);
        }
    }
}