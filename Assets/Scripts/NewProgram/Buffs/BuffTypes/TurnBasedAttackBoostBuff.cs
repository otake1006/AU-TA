using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Boost Buff", menuName = "Card Game/Buffs/Attack Boost")]
public class TurnBasedAttackBoostBuff : TurnBasedBuffEffect
{
    [Header("Attack Boost Settings")]
    public int attackIncrease = 5;

    public override void OnApply()
    {
        target.ModifyAttack(attackIncrease * stackCount);
        GameEvents.OnDebugMessage?.Invoke($"{target.characterName} gains {attackIncrease * stackCount} attack for {remainingTurns} turns");
    }

    public override void OnRemove()
    {
        target.ModifyAttack(-attackIncrease * stackCount);
        GameEvents.OnDebugMessage?.Invoke($"{target.characterName}'s attack buff expires");
    }

    public override void OnTurnStart()
    {
        GameEvents.OnDebugMessage?.Invoke($"{target.characterName}'s attack buff continues ({remainingTurns - 1} turns remaining)");
    }

    public override void OnTurnEnd() { }

    protected override void OnStackEffect()
    {
        // 新しいスタックの効果を適用
        target.ModifyAttack(attackIncrease);
    }

    public override int GetEffectValue()
    {
        return attackIncrease;
    }
}