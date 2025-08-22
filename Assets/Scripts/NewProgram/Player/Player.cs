using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : MonoBehaviour
{

    private Character character;
    private BattleManager battleManager;
    private CardManager cardManager;

    void Start()
    {
        character = GetComponent<Character>();
        battleManager = FindFirstObjectByType<BattleManager>();
        cardManager = FindFirstObjectByType<CardManager>();

    }

    public IEnumerator ExecuteTurn()
    {
        var target = battleManager.enemy;
        var usableCards = cardManager.GetUsableCards(character);

        if (usableCards.Count == 0)
        {
            GameEvents.OnDebugMessage?.Invoke("Player has no usable cards");
            yield break;
        }

        var selectedCard = usableCards.First();

        if (selectedCard != null)
        {
            // �J�[�h�g�p
            bool success = cardManager.UseCard(selectedCard, character, target);
            if (success)
            {
                yield return StartCoroutine(ExecuteCardEffect(selectedCard, target));
            }
        }
    }

    IEnumerator ExecuteCardEffect(ConditionalSkillCard card, Character target)
    {
        // �A�j���[�V�����Đ�
        character.GetComponent<CharacterAnimator>()?.PlayAttackAnimation();

        // �G�t�F�N�g���s
        var effects = card.GetEffectsToUse(character, target);
        foreach (var effect in effects)
        {
            ExecuteEffect(effect, target);
            yield return new WaitForSeconds(0.3f);
        }

        // �����Đ�
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
                //int damage = CalculateDamage(effect.value);
                target.TakeDamage(effect.value);
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
}