using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "CardBattle/GameSettings")]
public class GameSettings : ScriptableObject
{
    [Header("ラウンド設定")]
    public int maxRounds = 5;
    public int turnsPerRound = 10;

    [Header("初期ステータス")]
    public int startingHP = 100;
    public int startingMP = 50;

    [Header("ゲームメカニクス")]
    public int mpRecoveryPerTurn = 10;
    public int roundEndHPRecover = 10;
    public int roundEndMPRecover = 20;

    [Header("AI設定")]
    public float aiThinkTime = 0.5f;

    [Header("デバッグ")]
    public bool enableDetailedLogging = true;
}