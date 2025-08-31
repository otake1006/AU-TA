using UnityEngine;

[System.Serializable]
public abstract class RelicEffect : ScriptableObject
{
    [Header("Basic Info")]
    public int relicID;
    public string relicName;
    public RelicRarity rarity;
    public Sprite relicIcon;

    [Header("Description")]
    [TextArea(2, 4)]
    public string description;

    [Header("Trigger Settings")]
    public RelicTriggerType triggerType = RelicTriggerType.Passive;

    [Header("Stack Settings")]
    public bool canStack = false;
    public int maxStacks = 1;
    public int stackCount = 1;

    protected Character owner;

    // プロパティ
    public Character Owner => owner;
    public int StackCount => stackCount;

    public virtual void Initialize(Character owner)
    {
        this.owner = owner;
        this.stackCount = 1;
    }

    // 抽象メソッド - 各レリックで実装が必要
    public abstract void OnAcquired();
    public abstract void OnRemoved();

    // 仮想メソッド - 必要に応じてオーバーライド
    public virtual void OnStack()
    {
        if (canStack)
        {
            stackCount = Mathf.Min(stackCount + 1, maxStacks);
            OnStackEffect();
        }
    }

    protected virtual void OnStackEffect()
    {
        // スタック時の追加効果
    }

    // イベントトリガー系メソッド
    public virtual void OnBattleStart()
    {
        // 戦闘開始時の処理
    }

    public virtual void OnBattleEnd()
    {
        // 戦闘終了時の処理
    }

    public virtual void OnTurnStart()
    {
        // ターン開始時の処理
    }

    public virtual void OnTurnEnd()
    {
        // ターン終了時の処理
    }

    public virtual void OnCardPlayed(ConditionalSkillCard card)
    {
        // カード使用時の処理
    }

    public virtual void OnDamageDealt(int damage, Character target, DamageType damageType)
    {
        // ダメージを与えた時の処理
    }

    public virtual void OnDamageTaken(int damage, Character attacker, DamageType damageType)
    {
        // ダメージを受けた時の処理
    }

    public virtual void OnHealReceived(int healAmount)
    {
        // 回復を受けた時の処理
    }

    public virtual void OnBuffApplied(TurnBasedBuffEffect buff)
    {
        // バフが適用された時の処理
    }

    public virtual void OnBuffRemoved(TurnBasedBuffEffect buff)
    {
        // バフが除去された時の処理
    }

    public virtual void OnEnemyDefeated(Character enemy)
    {
        // 敵を倒した時の処理
    }

    public virtual void OnLowHealth(float healthPercentage)
    {
        // 体力が低下した時の処理
    }

    // レリックの詳細情報を取得
    public virtual string GetDetailedDescription()
    {
        string details = $"<b>{relicName}</b>\n";
        details += $"<color={GetRarityColor()}>{rarity}</color>\n";

        if (canStack && stackCount > 1)
            details += $"Stacks: {stackCount}/{maxStacks}\n";

        details += $"\n{description}";
        return details;
    }

    public virtual string GetRarityColor()
    {
        return rarity switch
        {
            RelicRarity.Common => "white",
            RelicRarity.Uncommon => "green",
            RelicRarity.Rare => "blue",
            RelicRarity.Epic => "purple",
            RelicRarity.Legendary => "orange",
            _ => "white"
        };
    }

    // レリックの効果値を取得（UIでの表示用）
    public virtual float GetEffectValue()
    {
        return 0f;
    }

    // レリックの優先度を取得（表示順序用）
    public virtual int GetPriority()
    {
        return (int)rarity;
    }

    public override string ToString()
    {
        return $"{relicName} (ID:{relicID}, Rarity:{rarity}, Stacks:{stackCount})";
    }
}