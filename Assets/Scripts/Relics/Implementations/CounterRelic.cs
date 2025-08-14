
using UnityEngine;

[CreateAssetMenu(fileName = "CounterRelic", menuName = "CardBattle/Relics/Counter")]
public class CounterRelic : RelicBase
{
    [Header("カウンター設定")]
    [Range(0f, 2f)]
    public float counterDamageMultiplier = 0.5f;
    public int fixedCounterDamage = 0;

    public override void OnTrigger(BattleContext context, Character owner, TriggerEvent evt)
    {
        if (!ConsumeUse()) return;
        if (!evt.Data.ContainsKey("Damage")) return;

        int damage = (int)evt.Data["Damage"];
        int counterDamage = fixedCounterDamage > 0 ?
            fixedCounterDamage :
            Mathf.RoundToInt(damage * counterDamageMultiplier);

        Character attacker = owner == context.Player ? context.Enemy : context.Player;
        attacker.TakeDamage(counterDamage, context);

        context.Log($"{relicName} 発動！ {counterDamage} の反撃ダメージ");
    }
}