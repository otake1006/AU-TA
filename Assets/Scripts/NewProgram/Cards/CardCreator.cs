using UnityEditor;
using UnityEngine;

public class CardCreator : MonoBehaviour
{
    [ContextMenu("Create Sample Cards")]
    public void CreateSampleCards()
    {
        CreateBasicAttackCard();
        CreateHealCard();
        CreateConditionalCards();
    }

    void CreateBasicAttackCard()
    {
        var card = ScriptableObject.CreateInstance<SkillCard>();
        card.cardID = 1;
        card.cardName = "Strike";
        card.description = "Deal 12 damage to target enemy";
        card.manaCost = 2;
        card.animationID = 1;
        card.rarity = CardRarity.Common;

        card.effects = new TurnBasedSkillEffect[]
        {
            new TurnBasedSkillEffect
            {
                effectType = SkillEffectType.Damage,
                value = 12,
                targetType = TargetType.Enemy
            }
        };

#if UNITY_EDITOR
        AssetDatabase.CreateAsset(card, "Assets/CardBattleGame/Data/Cards/Basic/Strike.asset");
        AssetDatabase.SaveAssets();
#endif
    }

    void CreateHealCard()
    {
        var card = ScriptableObject.CreateInstance<SkillCard>();
        card.cardID = 2;
        card.cardName = "Heal";
        card.description = "Restore 18 HP";
        card.manaCost = 2;
        card.animationID = 2;
        card.rarity = CardRarity.Common;

        card.effects = new TurnBasedSkillEffect[]
        {
            new TurnBasedSkillEffect
            {
                effectType = SkillEffectType.Heal,
                value = 18,
                targetType = TargetType.Self
            }
        };

#if UNITY_EDITOR
        AssetDatabase.CreateAsset(card, "Assets/CardBattleGame/Data/Cards/Basic/Heal.asset");
        AssetDatabase.SaveAssets();
#endif
    }

    void CreateConditionalCards()
    {
        CreateDesperationStrike();
        CreateHealingLight();
        CreateVengeanceBlast();
        CreatePerfectDefense();
        CreateFinisher();
    }

    void CreateDesperationStrike()
    {
        var card = ScriptableObject.CreateInstance<ConditionalSkillCard>();
        card.cardID = 101;
        card.cardName = "Desperation Strike";
        card.description = "Deal 15 damage. Enhanced when HP ≤ 10%";
        card.manaCost = 2;
        card.animationID = 1;
        card.rarity = CardRarity.Uncommon;

        // 使用条件：自分のHPが30%以下
        card.usageConditions[0] = new SkillCondition
        {
            conditionType = ConditionType.HealthPercentage,
            target = ConditionTarget.Self,
            comparisonOperator = ComparisonOperator.LessThanOrEqual,
            value = 30
        };

        // 基本効果
        card.effects = new TurnBasedSkillEffect[]
        {
            new TurnBasedSkillEffect
            {
                effectType = SkillEffectType.Damage,
                value = 15,
                targetType = TargetType.Enemy
            }
        };

        // 強化効果：HPが10%以下の場合、追加でダメージ+15
        card.hasEnhancedVersion = true;
        card.usageConditions[1] = new SkillCondition
        {
            conditionType = ConditionType.HealthPercentage,
            target = ConditionTarget.Self,
            comparisonOperator = ComparisonOperator.LessThanOrEqual,
            value = 10
        };

        card.enhancedEffects = new TurnBasedSkillEffect[]
        {
            new TurnBasedSkillEffect
            {
                effectType = SkillEffectType.Damage,
                value = 15,
                targetType = TargetType.Enemy
            }
        };

#if UNITY_EDITOR
        AssetDatabase.CreateAsset(card, "Assets/CardBattleGame/Data/Cards/Conditional/DesperationStrike.asset");
        AssetDatabase.SaveAssets();
#endif
    }

