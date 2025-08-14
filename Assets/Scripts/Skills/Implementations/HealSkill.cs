using UnityEngine;

[CreateAssetMenu(fileName = "HealSkill", menuName = "CardBattle/Skills/Heal")]
public class HealSkill : SkillBase
{
    [Header("回復設定")]
    public int baseHealAmount = 25;
    public bool healPercentage = false;
    [Range(0f, 1f)]
    public float healPercent = 0.3f;

    [Header("追加効果")]
    public bool removeDebuffs = false;
    public int bonusShield = 0;

    public override bool CanExecute(BattleContext context, Character caster)
    {
        return caster.Stats.currentMP >= mpCost;
    }

    public override void Execute(BattleContext context, Character caster, Character target)
    {
        caster.Stats.currentMP -= mpCost;

        int healAmount = healPercentage ?
            Mathf.RoundToInt(target.Stats.maxHP * healPercent) :
            baseHealAmount;

        target.Heal(healAmount, context);

        if (bonusShield > 0)
        {
            target.Stats.shield += bonusShield;
            context.Log($"{target.Name} にシールド +{bonusShield}");
        }

        if (removeDebuffs)
        {
            target.Stats.ClearDebuffs();
            context.Log($"{target.Name} のデバフを解除");
        }

        context.Log($"{caster.Name} が {skillName} を発動！ {healAmount} 回復");
    }
}