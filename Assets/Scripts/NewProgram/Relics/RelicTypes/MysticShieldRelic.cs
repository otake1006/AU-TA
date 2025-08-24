using UnityEngine;

[CreateAssetMenu(fileName = "MysticShieldRelic", menuName = "Card Game/Relics/Mystic Shield")]
public class MysticShieldRelic : RelicEffect
{
    [Header("Mystic Shield Settings")]
    public int shieldAmount = 3;
    public int maxShieldStacks = 5;
    private int currentShield = 0;

    void Awake()
    {
        buffID = 1002;
        buffName = "神秘の盾";
        description = $"ラウンド開始時に{shieldAmount}のシールドを獲得する（最大{maxShieldStacks}）";
        flavorText = "魔法の力で身を守る古代の盾";
        rarity = RelicRarity.Uncommon;
        category = RelicCategory.Defense;
        isStackable = false;
    }

    public override void OnApply()
    {
        currentShield = 0;
        GameEvents.OnDebugMessage?.Invoke($"{target.characterName} equipped Mystic Shield");
    }

    public override void OnRemove()
    {
        // シールドをすべて削除
        if (target != null && currentShield > 0)
        {
            target.ModifyDefense(-currentShield);
            currentShield = 0;
        }
    }

    public override void OnRoundStart()
    {
        if (target != null)
        {
            // 既存のシールドを削除
            if (currentShield > 0)
            {
                target.ModifyDefense(-currentShield);
            }

            // 新しいシールドを適用
            currentShield = Mathf.Min(shieldAmount, maxShieldStacks);
            target.ModifyDefense(currentShield);
            
            GameEvents.OnDebugMessage?.Invoke($"{target.characterName} gains {currentShield} shield from Mystic Shield");
        }
    }

    public override void OnTurnStart()
    {
        // ターン開始時は何もしない
    }

    public override void OnTurnEnd()
    {
        // ターン終了時は何もしない
    }

    public override void OnDamageTaken(int damage, DamageType damageType)
    {
        if (currentShield > 0 && damage > 0)
        {
            int shieldReduction = Mathf.Min(currentShield, damage);
            currentShield -= shieldReduction;
            
            // 防御力からシールド分を削除
            target.ModifyDefense(-shieldReduction);
            
            GameEvents.OnDebugMessage?.Invoke($"Mystic Shield absorbed {shieldReduction} damage, {currentShield} shield remaining");
        }
    }

    public override int GetEffectValue()
    {
        return currentShield;
    }

    public override string GetDetailedDescription()
    {
        string baseDesc = base.GetDetailedDescription();
        if (currentShield > 0)
        {
            baseDesc += $"\n<color=cyan>Current Shield: {currentShield}</color>";
        }
        return baseDesc;
    }

    public override bool CanAcquire(Character character)
    {
        // 魔法系キャラクターのみ獲得可能
        //return character.BaseMana >= 10;
        return true;
    }
}