    void CreateHealingLight()
    {
        var card = ScriptableObject.CreateInstance<ConditionalSkillCard>();
        card.cardID = 102;
        card.cardName = "Healing Light";
        card.description = "Heal 25 HP. Enhanced when enemy has 2+ buffs";
        card.manaCost = 3;
        card.animationID = 2;
        card.rarity = CardRarity.Uncommon;

        // 使用条件：相手の攻撃力が15以上
        card.usageConditions[0] = new SkillCondition
        {
            conditionType = ConditionType.Attack,
            target = ConditionTarget.Enemy,
            comparisonOperator = ComparisonOperator.GreaterThanOrEqual,
            value = 15
        };

        // 基本効果
        card.effects = new TurnBasedSkillEffect[]
        {
            new TurnBasedSkillEffect
            {
                effectType = SkillEffectType.Heal,
                value = 25,
                targetType = TargetType.Self
            }
        };

        // 強化効果：相手のバフが2個以上ある場合、デバフ除去も追加
        card.hasEnhancedVersion = true;
        card.usageConditions[1] = new SkillCondition
        {
            conditionType = ConditionType.BuffCount,
            target = ConditionTarget.Enemy,
            comparisonOperator = ComparisonOperator.GreaterThanOrEqual,
            value = 2
        };

        card.enhancedEffects = new TurnBasedSkillEffect[]
        {
            new TurnBasedSkillEffect
            {
                effectType = SkillEffectType.RemoveBuff,
                targetType = TargetType.Self
            }
        };

#if UNITY_EDITOR
        AssetDatabase.CreateAsset(card, "Assets/CardBattleGame/Data/Cards/Conditional/HealingLight.asset");
        AssetDatabase.SaveAssets();
#endif
    }

    void CreateVengeanceBlast()
    {
        var card = ScriptableObject.CreateInstance<ConditionalSkillCard>();
        card.cardID = 103;
        card.cardName = "Vengeance Blast";
        card.description = "Deal 20 damage to high-HP enemies";
        card.manaCost = 4;
        card.animationID = 3;
        card.rarity = CardRarity.Rare;

        // 使用条件：相手のHPが70%以上
        card.usageConditions[0] = new SkillCondition
        {
            conditionType = ConditionType.HealthPercentage,
            target = ConditionTarget.Enemy,
            comparisonOperator = ComparisonOperator.GreaterThanOrEqual,
            value = 70
        };

        // 基本効果
        card.effects = new TurnBasedSkillEffect[]
        {
            new TurnBasedSkillEffect
            {
                effectType = SkillEffectType.Damage,
                value = 20,
                targetType = TargetType.Enemy
            }
        };

        // 強化効果：相手のHPが90%以上の場合、毒も付与
        card.hasEnhancedVersion = true;
        card.usageConditions[1] = new SkillCondition
        {
            conditionType = ConditionType.HealthPercentage,
            target = ConditionTarget.Enemy,
            comparisonOperator = ComparisonOperator.GreaterThanOrEqual,
            value = 90
        };

        var poisonBuff = ScriptableObject.CreateInstance<TurnBasedPoisonDebuff>();
        poisonBuff.buffID = 1001;
        poisonBuff.buffName = "Vengeance Poison";
        poisonBuff.buffType = BuffType.Debuff;
        poisonBuff.duration = 3;
        poisonBuff.damagePerTurn = 7;
        poisonBuff.maxStacks = 2;

        card.enhancedEffects = new TurnBasedSkillEffect[]
        {
            new TurnBasedSkillEffect
            {
                effectType = SkillEffectType.ApplyBuff,
                targetType = TargetType.Enemy,
                duration = 3,
                associatedBuff = poisonBuff
            }
        };

#if UNITY_EDITOR
        AssetDatabase.CreateAsset(card, "Assets/CardBattleGame/Data/Cards/Conditional/VengeanceBlast.asset");
        AssetDatabase.SaveAssets();
#endif
    }

