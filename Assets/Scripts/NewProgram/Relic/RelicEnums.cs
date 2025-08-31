public enum RelicRarity
{
    Common = 0,
    Uncommon = 1,
    Rare = 2,
    Epic = 3,
    Legendary = 4
}

public enum RelicTriggerType
{
    Passive,        // 常時効果
    BattleStart,    // 戦闘開始時
    BattleEnd,      // 戦闘終了時
    TurnStart,      // ターン開始時
    TurnEnd,        // ターン終了時
    CardPlayed,     // カード使用時
    DamageDealt,    // ダメージを与えた時
    DamageTaken,    // ダメージを受けた時
    HealReceived,   // 回復を受けた時
    BuffApplied,    // バフが適用された時
    BuffRemoved,    // バフが除去された時
    EnemyDefeated,  // 敵を倒した時
    LowHealth       // 体力が低下した時
}