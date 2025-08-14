using UnityEngine;

[CreateAssetMenu(fileName = "DamageSkill", menuName = "CardBattle/Skills/Damage")]
public class DamageSkill : SkillBase
{
    [Header("ダメージ設定")]
    public int baseDamage = 20;
    public float damageMultiplier = 1.0f;
    public bool ignoreShield = false;

    [Header("追加効果")]
    public bool hasLifeSteal = false;
    [Range(0f, 1f)]
    public float lifeStealPercent = 0.3f;

    public override bool CanExecute(BattleContext context, Character caster)
    {
        return caster.Stats.currentMP >= mpCost;
    }

    public override void Execute(BattleContext context, Character caster, Character target)
    {
        caster.Stats.currentMP -= mpCost;

        // バフ計算
        float totalMultiplier = damageMultiplier;
        if (caster.Stats.attackBuff > 0)
            totalMultiplier *= (1f + caster.Stats.attackBuff);

        int finalDamage = Mathf.RoundToInt(baseDamage * totalMultiplier);

        // ダメージ処理
        if (ignoreShield)
        {
            target.TakeTrueDamage(finalDamage, context);
        }
        else
        {
            target.TakeDamage(finalDamage, context);
        }

        // ライフスティール
        if (hasLifeSteal)
        {
            int healAmount = Mathf.RoundToInt(finalDamage * lifeStealPercent);
            caster.Heal(healAmount, context);
            context.Log($"{caster.Name} がライフスティールで {healAmount} 回復");
        }

        context.Log($"{caster.Name} が {skillName} を発動！ {target.Name} に {finalDamage} ダメージ");
    }
}