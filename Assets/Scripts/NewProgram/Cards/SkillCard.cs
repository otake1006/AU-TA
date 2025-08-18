
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill Card", menuName = "Card Game/Basic Skill Card")]
public class SkillCard : ScriptableObject
{
    [Header("Basic Info")]
    public int cardID;
    public string cardName;
    public string description;
    public Sprite cardImage;
    public CardRarity rarity = CardRarity.Common;

    [Header("Cost")]
    public int manaCost;

    [Header("Animation")]
    public int animationID;
    public float animationDuration = 1f;

    [Header("Effects")]
    public TurnBasedSkillEffect[] effects;

    [Header("Audio")]
    public AudioClip skillSound;

    [Header("Visual")]
    public Color cardColor = Color.white;
    public Sprite cardFrame;

    // 基本的なスキル実行
    public virtual void ExecuteSkill(Character caster, Character target)
    {
        foreach (var effect in effects)
        {
            ExecuteEffect(effect, caster, target);
        }
    }

    protected virtual void ExecuteEffect(TurnBasedSkillEffect effect, Character caster, Character target)
    {
        // 基本的なエフェクト実行
        // 個別のカードで必要に応じてオーバーライド
    }

    public virtual bool CanUse(Character caster, Character target)
    {
        return caster.CanUseSkills &&
               caster.CurrentMana >= manaCost &&
               !caster.IsDead;
    }

    public virtual string GetTooltipText()
    {
        string tooltip = $"<b>{cardName}</b>\n";
        tooltip += $"<color=blue>Cost: {manaCost}</color>\n\n";
        tooltip += description;

        if (effects != null && effects.Length > 0)
        {
            tooltip += "\n\n<b>Effects:</b>";
            foreach (var effect in effects)
            {
                tooltip += $"\n• {GetEffectDescription(effect)}";
            }
        }

        return tooltip;
    }

    protected string GetEffectDescription(TurnBasedSkillEffect effect)
    {
        switch (effect.effectType)
        {
            case SkillEffectType.Damage:
                return $"Deal {effect.value} damage";
            case SkillEffectType.Heal:
                return $"Heal {effect.value} HP";
            case SkillEffectType.Shield:
                return $"Gain {effect.value} shield";
            case SkillEffectType.ApplyBuff:
                return $"Apply {effect.associatedBuff?.buffName ?? "buff"}";
            case SkillEffectType.RemoveBuff:
                return "Remove debuffs";
            default:
                return effect.effectType.ToString();
        }
    }
}
