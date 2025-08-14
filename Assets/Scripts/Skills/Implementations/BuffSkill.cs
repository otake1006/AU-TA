using CardBattle.Core;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffSkill", menuName = "CardBattle/Skills/Buff")]
public class BuffSkill : SkillBase
{
    [Header("バフ設定")]
    public EffectType buffType = EffectType.AttackBuff;
    [Range(0f, 2f)]
    public float buffValue = 0.5f;
    public int duration = 3;

    [Header("追加MP回復")]
    public bool restoreMP = false;
    public int mpRestoreAmount = 10;

    public override bool CanExecute(BattleContext context, Character caster)
    {
        return caster.Stats.currentMP >= mpCost;
    }

    public override void Execute(BattleContext context, Character caster, Character target)
    {
        caster.Stats.currentMP -= mpCost;

        switch (buffType)
        {
            case EffectType.AttackBuff:
                target.Stats.attackBuff += buffValue;
                target.Stats.attackBuffDuration = duration;
                context.Log($"{target.Name} の攻撃力 +{buffValue * 100}%");
                break;

            case EffectType.DefenseBuff:
                target.Stats.defenseBuff += buffValue;
                target.Stats.defenseBuffDuration = duration;
                context.Log($"{target.Name} の防御力 +{buffValue * 100}%");
                break;

            case EffectType.SpeedBuff:
                target.Stats.speedBuff += buffValue;
                target.Stats.speedBuffDuration = duration;
                context.Log($"{target.Name} の速度 +{buffValue * 100}%");
                break;
        }

        if (restoreMP)
        {
            target.RestoreMP(mpRestoreAmount, context);
        }

        context.Log($"{caster.Name} が {skillName} を発動！");
    }
}