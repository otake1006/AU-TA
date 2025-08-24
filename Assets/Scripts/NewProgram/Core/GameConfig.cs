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

    [Header("Relic Settings")]
    [Tooltip("レリック機能を有効にするか")]
    public bool enableRelicSystem = true;
    
    [Tooltip("1キャラクターが持てる最大レリック数")]
    public int maxRelicsPerCharacter = 6;
    
    [Tooltip("レリックがラウンド間で持続するか")]
    public bool persistRelicsBetweenRounds = true;
    
    [Tooltip("レリックがマッチ間で持続するか")]
    public bool persistRelicsBetweenMatches = false;
    
    [Tooltip("レリックカードの出現率")]
    [Range(0f, 1f)]
    public float relicCardSpawnRate = 0.15f;
    
    [Tooltip("レアリティ別出現率 - Common")]
    [Range(0f, 1f)]
    public float commonRelicRate = 0.50f;
    
    [Tooltip("レアリティ別出現率 - Uncommon")]
    [Range(0f, 1f)]
    public float uncommonRelicRate = 0.30f;
    
    [Tooltip("レアリティ別出現率 - Rare")]
    [Range(0f, 1f)]
    public float rareRelicRate = 0.15f;
    
    [Tooltip("レアリティ別出現率 - Epic")]
    [Range(0f, 1f)]
    public float epicRelicRate = 0.04f;
    
    [Tooltip("レアリティ別出現率 - Legendary")]
    [Range(0f, 1f)]
    public float legendaryRelicRate = 0.01f;

    [Header("Defeat Rescue Settings")]
    [Tooltip("敗北救済システムを有効にするか")]
    public bool enableDefeatRescue = true;
    
    [Tooltip("救済で提示されるレリック数")]
    public int rescueRelicCount = 3;
    
    [Tooltip("1マッチでの最大救済回数")]
    public int maxRescueCount = 1;
    
    [Tooltip("救済時のHP回復率")]
    [Range(0f, 1f)]
    public float rescueHealPercentage = 0.5f;
    
    [Tooltip("救済時のマナ回復量")]
    public int rescueManaRestore = 5;
    
    [Tooltip("救済時に重複レリックを許可するか")]
    public bool allowDuplicateRescueRelics = false;

    [Header("Balance Settings")]
    [Tooltip("基本ダメージ倍率")]
    public float baseDamageMultiplier = 1.0f;

    [Tooltip("クリティカル倍率")]
    public float criticalMultiplier = 1.5f;

    [Tooltip("シールド効率")]
    public float shieldEfficiency = 1.0f;
}