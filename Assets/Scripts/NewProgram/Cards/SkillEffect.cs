using UnityEngine;

[System.Serializable]
public class TurnBasedSkillEffect
{
    [Header("Effect Type")]
    public SkillEffectType effectType;

    [Header("Target")]
    public TargetType targetType;

    [Header("Values")]
    public int value;
    public int duration; // ターン数

    [Header("Associated Buff")]
    public TurnBasedBuffEffect associatedBuff;

    [Header("Conditions")]
    public bool hasCondition = false;
    public SkillCondition condition;

    [Header("Visual")]
    public string effectName;
    public Color effectColor = Color.white;

    public bool CanExecute(Character caster, Character target)
    {
        if (!hasCondition) return true;
        return condition?.IsMet(caster, target) ?? true;
    }

    public string GetDescription()
    {
        string desc = "";

        switch (effectType)
        {
            case SkillEffectType.Damage:
                desc = $"Deal {value} damage";
                break;
            case SkillEffectType.Heal:
                desc = $"Heal {value} HP";
                break;
            case SkillEffectType.Shield:
                desc = $"Gain {value} shield";
                break;
            case SkillEffectType.ApplyBuff:
                desc = $"Apply {associatedBuff?.buffName ?? "buff"}";
                if (duration > 0)
                    desc += $" for {duration} turns";
                break;
            case SkillEffectType.RemoveBuff:
                desc = "Remove debuffs";
                break;
            case SkillEffectType.DrawCard:
                desc = $"Draw {value} card(s)";
                break;
            case SkillEffectType.ManaRestore:
                desc = $"Restore {value} mana";
                break;
            case SkillEffectType.ManaReduce:
                desc = $"Reduce target's mana by {value}";
                break;
        }

        // ターゲット情報を追加
        switch (targetType)
        {
            case TargetType.Self:
                desc += " (self)";
                break;
            case TargetType.Enemy:
                desc += " (enemy)";
                break;
            case TargetType.All:
                desc += " (all)";
                break;
        }

        // 条件情報を追加
        if (hasCondition && condition != null)
        {
            desc += $" [If: {condition.GetConditionDescription()}]";
        }

        return desc;
    }
}