using CardBattle.Core;
using UnityEngine;

public abstract class ConditionBase : ScriptableObject
{
    [Header("Šî–{î•ñ")]
    public string conditionId;
    public string conditionName;
    [TextArea(2, 3)]
    public string description;

    public abstract bool Evaluate(BattleContext context, Character owner);
}

[CreateAssetMenu(fileName = "HPCondition", menuName = "CardBattle/Conditions/HP")]
public class HPCondition : ConditionBase
{
    [Header("HPðŒ")]
    public ConditionTarget checkTarget = ConditionTarget.Enemy;
    public ComparisonOperator comparison = ComparisonOperator.LessThan;
    [Range(0f, 100f)]
    public float hpPercentThreshold = 30f;

    public override bool Evaluate(BattleContext context, Character owner)
    {
        Character target = GetTarget(context, owner, checkTarget);
        if (target == null) return false;

        float hpPercent = (target.Stats.currentHP / (float)target.Stats.maxHP) * 100f;

        return comparison switch
        {
            ComparisonOperator.GreaterThan => hpPercent > hpPercentThreshold,
            ComparisonOperator.LessThan => hpPercent < hpPercentThreshold,
            ComparisonOperator.GreaterOrEqual => hpPercent >= hpPercentThreshold,
            ComparisonOperator.LessOrEqual => hpPercent <= hpPercentThreshold,
            ComparisonOperator.Equal => Mathf.Abs(hpPercent - hpPercentThreshold) < 0.01f,
            ComparisonOperator.NotEqual => Mathf.Abs(hpPercent - hpPercentThreshold) >= 0.01f,
            _ => false
        };
    }

    private Character GetTarget(BattleContext context, Character owner, ConditionTarget target)
    {
        return target switch
        {
            ConditionTarget.Self => owner,
            ConditionTarget.Enemy => owner == context.Player ? context.Enemy : context.Player,
            _ => null
        };
    }
}