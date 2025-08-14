using CardBattle.Core;
using UnityEngine;

[CreateAssetMenu(fileName = "MPCondition", menuName = "CardBattle/Conditions/MP")]
public class MPCondition : ConditionBase
{
    [Header("MPğŒ")]
    public ConditionTarget checkTarget = ConditionTarget.Self;
    public ComparisonOperator comparison = ComparisonOperator.GreaterThan;
    public int mpThreshold = 30;

    public override bool Evaluate(BattleContext context, Character owner)
    {
        Character target = checkTarget == ConditionTarget.Self ? owner :
                         (owner == context.Player ? context.Enemy : context.Player);

        return comparison switch
        {
            ComparisonOperator.GreaterThan => target.Stats.currentMP > mpThreshold,
            ComparisonOperator.LessThan => target.Stats.currentMP < mpThreshold,
            ComparisonOperator.GreaterOrEqual => target.Stats.currentMP >= mpThreshold,
            ComparisonOperator.LessOrEqual => target.Stats.currentMP <= mpThreshold,
            ComparisonOperator.Equal => target.Stats.currentMP == mpThreshold,
            ComparisonOperator.NotEqual => target.Stats.currentMP != mpThreshold,
            _ => false
        };
    }
}