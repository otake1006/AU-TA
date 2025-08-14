// File: Assets/CardBattle/Scripts/Core/Enums/BattleEnums.cs

namespace CardBattle.Core
{
    /// <summary>
    /// スキル発動タイミング
    /// </summary>
    public enum SkillTiming
    {
        Instant,        // 即時発動
        TurnStart,      // ターン開始時
        TurnEnd,        // ターン終了時
        RoundStart,     // ラウンド開始時
        RoundEnd,       // ラウンド終了時
        OnDamage,       // ダメージ時
        OnHeal,         // 回復時
        OnSkillUse,     // スキル使用時
        OnConditionMet  // 条件満たした時
    }

    /// <summary>
    /// ターゲットタイプ
    /// </summary>
    public enum TargetType
    {
        Self,    // 自分
        Enemy,   // 敵
        Both,    // 両方
        Random   // ランダム
    }

    /// <summary>
    /// 比較演算子
    /// </summary>
    public enum ComparisonOperator
    {
        GreaterThan,    // >
        LessThan,       // <
        GreaterOrEqual, // >=
        LessOrEqual,    // <=
        Equal,          // ==
        NotEqual        // !=
    }

    /// <summary>
    /// 条件チェック対象
    /// </summary>
    public enum ConditionTarget
    {
        Self,   // 自分
        Enemy,  // 敵
        Both    // 両方
    }

    /// <summary>
    /// エフェクトタイプ
    /// </summary>
    public enum EffectType
    {
        Damage,         // ダメージ
        Heal,           // 回復
        MPRestore,      // MP回復
        MPDrain,        // MP吸収
        Shield,         // シールド
        AttackBuff,     // 攻撃バフ
        DefenseBuff,    // 防御バフ
        SpeedBuff,      // 速度バフ
        AttackDebuff,   // 攻撃デバフ
        DefenseDebuff,  // 防御デバフ
        StatusEffect,   // 状態異常
        Custom          // カスタム
    }

    /// <summary>
    /// レリック発動トリガー
    /// </summary>
    public enum RelicTrigger
    {
        TurnStart,      // ターン開始時
        TurnEnd,        // ターン終了時
        RoundStart,     // ラウンド開始時
        RoundEnd,       // ラウンド終了時
        OnDamageDealt,  // ダメージを与えた時
        OnDamageTaken,  // ダメージを受けた時
        OnHeal,         // 回復時
        OnSkillNotUsed, // スキル未使用時
        OnLowHP,        // HP低下時
        Always          // 常時
    }
}