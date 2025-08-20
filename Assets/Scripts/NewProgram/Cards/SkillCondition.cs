using UnityEngine;

[CreateAssetMenu(fileName = "New Condition Card", menuName = "Card Game/Basic Condition Card")]
public class SkillCondition : ScriptableObject
{
    [Header("Condition Settings")]
    public ConditionType conditionType;
    public ConditionTarget target;
    public ComparisonOperator comparisonOperator;
    public int value;
    public string description; // カスタム説明文

    // 条件を満たしているかチェック
    public bool IsMet(Character caster, Character enemy)
    {
        int currentValue = GetCurrentValue(caster, enemy);

        switch (comparisonOperator)
        {
            case ComparisonOperator.GreaterThan:
                return currentValue > value;
            case ComparisonOperator.GreaterThanOrEqual:
                return currentValue >= value;
            case ComparisonOperator.LessThan:
                return currentValue < value;
            case ComparisonOperator.LessThanOrEqual:
                return currentValue <= value;
            case ComparisonOperator.Equal:
                return currentValue == value;
            case ComparisonOperator.NotEqual:
                return currentValue != value;
            default:
                return true;
        }
    }

    private int GetCurrentValue(Character caster, Character enemy)
    {
        Character targetCharacter = (target == ConditionTarget.Self) ? caster : enemy;

        switch (conditionType)
        {
            case ConditionType.Health:
                return targetCharacter.CurrentHealth;
            case ConditionType.HealthPercentage:
                return Mathf.RoundToInt((float)targetCharacter.CurrentHealth / targetCharacter.maxHealth * 100);
            case ConditionType.Mana:
                return targetCharacter.CurrentMana;
            case ConditionType.ManaPercentage:
                return Mathf.RoundToInt((float)targetCharacter.CurrentMana / targetCharacter.maxMana * 100);
            case ConditionType.Attack:
                return targetCharacter.CurrentAttack;
            case ConditionType.Defense:
                return targetCharacter.CurrentDefense;
            case ConditionType.Shield:
                return targetCharacter.CurrentShield;
            case ConditionType.BuffCount:
                return GetBuffCount(targetCharacter, BuffType.Buff);
            case ConditionType.DebuffCount:
                return GetBuffCount(targetCharacter, BuffType.Debuff);
            case ConditionType.TotalBuffCount:
                return GetBuffCount(targetCharacter, BuffType.Buff) + GetBuffCount(targetCharacter, BuffType.Debuff);
            case ConditionType.HandSize:
                return GetHandSize(targetCharacter);
            case ConditionType.TurnNumber:
                return GetCurrentTurnNumber();
            default:
                return 0;
        }
    }

    private int GetBuffCount(Character character, BuffType buffType)
    {
        TurnBasedBuffManager buffManager = character.GetComponent<TurnBasedBuffManager>();
        if (buffManager == null) return 0;

        var buffs = buffManager.GetActiveBuffs();
        int count = 0;
        foreach (var buff in buffs)
        {
            if (buff.buffType == buffType)
                count++;
        }
        return count;
    }

    private int GetHandSize(Character character)
    {
        var cardManager = Object.FindObjectOfType<CardManager>();
        if (cardManager == null) return 0;

        return cardManager.GetHandSize(character);
    }

    private int GetCurrentTurnNumber()
    {
        var turnManager = Object.FindObjectOfType<TurnManager>();
        return turnManager?.CurrentTurn ?? 1;
    }

    // 条件の説明文を自動生成
    public string GetConditionDescription()
    {
        if (!string.IsNullOrEmpty(description))
            return description;

        string targetText = target == ConditionTarget.Self ? "Your" : "Enemy's";
        string conditionText = GetConditionTypeText();
        string operatorText = GetOperatorText();
        string valueText = GetValueText();

        return $"{targetText} {conditionText} {operatorText} {valueText}";
    }

    private string GetConditionTypeText()
    {
        switch (conditionType)
        {
            case ConditionType.Health: return "HP";
            case ConditionType.HealthPercentage: return "HP";
            case ConditionType.Mana: return "Mana";
            case ConditionType.ManaPercentage: return "Mana";
            case ConditionType.Attack: return "攻撃";
            case ConditionType.Defense: return "防御";
            case ConditionType.Shield: return "シールド";
            case ConditionType.BuffCount: return "バフ";
            case ConditionType.DebuffCount: return "デバフ";
            case ConditionType.TotalBuffCount: return "Status Effects";
            case ConditionType.HandSize: return "Hand Size";
            case ConditionType.TurnNumber: return "Turn";
            default: return "Unknown";
        }
    }

    private string GetOperatorText()
    {
        switch (comparisonOperator)
        {
            case ComparisonOperator.GreaterThan: return ">";
            case ComparisonOperator.GreaterThanOrEqual: return "?";
            case ComparisonOperator.LessThan: return "<";
            case ComparisonOperator.LessThanOrEqual: return "?";
            case ComparisonOperator.Equal: return "=";
            case ComparisonOperator.NotEqual: return "≠";
            default: return "";
        }
    }

    private string GetValueText()
    {
        if (conditionType == ConditionType.HealthPercentage || conditionType == ConditionType.ManaPercentage)
        {
            return value + "%";
        }
        return value.ToString();
    }
}