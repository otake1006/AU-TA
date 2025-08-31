using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Card Game/Game Config")]
public class GameConfig : ScriptableObject
{
    [Header("Match Settings")]
    [Tooltip("勝利に必要なラウンド数")]
    public int winsNeeded = 2;

    [Tooltip("1ラウンドの最大ターン数")]
    public int maxTurnsPerRound = 10;

    [Tooltip("同時撃破を有効にするか")]
    public bool enableSimultaneousDefeat = true;

    [Tooltip("同時撃破時の判定方法")]
    public SimultaneousDefeatRule simultaneousDefeatRule = SimultaneousDefeatRule.Draw;

    [Header("Card Settings")]
    [Tooltip("初期手札枚数")]
    public int initialHandSize = 4;

    [Tooltip("最大手札枚数")]
    public int maxHandSize = 7;

    [Tooltip("ターン開始時のドロー枚数")]
    public int drawPerTurn = 1;

    [Header("Character Settings")]
    [Tooltip("ターン開始時のマナ回復量")]
    public int manaRegenPerTurn = 3;

    [Tooltip("ラウンド間でのバフ継続")]
    public bool persistBuffsBetweenRounds = false;

    [Header("Debug Settings")]
    [Tooltip("デバッグモードを有効にする")]
    public bool enableDebugMode = true;

    [Tooltip("詳細ログを出力する")]
    public bool enableVerboseLogging = false;

    [Tooltip("AIの思考時間（秒）")]
    public float aiThinkingTime = 1f;

    [Tooltip("アニメーション速度倍率")]
    public float animationSpeedMultiplier = 1f;

    [Header("UI Settings")]
    [Tooltip("ダメージテキストの表示時間")]
    public float damageTextDuration = 2f;

    [Tooltip("カードホバー時の拡大率")]
    public float cardHoverScale = 1.1f;

    [Tooltip("バフアイコンの最大表示数")]
    public int maxBuffIconsDisplay = 8;

    [Header("Audio Settings")]
    [Tooltip("BGM音量")]
    [Range(0f, 1f)]
    public float bgmVolume = 0.7f;

    [Tooltip("SE音量")]
    [Range(0f, 1f)]
    public float sfxVolume = 0.8f;

    [Tooltip("ボイス音量")]
    [Range(0f, 1f)]
    public float voiceVolume = 0.9f;

    [Header("Balance Settings")]
    [Tooltip("基本ダメージ倍率")]
    public float baseDamageMultiplier = 1.0f;

    [Tooltip("クリティカル倍率")]
    public float criticalMultiplier = 1.5f;

    [Tooltip("シールド効率")]
    public float shieldEfficiency = 1.0f;
}