    void CreatePerfectDefense()
    {
        var card = ScriptableObject.CreateInstance<ConditionalSkillCard>();
        card.cardID = 104;
        card.cardName = "Perfect Defense";
        card.description = "Gain 15 shield. Enhanced with high mana + enemy debuffs";
        card.manaCost = 2;
        card.animationID = 4;
        card.rarity = CardRarity.Rare;

        // 使用条件：自分のマナが80%以上
        card.usageConditions[0] = new SkillCondition
        {
            conditionType = ConditionType.ManaPercentage,
            target = ConditionTarget.Self,
            comparisonOperator = ComparisonOperator.GreaterThanOrEqual,
            value = 80
        };

        // 使用条件2：相手にデバフが1個以上
        card.usageConditions[1] = new SkillCondition
        {
            conditionType = ConditionType.DebuffCount,
            target = ConditionTarget.Enemy,
            comparisonOperator = ComparisonOperator.GreaterThanOrEqual,
            value = 1
        };

        // 基本効果
        card.effects = new TurnBasedSkillEffect[]
        {
            new TurnBasedSkillEffect
            {
                effectType = SkillEffectType.Shield,
                value = 15,
                targetType = TargetType.Self
            }
        };

        // 強化効果：条件を両方満たす場合、防御力も上昇
        card.hasEnhancedVersion = true;

        var defenseBuff = ScriptableObject.CreateInstance<TurnBasedDefenseBoostBuff>();
        defenseBuff.buffID = 2003;
        defenseBuff.buffName = "Iron Wall";
        defenseBuff.buffType = BuffType.Buff;
        defenseBuff.duration = 3;
        defenseBuff.defenseIncrease = 5;
        defenseBuff.maxStacks = 2;

        card.enhancedEffects = new TurnBasedSkillEffect[]
        {
            new TurnBasedSkillEffect
            {
                effectType = SkillEffectType.ApplyBuff,
                targetType = TargetType.Self,
                duration = 3,
                associatedBuff = defenseBuff
            }
        };

#if UNITY_EDITOR
        AssetDatabase.CreateAsset(card, "Assets/CardBattleGame/Data/Cards/Conditional/PerfectDefense.asset");
        AssetDatabase.SaveAssets();
#endif
    }

    void CreateFinisher()
    {
        var card = ScriptableObject.CreateInstance<ConditionalSkillCard>();
        card.cardID = 105;
        card.cardName = "Finisher";
        card.description = "Devastating attack against low-HP enemies";
        card.manaCost = 5;
        card.animationID = 5;
        card.rarity = CardRarity.Epic;

        // 使用条件：相手のHPが40%以下
        card.usageConditions[0] = new SkillCondition
        {
            conditionType = ConditionType.HealthPercentage,
            target = ConditionTarget.Enemy,
            comparisonOperator = ComparisonOperator.LessThanOrEqual,
            value = 40
        };

        // 基本効果
        card.effects = new TurnBasedSkillEffect[]
        {
            new TurnBasedSkillEffect
            {
                effectType = SkillEffectType.Damage,
                value = 30,
                targetType = TargetType.Enemy
            }
        };

        // 強化効果：相手のHPが15%以下の場合、即死級ダメージ
        card.hasEnhancedVersion = true;
        card.usageConditions[1] = new SkillCondition
        {
            conditionType = ConditionType.HealthPercentage,
            target = ConditionTarget.Enemy,
            comparisonOperator = ComparisonOperator.LessThanOrEqual,
            value = 15
        };

        card.enhancedEffects = new TurnBasedSkillEffect[]
        {
            new TurnBasedSkillEffect
            {
                effectType = SkillEffectType.Damage,
                value = 50,
                targetType = TargetType.Enemy
            }
        };

#if UNITY_EDITOR
        AssetDatabase.CreateAsset(card, "Assets/CardBattleGame/Data/Cards/Conditional/Finisher.asset");
        AssetDatabase.SaveAssets();
#endif
    }
}