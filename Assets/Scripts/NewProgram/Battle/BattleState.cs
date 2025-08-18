using UnityEngine;

public class BattleStateManager : MonoBehaviour
{
    private BattleManager battleManager;
    private BattleState currentState;
    private BattleState previousState;

    public BattleState CurrentState => currentState;
    public BattleState PreviousState => previousState;

    public void Initialize(BattleManager manager)
    {
        battleManager = manager;
        GameEvents.OnBattleStateChanged += OnBattleStateChanged;
    }

    void OnBattleStateChanged(BattleState newState)
    {
        previousState = currentState;
        currentState = newState;

        ProcessStateChange(previousState, newState);
    }

    void ProcessStateChange(BattleState from, BattleState to)
    {
        // 状態変更時の処理
        switch (to)
        {
            case BattleState.Initializing:
                OnEnterInitializing();
                break;
            case BattleState.PlayerTurn:
                OnEnterPlayerTurn();
                break;
            case BattleState.EnemyTurn:
                OnEnterEnemyTurn();
                break;
            case BattleState.SkillExecution:
                OnEnterSkillExecution();
                break;
            case BattleState.BuffProcessing:
                OnEnterBuffProcessing();
                break;
            case BattleState.RoundEnd:
                OnEnterRoundEnd();
                break;
            case BattleState.GameOver:
                OnEnterGameOver();
                break;
        }
    }

    void OnEnterInitializing()
    {
        // UI初期化など
        GameEvents.OnUIUpdate?.Invoke();
    }

    void OnEnterPlayerTurn()
    {
        // プレイヤーUI有効化
        GameEvents.OnNotificationShow?.Invoke("Your Turn");
    }

    void OnEnterEnemyTurn()
    {
        // 敵ターンUI表示
        GameEvents.OnNotificationShow?.Invoke("Enemy Turn");
    }

    void OnEnterSkillExecution()
    {
        // スキル実行中UI
    }

    void OnEnterBuffProcessing()
    {
        // バフ処理中
    }

    void OnEnterRoundEnd()
    {
        // ラウンド終了処理
    }

    void OnEnterGameOver()
    {
        // ゲーム終了処理
        GameEvents.OnNotificationShow?.Invoke("Game Over");
    }

    public bool CanPlayerAct()
    {
        return currentState == BattleState.PlayerTurn && !battleManager.IsAnimationPlaying;
    }

    public bool IsInBattle()
    {
        return currentState != BattleState.GameOver && currentState != BattleState.Initializing;
    }

    void OnDestroy()
    {
        GameEvents.OnBattleStateChanged -= OnBattleStateChanged;
    }
}