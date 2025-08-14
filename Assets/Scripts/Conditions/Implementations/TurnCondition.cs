using CardBattle.Core;
using UnityEngine;

[CreateAssetMenu(fileName = "TurnCondition", menuName = "CardBattle/Conditions/Turn")]
public class TurnCondition : ConditionBase
{
    [Header("ƒ^[ƒ“ðŒ")]
    public ComparisonOperator comparison = ComparisonOperator.GreaterOrEqual;
    public int turnNumber = 5;
    public bool checkRoundInstead = false;

    public override bool Evaluate(BattleContext context, Character owner)
    {
        int value = checkRoundInstead ? context.CurrentRound : context.CurrentTurn;

        return comparison switch
        {
            ComparisonOperator.GreaterThan => value > turnNumber,
            ComparisonOperator.LessThan => value < turnNumber,
            ComparisonOperator.GreaterOrEqual => value >= turnNumber,
            ComparisonOperator.LessOrEqual => value <= turnNumber,
            ComparisonOperator.Equal => value == turnNumber,
            ComparisonOperator.NotEqual => value != turnNumber,
            _ => false
        };
    }
}