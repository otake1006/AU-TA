using CardBattle.Core;
using UnityEngine;

public abstract class SkillBase : ScriptableObject
{
    [Header("基本情報")]
    public string skillId;
    public string skillName;
    [TextArea(2, 4)]
    public string description;
    public Sprite icon;

    [Header("コスト")]
    public int mpCost = 10;

    [Header("ターゲット")]
    public TargetType targetType = TargetType.Enemy;

    [Header("タイミング")]
    public SkillTiming timing = SkillTiming.Instant;

    public abstract bool CanExecute(BattleContext context, Character caster);
    public abstract void Execute(BattleContext context, Character caster, Character target);
}