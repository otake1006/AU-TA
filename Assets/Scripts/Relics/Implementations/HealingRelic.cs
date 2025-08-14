using UnityEngine;

[CreateAssetMenu(fileName = "HealingRelic", menuName = "CardBattle/Relics/Healing")]
public class HealingRelic : RelicBase
{
    [Header("‰ñ•œİ’è")]
    public int healAmount = 5;
    public bool healPercentage = false;
    [Range(0f, 0.5f)]
    public float healPercent = 0.1f;

    public override void OnTrigger(BattleContext context, Character owner, TriggerEvent evt)
    {
        if (!ConsumeUse()) return;

        int amount = healPercentage ?
            Mathf.RoundToInt(owner.Stats.maxHP * healPercent) :
            healAmount;

        owner.Heal(amount, context);
        context.Log($"{owner.Name} ‚Ì {relicName} ‚ª”­“®I {amount} ‰ñ•œ");
    }
}