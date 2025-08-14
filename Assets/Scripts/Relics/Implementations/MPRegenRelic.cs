using UnityEngine;

[CreateAssetMenu(fileName = "MPRegenRelic", menuName = "CardBattle/Relics/MPRegen")]
public class MPRegenRelic : RelicBase
{
    [Header("MP‰ñ•œİ’è")]
    public int mpRestoreAmount = 5;

    public override void OnTrigger(BattleContext context, Character owner, TriggerEvent evt)
    {
        if (!ConsumeUse()) return;

        owner.RestoreMP(mpRestoreAmount, context);
        context.Log($"{relicName} ”­“®I MP +{mpRestoreAmount}");
    }
}