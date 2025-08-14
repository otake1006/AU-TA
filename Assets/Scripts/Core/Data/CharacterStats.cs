using System;
using UnityEngine;

[Serializable]
public class CharacterStats
{
    [Header("HP/MP")]
    public int maxHP = 100;
    public int currentHP = 100;
    public int maxMP = 50;
    public int currentMP = 50;

    [Header("防御")]
    public int shield = 0;

    [Header("バフ/デバフ")]
    public float attackBuff = 0f;
    public int attackBuffDuration = 0;

    public float defenseBuff = 0f;
    public int defenseBuffDuration = 0;

    public float speedBuff = 0f;
    public int speedBuffDuration = 0;

    [Header("ステータス異常")]
    public int poisonStacks = 0;
    public int burnStacks = 0;
    public int freezeDuration = 0;

    public void Initialize(int hp, int mp)
    {
        maxHP = currentHP = hp;
        maxMP = currentMP = mp;
        shield = 0;
        ClearBuffs();
        ClearDebuffs();
    }

    public void ClearBuffs()
    {
        attackBuff = defenseBuff = speedBuff = 0f;
        attackBuffDuration = defenseBuffDuration = speedBuffDuration = 0;
    }

    public void ClearDebuffs()
    {
        poisonStacks = burnStacks = 0;
        freezeDuration = 0;
    }

    public void UpdateBuffDurations()
    {
        if (attackBuffDuration > 0)
        {
            attackBuffDuration--;
            if (attackBuffDuration == 0) attackBuff = 0;
        }

        if (defenseBuffDuration > 0)
        {
            defenseBuffDuration--;
            if (defenseBuffDuration == 0) defenseBuff = 0;
        }

        if (speedBuffDuration > 0)
        {
            speedBuffDuration--;
            if (speedBuffDuration == 0) speedBuff = 0;
        }

        if (freezeDuration > 0) freezeDuration--;
    }
}