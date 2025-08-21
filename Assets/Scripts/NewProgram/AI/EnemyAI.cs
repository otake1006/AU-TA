using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyAI : MonoBehaviour
{
    [Header("AI Settings")]
    public AIDifficulty difficulty = AIDifficulty.Normal;
    public float thinkingTime = 1f;
    public bool enableRandomness = true;

    private Character character;
    private BattleManager battleManager;
    private CardManager cardManager;

    // AI状態
    private AIStrategy currentStrategy;
    private Dictionary<ConditionalSkillCard, float> cardEvaluations = new Dictionary<ConditionalSkillCard, float>();

    void Start()
    {
        character = GetComponent<Character>();
        battleManager = FindObjectOfType<BattleManager>();
        cardManager = FindObjectOfType<CardManager>();

        SetupAIStrategy();
    }

    void SetupAIStrategy()
    {
        switch (difficulty)
        {
            case AIDifficulty.Easy:
                currentStrategy = new RandomStrategy();
                break;
            case AIDifficulty.Normal:
                currentStrategy = new BalancedStrategy();
                break;
            case AIDifficulty.Hard:
                currentStrategy = new AggressiveStrategy();
                break;
            case AIDifficulty.Expert:
                currentStrategy = new OptimalStrategy();
                break;
        }

        currentStrategy.Initialize(this);
    }

    public IEnumerator ExecuteTurn()
    {
        yield return new WaitForSeconds(thinkingTime);

        var player = battleManager.player;
        var usableCards = cardManager.GetUsableCards(character);
        GameEvents.OnDebugMessage?.Invoke(character == battleManager.player ? "playerHand" : "enemyHand");

        if (usableCards.Count == 0)
        {
            GameEvents.OnDebugMessage?.Invoke("Enemy has no usable cards");
            yield break;
        }


        for (int i = 0; i < usableCards.Count; i++)
        {
            // AI戦略に基づいてカードを選択
            var selectedCard = currentStrategy.SelectCard(usableCards, character, player);

            if (selectedCard != null)
            {
                // カード使用
                bool success = cardManager.UseCard(selectedCard, character, player);
                if (success)
                {
                    yield return StartCoroutine(ExecuteCardEffect(selectedCard, player));
                }
            }
        }
    }

    IEnumerator ExecuteCardEffect(ConditionalSkillCard card, Character target)
    {
        // アニメーション再生
        character.GetComponent<CharacterAnimator>()?.PlayAttackAnimation();

        // エフェクト実行
        var effects = card.GetEffectsToUse(character, target);
        foreach (var effect in effects)
        {
            ExecuteEffect(effect, target);
            yield return new WaitForSeconds(0.3f);
        }

        // 音声再生
        if (card.skillSound != null)
        {
            character.GetComponent<CharacterAudio>()?.PlaySFX("Skill");
        }
    }

    void ExecuteEffect(TurnBasedSkillEffect effect, Character target)
    {
        switch (effect.effectType)
        {
            case SkillEffectType.Damage:
                int damage = CalculateDamage(effect.value);
                target.TakeDamage(damage);
                break;

            case SkillEffectType.Heal:
                character.Heal(effect.value);
                break;

            case SkillEffectType.Shield:
                character.AddShield(effect.value);
                break;

            case SkillEffectType.ApplyBuff:
                if (effect.associatedBuff != null)
                {
                    var targetChar = effect.targetType == TargetType.Self ? character : target;
                    var buffManager = targetChar.GetComponent<TurnBasedBuffManager>();
                    if (buffManager != null)
                    {
                        buffManager.ApplyBuff(effect.associatedBuff, effect.duration);
                    }
                }
                break;
        }
    }

    int CalculateDamage(int baseDamage)
    {
        float multiplier = 1f + (character.CurrentAttack - GameConstants.DEFAULT_ATTACK) * 0.1f;
        return Mathf.RoundToInt(baseDamage * multiplier);
    }

    public float EvaluateCard(ConditionalSkillCard card, Character target)
    {
        return currentStrategy.EvaluateCard(card, character, target);
    }
}