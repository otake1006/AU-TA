using UnityEngine;

[CreateAssetMenu(fileName = "DamageSkill", menuName = "CardBattle/Skills/Damage")]
public class DamageSkill : SkillBase
{
    [Header("�_���[�W�ݒ�")]
    public int baseDamage = 20;
    public float damageMultiplier = 1.0f;
    public bool ignoreShield = false;

    [Header("�ǉ�����")]
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

        // �o�t�v�Z
        float totalMultiplier = damageMultiplier;
        if (caster.Stats.attackBuff > 0)
            totalMultiplier *= (1f + caster.Stats.attackBuff);

        int finalDamage = Mathf.RoundToInt(baseDamage * totalMultiplier);

        // �_���[�W����
        if (ignoreShield)
        {
            target.TakeTrueDamage(finalDamage, context);
        }
        else
        {
            target.TakeDamage(finalDamage, context);
        }

        // ���C�t�X�e�B�[��
        if (hasLifeSteal)
        {
            int healAmount = Mathf.RoundToInt(finalDamage * lifeStealPercent);
            caster.Heal(healAmount, context);
            context.Log($"{caster.Name} �����C�t�X�e�B�[���� {healAmount} ��");
        }

        context.Log($"{caster.Name} �� {skillName} �𔭓��I {target.Name} �� {finalDamage} �_���[�W");
    }
}