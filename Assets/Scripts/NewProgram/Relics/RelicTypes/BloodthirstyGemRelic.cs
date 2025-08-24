using UnityEngine;

[CreateAssetMenu(fileName = "BloodthirstyGemRelic", menuName = "Card Game/Relics/Bloodthirsty Gem")]
public class BloodthirstyGemRelic : RelicEffect
{
    [Header("Bloodthirsty Gem Settings")]
    public int healPerKill = 5;
    public int manaPerKill = 2;

    void Awake()
    {
        buffID = 1003;
        buffName = "血に飢えた宝石";
        description = $"敵を倒すたびに{healPerKill}HP、{manaPerKill}マナを回復する";
        flavorText = "血の力で持ち主を強化する呪われた宝石";
        rarity = RelicRarity.Rare;
        category = RelicCategory.Combat;
        isStackable = true;
    }

    public override void OnApply()
    {
        if (target != null)
        {
            GameEvents.OnCharacterDeath += OnEnemyKilled;
            GameEvents.OnDebugMessage?.Invoke($"{target.characterName} equipped Bloodthirsty Gem");
        }
    }

    public override void OnRemove()
    {
        GameEvents.OnCharacterDeath -= OnEnemyKilled;
    }

    public override void OnTurnStart()
    {
        // ターン開始時は何もしない
    }

    public override void OnTurnEnd()
    {
        // ターン終了時は何もしない
    }

    void OnEnemyKilled(Character deadCharacter)
    {
        // 自分以外が死んだ場合に回復
        if (target != null && deadCharacter != target)
        {
            int healAmount = healPerKill * stackCount;
            int manaAmount = manaPerKill * stackCount;
            
            // HP回復
            target.characterStats.RestoreHealth(healAmount);
            
            // マナ回復
            target.characterStats.RestoreMana(manaAmount);
            
            GameEvents.OnDebugMessage?.Invoke($"{target.characterName} gains {healAmount}HP and {manaAmount}MP from Bloodthirsty Gem");
            
            // 視覚効果
            GameEvents.OnHealEffect?.Invoke(target, healAmount);
        }
    }

    protected override void OnStackEffect()
    {
        GameEvents.OnDebugMessage?.Invoke($"Bloodthirsty Gem stacked! Now provides {healPerKill * stackCount}HP and {manaPerKill * stackCount}MP per kill");
    }

    public override int GetEffectValue()
    {
        return healPerKill + manaPerKill;
    }

    public override bool CanAcquire(Character character)
    {
        // 誰でも獲得可能だが、すでに血の呪いを持っている場合は獲得不可
        var buffManager = character.GetComponent<TurnBasedBuffManager>();
        return buffManager == null || !buffManager.HasBuff(2001); // 血の呪いID仮定
    }

    public override void OnBattleStart()
    {
        GameEvents.OnDebugMessage?.Invoke($"Bloodthirsty Gem activated for {target.characterName}");
    }

    public override void OnMatchEnd(Character winner)
    {
        if (target == winner && target != null)
        {
            // 勝利時にボーナス回復
            int bonusHeal = healPerKill * stackCount * 2;
            target.characterStats.RestoreHealth(bonusHeal);
            GameEvents.OnDebugMessage?.Invoke($"{target.characterName} gains {bonusHeal} bonus HP from victory with Bloodthirsty Gem");
        }
    }
}