using CardBattle.Core;
using UnityEngine;

public abstract class SkillBase : ScriptableObject
{
    [Header("��{���")]
    public string skillId;
    public string skillName;
    [TextArea(2, 4)]
    public string description;
    public Sprite icon;

    [Header("�R�X�g")]
    public int mpCost = 10;

    [Header("�^�[�Q�b�g")]
    public TargetType targetType = TargetType.Enemy;

    [Header("�^�C�~���O")]
    public SkillTiming timing = SkillTiming.Instant;

    public abstract bool CanExecute(BattleContext context, Character caster);
    public abstract void Execute(BattleContext context, Character caster, Character target);
}