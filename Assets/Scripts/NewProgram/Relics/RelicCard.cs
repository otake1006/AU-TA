using UnityEngine;

[CreateAssetMenu(fileName = "RelicCard", menuName = "Card Game/Relic Card")]
public class RelicCard : ConditionalSkillCard
{
    [Header("Relic Card Settings")]
    public RelicEffect relicEffect;
    public bool consumeOnUse = true;
    public string relicDescription;

    public override bool CanUse(Character caster, Character target = null)
    {
        if (!base.CanUse(caster, target)) return false;

        // すでに同じレリックを持っているかチェック
        if (relicEffect != null && !relicEffect.isStackable)
        {
            var buffManager = caster.GetComponent<TurnBasedBuffManager>();
            if (buffManager != null && buffManager.HasBuff(relicEffect.buffID))
            {
                GameEvents.OnDebugMessage?.Invoke($"Already has relic: {relicEffect.buffName}");
                return false;
            }
        }

        // レリック獲得条件チェック
        return relicEffect?.CanAcquire(caster) ?? false;
    }

    public virtual void Use(Character caster, Character target = null)
    {
        if (!CanUse(caster, target)) return;

        GameEvents.OnDebugMessage?.Invoke($"{caster.characterName} uses relic card: {cardName}");

        // レリック効果を適用
        if (relicEffect != null)
        {
            var buffManager = caster.GetComponent<TurnBasedBuffManager>();
            if (buffManager != null)
            {
                // レリックを永続バフとして適用
                buffManager.ApplyBuff(relicEffect);
                
                // レリック獲得イベント発火
                relicEffect.OnRelicAcquired();
                GameEvents.OnRelicAcquired?.Invoke(caster, relicEffect);
            }
        }

        // カードが消費される場合の処理
        if (consumeOnUse)
        {
            GameEvents.OnCardConsumed?.Invoke(this, caster);
        }
    }


    // レリックカードの価値を評価（AI用）
    public virtual int EvaluateRelicValue(Character character)
    {
        int baseValue = 50; // レリックの基本価値
        
        if (relicEffect != null)
        {
            switch (relicEffect.rarity)
            {
                case RelicRarity.Common: baseValue += 10; break;
                case RelicRarity.Uncommon: baseValue += 25; break;
                case RelicRarity.Rare: baseValue += 50; break;
                case RelicRarity.Epic: baseValue += 100; break;
                case RelicRarity.Legendary: baseValue += 200; break;
            }

            // スタック可能で既に持っている場合
            if (relicEffect.isStackable)
            {
                var buffManager = character.GetComponent<TurnBasedBuffManager>();
                var existingBuff = buffManager?.GetBuff(relicEffect.buffID);
                if (existingBuff != null)
                {
                    baseValue += existingBuff.stackCount * 20;
                }
            }
        }

        return baseValue;
    }
}