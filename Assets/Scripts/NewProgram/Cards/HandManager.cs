using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HandManager : MonoBehaviour
{
    private CardManager cardManager;
    private Character owner;

    [Header("Hand Settings")]
    public int maxHandSize = 7;
    public bool autoDiscard = true;

    // 手札情報
    private List<ConditionalSkillCard> hand = new List<ConditionalSkillCard>();
    private Dictionary<ConditionalSkillCard, float> cardPriorities = new Dictionary<ConditionalSkillCard, float>();

    public List<ConditionalSkillCard> Hand => new List<ConditionalSkillCard>(hand);
    public int HandSize => hand.Count;
    public bool IsHandFull => hand.Count >= maxHandSize;

    public void Initialize(Character character, CardManager manager)
    {
        owner = character;
        cardManager = manager;
    }

    public bool AddCard(ConditionalSkillCard card)
    {
        if (IsHandFull)
        {
            if (autoDiscard)
            {
                DiscardLowestPriorityCard();
            }
            else
            {
                return false;
            }
        }

        hand.Add(card);
        UpdateCardPriority(card);
        GameEvents.OnCardDrawn?.Invoke(owner, card);
        return true;
    }

    public bool RemoveCard(ConditionalSkillCard card)
    {
        bool removed = hand.Remove(card);
        if (removed && cardPriorities.ContainsKey(card))
        {
            cardPriorities.Remove(card);
        }
        return removed;
    }

    public List<ConditionalSkillCard> GetPlayableCards(Character target)
    {
        return hand.Where(card => card.CanUse(owner, target)).ToList();
    }

    public ConditionalSkillCard GetBestCard(Character target)
    {
        var playableCards = GetPlayableCards(target);
        if (playableCards.Count == 0) return null;

        // 優先度が最も高いカードを選択
        ConditionalSkillCard bestCard = null;
        float highestPriority = float.MinValue;

        foreach (var card in playableCards)
        {
            float priority = CalculateCardPriority(card, target);
            if (priority > highestPriority)
            {
                highestPriority = priority;
                bestCard = card;
            }
        }

        return bestCard;
    }

    void UpdateCardPriority(ConditionalSkillCard card)
    {
        float priority = CalculateBasePriority(card);
        cardPriorities[card] = priority;
    }

    float CalculateBasePriority(ConditionalSkillCard card)
    {
        float priority = 0f;

        // レアリティによる基本優先度
        switch (card.rarity)
        {
            case CardRarity.Common: priority += 1f; break;
            case CardRarity.Uncommon: priority += 2f; break;
            case CardRarity.Rare: priority += 3f; break;
            case CardRarity.Epic: priority += 4f; break;
            case CardRarity.Legendary: priority += 5f; break;
        }

        // マナコストによる調整（低コストは優先度アップ）
        priority += (10f - card.manaCost) * 0.1f;

        // 効果による優先度
        foreach (var effect in card.effects)
        {
            switch (effect.effectType)
            {
                case SkillEffectType.Damage:
                    priority += effect.value * 0.1f;
                    break;
                case SkillEffectType.Heal:
                    priority += effect.value * 0.08f;
                    break;
                case SkillEffectType.ApplyBuff:
                    priority += 1f;
                    break;
            }
        }

        return priority;
    }

    float CalculateCardPriority(ConditionalSkillCard card, Character target)
    {
        float basePriority = cardPriorities.GetValueOrDefault(card, 0f);
        float situationalPriority = 0f;

        // 状況に応じた優先度調整
        float ownerHPPercent = (float)owner.CurrentHealth / owner.maxHealth;
        float targetHPPercent = (float)target.CurrentHealth / target.maxHealth;

        foreach (var effect in card.effects)
        {
            switch (effect.effectType)
            {
                case SkillEffectType.Damage:
                    // 相手のHPが低いほど攻撃優先度アップ
                    if (targetHPPercent < 0.3f)
                        situationalPriority += 3f;
                    break;

                case SkillEffectType.Heal:
                    // 自分のHPが低いほど回復優先度アップ
                    if (ownerHPPercent < 0.4f)
                        situationalPriority += 4f;
                    break;

                case SkillEffectType.Shield:
                    // HPが低く、シールドがない場合優先度アップ
                    if (ownerHPPercent < 0.5f && owner.CurrentShield == 0)
                        situationalPriority += 2f;
                    break;
            }
        }

        // 強化版が使える場合は大幅優先度アップ
        if (card.IsEnhanced(owner, target))
        {
            situationalPriority += 5f;
        }

        return basePriority + situationalPriority;
    }

    void DiscardLowestPriorityCard()
    {
        if (hand.Count == 0) return;

        ConditionalSkillCard lowestCard = null;
        float lowestPriority = float.MaxValue;

        foreach (var card in hand)
        {
            float priority = cardPriorities.GetValueOrDefault(card, 0f);
            if (priority < lowestPriority)
            {
                lowestPriority = priority;
                lowestCard = card;
            }
        }

        if (lowestCard != null)
        {
            RemoveCard(lowestCard);
            GameEvents.OnDebugMessage?.Invoke($"{owner.characterName} discarded {lowestCard.cardName}");
        }
    }

    public void ClearHand()
    {
        hand.Clear();
        cardPriorities.Clear();
    }

    // 統計情報
    public Dictionary<SkillEffectType, int> GetEffectTypeCount()
    {
        var counts = new Dictionary<SkillEffectType, int>();

        foreach (var card in hand)
        {
            foreach (var effect in card.effects)
            {
                if (counts.ContainsKey(effect.effectType))
                    counts[effect.effectType]++;
                else
                    counts[effect.effectType] = 1;
            }
        }

        return counts;
    }

    public float GetAverageManaCost()
    {
        if (hand.Count == 0) return 0f;
        return (float)hand.Average(card => card.manaCost);
    }
}