using UnityEngine;

[CreateAssetMenu(fileName = "AlwaysCondition", menuName = "CardBattle/Conditions/Always")]
public class AlwaysCondition : ConditionBase
{
    public override bool Evaluate(BattleContext context, Character owner)
    {
        return true;
    }
}