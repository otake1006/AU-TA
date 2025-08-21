using UnityEngine;
using System.Collections;

public class RoundManager : MonoBehaviour
{
    private BattleManager battleManager;
    private GameConfig config;

    // ラウンド状態
    public int CurrentRound { get; private set; } = 1;
    public int PlayerWins { get; private set; } = 0;
    public int EnemyWins { get; private set; } = 0;

    public void Initialize(BattleManager manager)
    {
        battleManager = manager;
        config = manager.gameConfig;
    }

    public void StartNewRound()
    {
        GameEvents.OnDebugMessage?.Invoke($"=== Round {CurrentRound} Start ===");

        // キャラクターリセット
        ResetCharacters();

        // カードリセット
        battleManager.cardManager.ResetForNewRound();

        // ターン開始
        battleManager.turnManager.StartPlayerTurn();

        // UI更新
        GameEvents.OnScoreChanged?.Invoke(PlayerWins, EnemyWins);
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

        GameEvents.OnDebugMessage?.Invoke("Characters reset for new round");
    }

    public void EndRound(RoundResult result)
    {
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
            StartCoroutine(StartNextRoundAfterDelay());
        }
    }

    IEnumerator StartNextRoundAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        StartNewRound();
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