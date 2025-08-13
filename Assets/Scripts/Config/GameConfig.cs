using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "CardBattle/GameConfig")]
[System.Serializable]
public class GameConfig : ScriptableObject
{
    [Header("ゲーム基本設定")]
    [Tooltip("最大ラウンド数")]
    public int maxRounds = 5;

    [Tooltip("1ラウンドあたりの最大ターン数")]
    public int maxTurnsPerRound = 10;

    [Tooltip("勝利に必要なラウンド数")]
    public int winsNeededForVictory = 3;

    [Header("プレイヤー初期設定")]
    [Tooltip("初期HP")]
    public int initialHP = 100;

    [Tooltip("初期最大HP")]
    public int initialMaxHP = 100;

    [Tooltip("初期MP")]
    public int initialMP = 5;

    [Tooltip("初期最大MP")]
    public int initialMaxMP = 5;

    [Tooltip("初期手札枚数")]
    public int initialHandSize = 3;

    [Header("MP回復設定")]
    [Tooltip("ターン開始時のMP回復量")]
    public int mpRecoveryPerTurn = 2;

    [Tooltip("最大MP制限を超えて回復可能")]
    public bool canOverchargeMP = false;

    [Tooltip("オーバーチャージ時の最大MP倍率")]
    [Range(1.0f, 3.0f)]
    public float maxMPOverchargeMultiplier = 1.5f;

    [Header("HP回復設定")]
    [Tooltip("ラウンド間でHPを完全初期化")]
    public bool hpResetBetweenRounds = true;

    [Tooltip("HPリセットしない場合の回復量")]
    public int hpRecoveryBetweenRounds = 50;

    [Tooltip("ラウンド間でのHP回復量の割合（0-1）")]
    [Range(0f, 1f)]
    public float hpRecoveryPercentage = 0.5f;

    [Header("聖遺物設定")]
    [Tooltip("敗者に聖遺物を付与")]
    public bool giveRelicToLoser = true;

    [Tooltip("聖遺物のHP増加量")]
    public int relicHPBonus = 20;

    [Tooltip("聖遺物のMP増加量")]
    public int relicMPBonus = 2;

    [Tooltip("聖遺物の効果倍率")]
    [Range(0.5f, 2.0f)]
    public float relicEffectMultiplier = 1.0f;

    [Header("戦術スキル設定")]
    [Tooltip("毒スキルのダメージ")]
    public int poisonDamage = 5;

    [Tooltip("毒スキルの持続ターン")]
    public int poisonDuration = 3;

    [Tooltip("再生スキルの回復量")]
    public int regenerationHeal = 8;

    [Tooltip("再生スキルの持続ターン")]
    public int regenerationDuration = 5;

    [Tooltip("条件付き攻撃スキルのダメージ")]
    public int conditionalAttackDamage = 30;

    [Tooltip("条件付き攻撃発動のHP閾値（割合）")]
    [Range(0f, 1f)]
    public float conditionalAttackHPThreshold = 0.25f;

    [Header("カード設定")]
    [Tooltip("基本攻撃カードのダメージ")]
    public int basicAttackDamage = 15;

    [Tooltip("基本攻撃カードのMPコスト")]
    public int basicAttackCost = 2;

    [Tooltip("基本回復カードの回復量")]
    public int basicHealAmount = 20;

    [Tooltip("基本回復カードのMPコスト")]
    public int basicHealCost = 2;

    [Tooltip("強力な攻撃カードのダメージ")]
    public int powerAttackDamage = 25;

    [Tooltip("強力な攻撃カードのMPコスト")]
    public int powerAttackCost = 3;

    [Header("AI設定")]
    [Tooltip("AIの思考時間（秒）")]
    [Range(0f, 5f)]
    public float aiThinkingTime = 1f;

    [Tooltip("AIの回復閾値（HP割合）")]
    [Range(0f, 1f)]
    public float aiHealThreshold = 0.3f;

    [Tooltip("AIの攻撃優先度")]
    [Range(0f, 1f)]
    public float aiAggressionLevel = 0.7f;

