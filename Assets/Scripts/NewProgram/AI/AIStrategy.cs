using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class AIStrategy
{
    protected EnemyAI ai;

    public virtual void Initialize(EnemyAI enemyAI)
    {
        ai = enemyAI;
    }

    public abstract ConditionalSkillCard SelectCard(List<ConditionalSkillCard> usableCards, Character caster, Character target);
    public abstract float EvaluateCard(ConditionalSkillCard card, Character caster, Character target);
}

// ランダム戦略（簡単）
public class RandomStrategy : AIStrategy
{
    public override ConditionalSkillCard SelectCard(List<ConditionalSkillCard> usableCards, Character caster, Character target)
    {
        return usableCards.GetRandomElement();
    }

    public override float EvaluateCard(ConditionalSkillCard card, Character caster, Character target)
    {
        return Random.Range(0f, 1f);
    }
}

// バランス戦略（普通）
public class BalancedStrategy : AIStrategy
{
    public override ConditionalSkillCard SelectCard(List<ConditionalSkillCard> usableCards, Character caster, Character target)
    {
        ConditionalSkillCard bestCard = null;
        float highestScore = float.MinValue;

        foreach (var card in usableCards)
        {
            float score = EvaluateCard(card, caster, target);
            if (score > highestScore)
            {
                highestScore = score;
                bestCard = card;
            }
        }

        return bestCard;
    }

    public override float EvaluateCard(ConditionalSkillCard card, Character caster, Character target)
    {
        float score = 0f;

        // 強化版ボーナス
        if (card.IsEnhanced(caster, target))
        {
            score += 50f;
        }

        // HP状況に応じた評価
        float casterHPPercent = (float)caster.CurrentHealth / caster.maxHealth;
        float targetHPPercent = (float)target.CurrentHealth / target.maxHealth;

        foreach (var effect in card.GetEffectsToUse(caster, target))
        {
            switch (effect.effectType)
            {
                case SkillEffectType.Damage:
                    score += effect.value;
                    // 相手のHPが低いほど高評価
                    if (targetHPPercent < 0.3f) score += 20f;
                    break;

                case SkillEffectType.Heal:
                    // 自分のHPが低いほど高評価
                    score += effect.value * (1f - casterHPPercent) * 2f;
                    break;

                case SkillEffectType.Shield:
                    // HPが低く、シールドがない場合高評価
                    if (casterHPPercent < 0.5f && caster.CurrentShield == 0)
                        score += effect.value * 1.5f;
                    else
                        score += effect.value * 0.5f;
                    break;

                case SkillEffectType.ApplyBuff:
                    score += 15f;
                    break;
            }
        }

        // コスト効率
        score = score / Mathf.Max(1, card.manaCost);

        return score;
    }
}

// 攻撃的戦略（難しい）
public class AggressiveStrategy : BalancedStrategy
{
    public override float EvaluateCard(ConditionalSkillCard card, Character caster, Character target)
    {
        float baseScore = base.EvaluateCard(card, caster, target);

        // ダメージ効果を優先
        foreach (var effect in card.effects)
        {
            if (effect.effectType == SkillEffectType.Damage)
            {
                baseScore *= 1.5f;
            }
        }

        return baseScore;
    }
}

// 最適戦略（エキスパート）
public class OptimalStrategy : BalancedStrategy
{
    public override float EvaluateCard(ConditionalSkillCard card, Character caster, Character target)
    {
        float score = base.EvaluateCard(card, caster, target);

        // より複雑な評価ロジック
        score += EvaluateGameState(caster, target);
        score += EvaluateCardSynergy(card, caster);

        return score;
    }

    float EvaluateGameState(Character caster, Character target)
    {
        float score = 0f;

        // HP差による評価
        float hpDifference = caster.CurrentHealth - target.CurrentHealth;
        score += hpDifference * 0.1f;

        // バフ数による評価
        var casterBuffs = caster.GetComponent<TurnBasedBuffManager>()?.GetActiveBuffs().Count ?? 0;
        var targetBuffs = target.GetComponent<TurnBasedBuffManager>()?.GetActiveBuffs().Count ?? 0;
        score += (casterBuffs - targetBuffs) * 5f;

        return score;
    }

    float EvaluateCardSynergy(ConditionalSkillCard card, Character caster)
    {
        // カードシナジーの評価（簡略化）
        return 0f;
    }
}
