using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Card Game/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Basic Info")]
    public string characterName;
    public string description;
    public Sprite characterPortrait;
    public Sprite characterSprite;

    [Header("Base Stats")]
    public int maxHealth = 100;
    public int maxMana = 50;
    public int baseAttack = 10;
    public int baseDefense = 5;

    [Header("Growth")]
    public float healthGrowth = 1.1f;
    public float manaGrowth = 1.05f;
    public float attackGrowth = 1.08f;
    public float defenseGrowth = 1.06f;

    [Header("Special Traits")]
    public CharacterTrait[] traits;

    [Header("Audio")]
    public AudioClip[] attackSounds;
    public AudioClip[] damagedSounds;
    public AudioClip[] deathSounds;
    public AudioClip[] victoryVoices;

    [Header("Animation")]
    public RuntimeAnimatorController animatorController;

    public CharacterStats GetStatsAtLevel(int level)
    {
        int health = Mathf.RoundToInt(maxHealth * Mathf.Pow(healthGrowth, level - 1));
        int mana = Mathf.RoundToInt(maxMana * Mathf.Pow(manaGrowth, level - 1));
        int attack = Mathf.RoundToInt(baseAttack * Mathf.Pow(attackGrowth, level - 1));
        int defense = Mathf.RoundToInt(baseDefense * Mathf.Pow(defenseGrowth, level - 1));

        return new CharacterStats(health, mana, attack, defense);
    }
}

[System.Serializable]
public class CharacterTrait
{
    public string traitName;
    public string description;
    public TraitType type;
    public float value;
}

public enum TraitType
{
    DamageBonus,
    DefenseBonus,
    ManaEfficiency,
    CriticalChance,
    Regeneration,
    Resistance
}