    [Header("バランス調整")]
    [Tooltip("全体的なダメージ倍率")]
    [Range(0.1f, 3.0f)]
    public float globalDamageMultiplier = 1.0f;

    [Tooltip("全体的な回復倍率")]
    [Range(0.1f, 3.0f)]
    public float globalHealMultiplier = 1.0f;

    [Tooltip("MPコスト倍率")]
    [Range(0.1f, 3.0f)]
    public float mpCostMultiplier = 1.0f;

    [Header("デバッグ設定")]
    [Tooltip("詳細ログを表示")]
    public bool enableDetailedLogging = true;

    [Tooltip("戦術ボードの詳細ログ")]
    public bool enableTacticalBoardLogging = true;

    [Tooltip("ターン終了時に状態表示")]
    public bool showStateAfterTurn = true;

    // 計算されたプロパティ
    public int CalculatedConditionalAttackThreshold => Mathf.RoundToInt(initialHP * conditionalAttackHPThreshold);
    public int CalculatedMaxMPWithOvercharge => Mathf.RoundToInt(initialMaxMP * maxMPOverchargeMultiplier);
    public int CalculatedHPRecovery => Mathf.RoundToInt(initialHP * hpRecoveryPercentage);

    // スケールされたパラメータを取得するメソッド
    public int GetScaledDamage(int baseDamage) => Mathf.RoundToInt(baseDamage * globalDamageMultiplier);
    public int GetScaledHeal(int baseHeal) => Mathf.RoundToInt(baseHeal * globalHealMultiplier);
    public int GetScaledMPCost(int baseCost) => Mathf.Max(1, Mathf.RoundToInt(baseCost * mpCostMultiplier));
    public int GetScaledRelicEffect(int baseEffect) => Mathf.RoundToInt(baseEffect * relicEffectMultiplier);

    // プリセット設定
    [Header("プリセット")]
    [Space(10)]
    [Tooltip("バランス調整用のプリセットボタン")]
    public bool applyEasyMode = false;
    public bool applyNormalMode = false;
    public bool applyHardMode = false;
    public bool applyTestMode = false;

    void OnValidate()
    {
        if (applyEasyMode)
        {
            ApplyEasyModeSettings();
            applyEasyMode = false;
        }
        else if (applyNormalMode)
        {
            ApplyNormalModeSettings();
            applyNormalMode = false;
        }
        else if (applyHardMode)
        {
            ApplyHardModeSettings();
            applyHardMode = false;
        }
        else if (applyTestMode)
        {
            ApplyTestModeSettings();
            applyTestMode = false;
        }
    }

    void ApplyEasyModeSettings()
    {
        initialHP = 150;
        mpRecoveryPerTurn = 3;
        poisonDamage = 3;
        globalDamageMultiplier = 0.8f;
        globalHealMultiplier = 1.2f;
        aiAggressionLevel = 0.5f;
        Debug.Log("イージーモード設定を適用しました");
    }

    void ApplyNormalModeSettings()
    {
        initialHP = 100;
        mpRecoveryPerTurn = 2;
        poisonDamage = 5;
        globalDamageMultiplier = 1.0f;
        globalHealMultiplier = 1.0f;
        aiAggressionLevel = 0.7f;
        Debug.Log("ノーマルモード設定を適用しました");
    }

    void ApplyHardModeSettings()
    {
        initialHP = 80;
        mpRecoveryPerTurn = 1;
        poisonDamage = 8;
        globalDamageMultiplier = 1.3f;
        globalHealMultiplier = 0.8f;
        aiAggressionLevel = 0.9f;
        Debug.Log("ハードモード設定を適用しました");
    }

    void ApplyTestModeSettings()
    {
        initialHP = 50;
        maxTurnsPerRound = 5;
        mpRecoveryPerTurn = 5;
        globalDamageMultiplier = 2.0f;
        enableDetailedLogging = true;
        aiThinkingTime = 0.1f;
        Debug.Log("テストモード設定を適用しました");
    }
}