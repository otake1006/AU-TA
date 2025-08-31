using System;

[Serializable]
public class CharacterStats
{
    public int currentHealth;
    public int currentMana;
    public int currentAttack;
    public int currentDefense;

    public CharacterStats()
    {
        // デフォルト値
        currentHealth = GameConstants.DEFAULT_MAX_HEALTH;
        currentMana = GameConstants.DEFAULT_MAX_MANA;
        currentAttack = GameConstants.DEFAULT_ATTACK;
        currentDefense = GameConstants.DEFAULT_DEFENSE;
    }

    public CharacterStats(int health, int mana, int attack, int defense)
    {
        currentHealth = health;
        currentMana = mana;
        currentAttack = attack;
        currentDefense = defense;
    }

    public CharacterStats Clone()
    {
        return new CharacterStats(currentHealth, currentMana, currentAttack, currentDefense);
    }

    public override string ToString()
    {
        return $"HP:{currentHealth} MP:{currentMana} ATK:{currentAttack} DEF:{currentDefense}";
    }
}