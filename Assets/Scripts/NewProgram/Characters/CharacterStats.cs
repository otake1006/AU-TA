using System;
using UnityEngine;

[Serializable]
public class CharacterStats
{
    public int currentHealth;
    public int currentMana;
    public int currentAttack;
    public int currentDefense;
    
    // 基本ステータス（救済用）
    public int maxHealth;
    public int maxMana;
    public int baseAttack;
    public int baseDefense;

    public CharacterStats()
    {
        // �f�t�H���g�l
        maxHealth = GameConstants.DEFAULT_MAX_HEALTH;
        maxMana = GameConstants.DEFAULT_MAX_MANA;
        baseAttack = GameConstants.DEFAULT_ATTACK;
        baseDefense = GameConstants.DEFAULT_DEFENSE;
        
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentAttack = baseAttack;
        currentDefense = baseDefense;
    }

    public CharacterStats(int maxHp, int maxMp, int atk, int def)
    {
        maxHealth = maxHp;
        maxMana = maxMp;
        baseAttack = atk;
        baseDefense = def;
        
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentAttack = baseAttack;
        currentDefense = baseDefense;
    }

    public CharacterStats Clone()
    {
        var clone = new CharacterStats(maxHealth, maxMana, baseAttack, baseDefense);
        clone.currentHealth = currentHealth;
        clone.currentMana = currentMana;
        clone.currentAttack = currentAttack;
        clone.currentDefense = currentDefense;
        return clone;
    }

    // ステータス操作メソッド
    public void RestoreHealth(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    public void RestoreMana(int amount)
    {
        currentMana = Mathf.Min(currentMana + amount, maxMana);
    }

    public void ModifyAttack(int amount)
    {
        currentAttack = Mathf.Max(0, currentAttack + amount);
    }

    public void ModifyDefense(int amount)
    {
        currentDefense = Mathf.Max(0, currentDefense + amount);
    }

    public void SetAlive(bool alive)
    {
        if (alive && currentHealth <= 0)
        {
            currentHealth = 1; // 最低1HPで復活
        }
        else if (!alive)
        {
            currentHealth = 0;
        }
    }

    // プロパティ
    public int MaxHealth => maxHealth;
    public int MaxMana => maxMana;
    public int BaseAttack => baseAttack;
    public int BaseDefense => baseDefense;
    public bool IsAlive => currentHealth > 0;

    public override string ToString()
    {
        return $"HP:{currentHealth}/{maxHealth} MP:{currentMana}/{maxMana} ATK:{currentAttack} DEF:{currentDefense}";
    }
}