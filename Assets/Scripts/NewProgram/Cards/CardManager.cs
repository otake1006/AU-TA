using System;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    private BattleManager battleManager;

    [Header("Decks")]
    public List<ConditionalSkillCard> playerDeck;
    public List<ConditionalSkillCard> enemyDeck;

    [Header("Skill Cards")]
    public List<ConditionalSkillCard> skillCard;

    [Header("Condition Cards")]
    public List<SkillCondition> conditionCard;


    // 手札
    private List<ConditionalSkillCard> playerHand = new List<ConditionalSkillCard>();
    private List<ConditionalSkillCard> enemyHand = new List<ConditionalSkillCard>();

    // 捨て札
    private List<ConditionalSkillCard> playerDiscard = new List<ConditionalSkillCard>();
    private List<ConditionalSkillCard> enemyDiscard = new List<ConditionalSkillCard>();

    // プロパティ
    public List<ConditionalSkillCard> PlayerHand => new List<ConditionalSkillCard>(playerHand);
    public List<ConditionalSkillCard> EnemyHand => new List<ConditionalSkillCard>(enemyHand);

    public void Initialize(BattleManager manager)
    {
        battleManager = manager;
    }


    public void ResetForNewRound()
    {
        // 手札をクリア
        playerHand.Clear();
        enemyHand.Clear();
        playerDiscard.Clear();
        enemyDiscard.Clear();

        // 初期手札を配布
        DrawInitialHands();

        GameEvents.OnHandUpdated?.Invoke(battleManager.player);
        GameEvents.OnHandUpdated?.Invoke(battleManager.enemy);
    }

    public void ResetAllCards()
    {
        ResetForNewRound();

        // デッキもリシャッフル（必要に応じて）
        ShuffleDeck(playerDeck);
        ShuffleDeck(enemyDeck);
    }

    void DrawInitialHands()
    {
        var config = battleManager.gameConfig;

        for (int i = 0; i < config.initialHandSize; i++)
        {
            DrawCard(battleManager.player);
            DrawCard(battleManager.enemy);
        }
    }

    public void DrawCards(Character character, int count)
    {
        for (int i = 0; i < count; i++)
        {
            DrawCard(character);
        }
    }

    public void DrawCard(Character character)
    {
        var config = battleManager.gameConfig;

        if (character == battleManager.player)
        {
            if (playerHand.Count >= config.maxHandSize) return;

            if (playerDeck.Count > 0)
            {
                var card = GetRandomCard(playerDeck);
                playerHand.Add(card);
                GameEvents.OnCardDrawn?.Invoke(character, card);
                GameEvents.OnHandUpdated?.Invoke(character);
            }
        }
        else if (character == battleManager.enemy)
        {
            if (enemyHand.Count >= config.maxHandSize) return;

            if (enemyDeck.Count > 0)
            {
                var card = GetRandomCard(enemyDeck);
                enemyHand.Add(card);
                GameEvents.OnCardDrawn?.Invoke(character, card);
                GameEvents.OnHandUpdated?.Invoke(character);
            }
        }
    }

    ConditionalSkillCard GetRandomCard(List<ConditionalSkillCard> deck)
    {
        if (deck.Count == 0) return null;
        int randomIndex = UnityEngine.Random.Range(0, deck.Count);
        return deck[randomIndex];
    }

    public bool UseCard(ConditionalSkillCard card, Character caster, Character target)
    {
        // カードの使用可能チェック
        if (!card.CanUse(caster, target))
        {
            return false;
        }

        // マナ消費
        if (!caster.ConsumeMana(card.manaCost))
        {
            return false;
        }

        // 手札から削除
        //RemoveCardFromHand(card, caster);

        // 捨て札に追加
        //AddToDiscard(card, caster);

        // イベント発火
        GameEvents.OnCardUsed?.Invoke(card, caster, target);
        //GameEvents.OnTestLogEvent?.Invoke(card, caster, target);
        GameEvents.OnHandUpdated?.Invoke(caster);

        return true;
    }

    void RemoveCardFromHand(ConditionalSkillCard card, Character character)
    {
        if (character == battleManager.player)
        {
            playerHand.Remove(card);
        }
        else if (character == battleManager.enemy)
        {
            enemyHand.Remove(card);
        }
    }

    void AddToDiscard(ConditionalSkillCard card, Character character)
    {
        if (character == battleManager.player)
        {
            playerDiscard.Add(card);
        }
        else if (character == battleManager.enemy)
        {
            enemyDiscard.Add(card);
        }
    }

    void OnCardUsed(ConditionalSkillCard card, Character caster, Character target)
    {
        GameEvents.OnDebugMessage?.Invoke($"{caster.characterName} used {card.cardName}");
    }

    public int GetHandSize(Character character)
    {
        if (character == battleManager.player)
            return playerHand.Count;
        else if (character == battleManager.enemy)
            return enemyHand.Count;
        return 0;
    }

    public List<ConditionalSkillCard> GetUsableCards(Character character)
    {
        var hand = character == battleManager.player ? playerHand : enemyHand;
        var target = character == battleManager.player ? battleManager.enemy : battleManager.player;

        var usableCards = new List<ConditionalSkillCard>();
        foreach (var card in hand)
        {
            if (card.CanUse(character, target))
            {
                usableCards.Add(card);
            }
        }

        return usableCards;
    }

    void ShuffleDeck(List<ConditionalSkillCard> deck)
    {
        for (int i = 0; i < deck.Count; i++)
        {
            var temp = deck[i];
            int randomIndex = UnityEngine.Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    // デバッグ用
    public void DebugAddCardToHand(ConditionalSkillCard card, Character character)
    {
        if (battleManager.gameConfig.enableDebugMode)
        {
            if (character == battleManager.player)
                playerHand.Add(card);
            else if (character == battleManager.enemy)
                enemyHand.Add(card);

            GameEvents.OnHandUpdated?.Invoke(character);
        }
    }

    public void DebugClearHand(Character character)
    {
        if (battleManager.gameConfig.enableDebugMode)
        {
            if (character == battleManager.player)
                playerHand.Clear();
            else if (character == battleManager.enemy)
                enemyHand.Clear();

            GameEvents.OnHandUpdated?.Invoke(character);
        }
    }

    void OnDestroy()
    {
    }
}