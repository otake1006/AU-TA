using UnityEngine;

[CreateAssetMenu(fileName = "WarriorsRingRelic", menuName = "Card Game/Relics/Warrior's Ring")]
public class WarriorsRingRelic : RelicEffect
{
    [Header("Warrior's Ring Settings")]
    public int attackBonus = 2;

    void Awake()
    {
        buffID = 1001;
        buffName = "戦士の指輪";
        description = $"すべての攻撃が+{attackBonus}ダメージを与える";
        flavorText = "古の戦士が身に着けていた力の指輪";
        rarity = RelicRarity.Common;
        category = RelicCategory.Combat;
        isStackable = true;
    }

    public override void OnApply()
    {
        if (target != null)
        {
            // 攻撃力永続ボーナス
            target.ModifyAttack(attackBonus * stackCount);
            GameEvents.OnDebugMessage?.Invoke($"{target.characterName} gains {attackBonus * stackCount} permanent attack from Warrior's Ring");
        }
    }

    public override void OnRemove()
    {
        if (target != null)
        {
            // 攻撃力ボーナス削除
            target.ModifyAttack(-attackBonus * stackCount);
            GameEvents.OnDebugMessage?.Invoke($"{target.characterName} loses {attackBonus * stackCount} attack from Warrior's Ring");
        }
    }

    public override void OnTurnStart()
    {
        // 必要に応じてターン開始時の処理
    }

    public override void OnTurnEnd()
    {
        // 必要に応じてターン終了時の処理
    }

    protected override void OnStackEffect()
    {
        // スタック時に追加の攻撃力ボーナス
        if (target != null)
        {
            target.ModifyAttack(attackBonus);
            GameEvents.OnDebugMessage?.Invoke($"{target.characterName} gains additional {attackBonus} attack from stacking Warrior's Ring");
        }
    }

    public override void OnDamageDealt(int damage, Character target)
    {
        // ダメージを与えた時の追加効果（例：クリティカル率上昇など）
        GameEvents.OnDebugMessage?.Invoke($"Warrior's Ring enhances damage dealt: {damage} -> {damage + (attackBonus * stackCount)}");
    }

    public override int GetEffectValue()
    {
        return attackBonus;
    }

    public override bool CanAcquire(Character character)
    {
        // 戦士系キャラクターのみ獲得可能な条件など
        return character.characterStats.BaseAttack >= 5; // 基本攻撃力5以上
    }
}