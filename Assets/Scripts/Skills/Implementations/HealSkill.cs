using UnityEngine;

[CreateAssetMenu(fileName = "HealSkill", menuName = "CardBattle/Skills/Heal")]
public class HealSkill : SkillBase
{
    [Header("�񕜐ݒ�")]
    public int baseHealAmount = 25;
    public bool healPercentage = false;
    [Range(0f, 1f)]
    public float healPercent = 0.3f;

    [Header("�ǉ�����")]
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
            context.Log($"{target.Name} �ɃV�[���h +{bonusShield}");
        }

        if (removeDebuffs)
        {
            target.Stats.ClearDebuffs();
            context.Log($"{target.Name} �̃f�o�t������");
        }

        context.Log($"{caster.Name} �� {skillName} �𔭓��I {healAmount} ��");
    }
}