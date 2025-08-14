using CardBattle.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Character
{
    [Header("��{���")]
    public string Name;
    public Sprite portrait;

    [Header("�X�e�[�^�X")]
    public CharacterStats Stats;

    [Header("��p�{�[�h")]
    public List<TacticalBoardEntry> tacticalBoard = new List<TacticalBoardEntry>();

    [Header("�����b�N")]
    public List<RelicBase> relics = new List<RelicBase>();

    public Character(string name, int hp, int mp)
    {
        Name = name;
        Stats = new CharacterStats();
        Stats.Initialize(hp, mp);
    }

    public void Initialize(GameSettings settings)
    {
        Stats.Initialize(settings.startingHP, settings.startingMP);

        // �����b�N�̏�����
        foreach (var relic in relics)
        {
            relic.Initialize();
        }
    }

    public void TakeDamage(int damage, BattleContext context)
    {
        int actualDamage = damage;

        // �h��o�t�K�p
        if (Stats.defenseBuff > 0)
        {
            actualDamage = Mathf.RoundToInt(actualDamage * (1f - Stats.defenseBuff));
        }

        // �V�[���h����
        if (Stats.shield > 0)
        {
            int shieldAbsorb = Mathf.Min(Stats.shield, actualDamage);
            Stats.shield -= shieldAbsorb;
            actualDamage -= shieldAbsorb;
            context.Log($"{Name} �̃V�[���h�� {shieldAbsorb} �_���[�W���z��");
        }

        Stats.currentHP = Mathf.Max(0, Stats.currentHP - actualDamage);
        context.Log($"{Name} �� {actualDamage} �_���[�W���󂯂� (HP: {Stats.currentHP}/{Stats.maxHP})");
    }

    public void TakeTrueDamage(int damage, BattleContext context)
    {
        Stats.currentHP = Mathf.Max(0, Stats.currentHP - damage);
        context.Log($"{Name} �� {damage} �̊ђʃ_���[�W���󂯂� (HP: {Stats.currentHP}/{Stats.maxHP})");
    }

    public void Heal(int amount, BattleContext context)
    {
        int actualHeal = Mathf.Min(amount, Stats.maxHP - Stats.currentHP);
        Stats.currentHP += actualHeal;
        context.Log($"{Name} �� {actualHeal} �񕜂��� (HP: {Stats.currentHP}/{Stats.maxHP})");
    }

    public void RestoreMP(int amount, BattleContext context)
    {
        int actualRestore = Mathf.Min(amount, Stats.maxMP - Stats.currentMP);
        Stats.currentMP += actualRestore;
        context.Log($"{Name} ��MP�� {actualRestore} �� (MP: {Stats.currentMP}/{Stats.maxMP})");
    }

    public bool IsAlive() => Stats.currentHP > 0;

    public SkillBase EvaluateTacticalBoard(BattleContext context)
    {
        // �D��x�Ń\�[�g
        var sortedEntries = tacticalBoard
            .Where(e => e.isActive && e.skill != null)
            .OrderByDescending(e => e.priority)
            .ToList();

        foreach (var entry in sortedEntries)
        {
            // �����]��
            if (entry.condition == null || entry.condition.Evaluate(context, this))
            {
                // �X�L�����s�\�`�F�b�N
                if (entry.skill.CanExecute(context, this))
                {
                    context.Log($"{Name} �̏��� [{entry.condition?.conditionName ?? "Always"}] ���������ꂽ");
                    return entry.skill;
                }
            }
        }

        return null;
    }
}