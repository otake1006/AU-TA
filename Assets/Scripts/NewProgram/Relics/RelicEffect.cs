using UnityEngine;

[System.Serializable]
public abstract class RelicEffect : TurnBasedBuffEffect
{
    [Header("Relic Settings")]
    public RelicRarity rarity = RelicRarity.Common;
    public RelicCategory category = RelicCategory.Combat;
    public bool isStackable = false;
    
    [Header("Acquisition")]
    public int acquisitionCost = 0;
    public string flavorText;

    public override void Initialize(Character target, int duration = -1)
    {
        base.Initialize(target, duration);
        // レリックは常に永続的
        isPermanent = true;
        buffType = BuffType.Relic;
        maxStacks = isStackable ? 10 : 1;
    }

    // レリック固有のメソッド
    public virtual void OnRelicAcquired()
    {
        // レリック獲得時の処理
        GameEvents.OnDebugMessage?.Invoke($"Relic acquired: {buffName}");
    }

    public virtual void OnRelicLost()
    {
        // レリック失う時の処理
        GameEvents.OnDebugMessage?.Invoke($"Relic lost: {buffName}");
    }

    // 戦闘開始時の効果
    public virtual void OnBattleStart()
    {
        // 戦闘開始時に発動する効果
    }

    // ラウンド開始時の効果
    public virtual void OnRoundStart()
    {
        // ラウンド開始時に発動する効果
    }

    // マッチ終了時の効果
    public virtual void OnMatchEnd(Character winner)
    {
        // マッチ終了時に発動する効果
    }

    // レリック説明の詳細取得
    public override string GetDetailedDescription()
    {
        string details = $"<b><color={GetRarityColor()}>{buffName}</color></b>\n";
        details += $"<color=yellow>Relic - {rarity}</color>\n";
        details += $"<color=gray>{category}</color>\n";
        
        if (isStackable && stackCount > 1)
            details += $"Stacks: {stackCount}\n";
        
        details += $"\n{description}";
        
        if (!string.IsNullOrEmpty(flavorText))
            details += $"\n\n<i><color=gray>\"{flavorText}\"</color></i>";

        return details;
    }

    private string GetRarityColor()
    {
        switch (rarity)
        {
            case RelicRarity.Common: return "white";
            case RelicRarity.Uncommon: return "lime";
            case RelicRarity.Rare: return "cyan";
            case RelicRarity.Epic: return "magenta";
            case RelicRarity.Legendary: return "gold";
            default: return "white";
        }
    }

    public override int GetPriority()
    {
        // レリックは最高優先度で表示
        return 10;
    }

    public virtual bool CanAcquire(Character character)
    {
        // レリック獲得可能かチェック
        return true;
    }

    public virtual string GetShortDescription()
    {
        return description.Length > 50 ? description.Substring(0, 47) + "..." : description;
    }
}

// レリック関連の列挙型
public enum RelicRarity
{
    Common,     // 一般的
    Uncommon,   // 珍しい
    Rare,       // レア
    Epic,       // エピック
    Legendary   // レジェンダリー
}

public enum RelicCategory
{
    Combat,     // 戦闘
    Defense,    // 防御
    Utility,    // ユーティリティ
    Economic,   // 経済
    Special     // 特殊
}