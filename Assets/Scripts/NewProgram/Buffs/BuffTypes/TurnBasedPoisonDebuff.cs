public class TurnBasedPoisonDebuff : TurnBasedBuffEffect
{
    public int damagePerTurn = 5;

    public override void OnApply()
    {
        triggerTiming = BuffTriggerTiming.TurnStart;
        GameEvents.OnDebugMessage?.Invoke($"{target.characterName} is poisoned for {remainingTurns} turns");
    }

    public override void OnRemove()
    {
        GameEvents.OnDebugMessage?.Invoke($"{target.characterName}'s poison has worn off");
    }

    public override void OnTurnStart()
    {
        int totalDamage = damagePerTurn * stackCount;
        target.TakeDamage(totalDamage, DamageType.Poison);
        GameEvents.OnDebugMessage?.Invoke($"{target.characterName} takes {totalDamage} poison damage");
    }

    public override void OnTurnEnd() { }
}