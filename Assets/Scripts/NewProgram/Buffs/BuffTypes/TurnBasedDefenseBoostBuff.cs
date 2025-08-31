public class TurnBasedDefenseBoostBuff : TurnBasedBuffEffect
{
    public int defenseIncrease = 3;

    public override void OnApply()
    {
        target.ModifyDefense(defenseIncrease * stackCount);
        GameEvents.OnDebugMessage?.Invoke($"{target.characterName} gains {defenseIncrease * stackCount} defense");
    }

    public override void OnRemove()
    {
        target.ModifyDefense(-defenseIncrease * stackCount);
        GameEvents.OnDebugMessage?.Invoke($"{target.characterName}'s defense buff expires");
    }

    public override void OnTurnStart()
    {
        GameEvents.OnDebugMessage?.Invoke($"{target.characterName}'s defense buff continues ({remainingTurns - 1} turns left)");
    }

    public override void OnTurnEnd() { }

    public override void OnStack()
    {
        int previousStacks = stackCount;
        base.OnStack();
        int newStacks = stackCount - previousStacks;
        if (newStacks > 0)
        {
            target.ModifyDefense(defenseIncrease * newStacks);
        }
    }
}