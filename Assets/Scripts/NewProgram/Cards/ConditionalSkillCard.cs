using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "New Conditional Skill Card", menuName = "Card Game/Conditional Skill Card")]
public class ConditionalSkillCard : SkillCard
{
    [Header("Usage Conditions")]
    [Tooltip("このスキルを使用するための条件（最大2つ）")]
    public SkillCondition[] usageConditions = new SkillCondition[2];

    [Header("Enhanced Effects")]
    [Tooltip("条件を満たした時の追加効果")]
    public TurnBasedSkillEffect[] enhancedEffects;
    public bool hasEnhancedVersion = false;

    [Header("Enhanced Audio")]
    public AudioClip enhancedSkillSound;

    public override bool CanUse(Character caster, Character target)
    {
        if (!base.CanUse(caster, target))
            return false;

        // 全ての条件をチェック
        foreach (var condition in usageConditions)
        {
            if (condition != null && !condition.IsMet(caster, target))
            {
                return false;
            }
        }

        return true;
    }

    // 条件を満たしているかチェック（強化版用）
    public bool IsEnhanced(Character caster, Character target)
    {
        if (!hasEnhancedVersion) return false;

        // 全ての条件を満たしている場合のみ強化版
        foreach (var condition in usageConditions)
        {
            if (condition != null && !condition.IsMet(caster, target))
            {
                return false;
            }
        }

        return true;
    }

    // 使用する効果を取得
    public TurnBasedSkillEffect[] GetEffectsToUse(Character caster, Character target)
    {
        if (IsEnhanced(caster, target) && enhancedEffects != null && enhancedEffects.Length > 0)
        {
            // 基本効果 + 強化効果
            List<TurnBasedSkillEffect> allEffects = new List<TurnBasedSkillEffect>(effects);
            allEffects.AddRange(enhancedEffects);
            return allEffects.ToArray();
        }
        return effects;
    }

    public override string GetTooltipText()
    {
        string tooltip = base.GetTooltipText();

        // 使用条件を追加
        var validConditions = usageConditions.Where(c => c != null).ToArray();
        if (validConditions.Length > 0)
        {
            tooltip += "\n\n<b><color=orange>Conditions:</color></b>";
            foreach (var condition in validConditions)
            {
                tooltip += $"\n? {condition.GetConditionDescription()}";
            }
        }

        // 強化効果の説明を追加
        if (hasEnhancedVersion && enhancedEffects != null && enhancedEffects.Length > 0)
        {
            tooltip += "\n\n<b><color=gold>Enhanced Effects:</color></b>";
            foreach (var effect in enhancedEffects)
            {
                tooltip += $"\n? {GetEffectDescription(effect)}";
            }
        }

        return tooltip;
    }

    public string GetConditionStatus(Character caster, Character target)
    {
        string status = "";
        for (int i = 0; i < usageConditions.Length; i++)
        {
            var condition = usageConditions[i];
            if (condition != null)
            {
                bool met = condition.IsMet(caster, target);
                string color = met ? "green" : "red";
                status += $"<color={color}>Condition {i + 1}: {(met ? "?" : "?")}</color>\n";
            }
        }
        return status;
    }
}