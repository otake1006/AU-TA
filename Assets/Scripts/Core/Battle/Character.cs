using CardBattle.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Character
{
    [Header("基本情報")]
    public string Name;
    public Sprite portrait;

    [Header("ステータス")]
    public CharacterStats Stats;

    [Header("戦術ボード")]
    public List<TacticalBoardEntry> tacticalBoard = new List<TacticalBoardEntry>();

    [Header("レリック")]
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

        // レリックの初期化
        foreach (var relic in relics)
        {
            relic.Initialize();
        }
    }

    public void TakeDamage(int damage, BattleContext context)
    {
        int actualDamage = damage;

        // 防御バフ適用
        if (Stats.defenseBuff > 0)
        {
            actualDamage = Mathf.RoundToInt(actualDamage * (1f - Stats.defenseBuff));
        }

        // シールド処理
        if (Stats.shield > 0)
        {
            int shieldAbsorb = Mathf.Min(Stats.shield, actualDamage);
            Stats.shield -= shieldAbsorb;
            actualDamage -= shieldAbsorb;
            context.Log($"{Name} のシールドが {shieldAbsorb} ダメージを吸収");
        }

        Stats.currentHP = Mathf.Max(0, Stats.currentHP - actualDamage);
        context.Log($"{Name} は {actualDamage} ダメージを受けた (HP: {Stats.currentHP}/{Stats.maxHP})");
    }

    public void TakeTrueDamage(int damage, BattleContext context)
    {
        Stats.currentHP = Mathf.Max(0, Stats.currentHP - damage);
        context.Log($"{Name} は {damage} の貫通ダメージを受けた (HP: {Stats.currentHP}/{Stats.maxHP})");
    }

    public void Heal(int amount, BattleContext context)
    {
        int actualHeal = Mathf.Min(amount, Stats.maxHP - Stats.currentHP);
        Stats.currentHP += actualHeal;
        context.Log($"{Name} は {actualHeal} 回復した (HP: {Stats.currentHP}/{Stats.maxHP})");
    }

    public void RestoreMP(int amount, BattleContext context)
    {
        int actualRestore = Mathf.Min(amount, Stats.maxMP - Stats.currentMP);
        Stats.currentMP += actualRestore;
        context.Log($"{Name} のMPが {actualRestore} 回復 (MP: {Stats.currentMP}/{Stats.maxMP})");
    }

    public bool IsAlive() => Stats.currentHP > 0;

    public SkillBase EvaluateTacticalBoard(BattleContext context)
    {
        // 優先度でソート
        var sortedEntries = tacticalBoard
            .Where(e => e.isActive && e.skill != null)
            .OrderByDescending(e => e.priority)
            .ToList();

        foreach (var entry in sortedEntries)
        {
            // 条件評価
            if (entry.condition == null || entry.condition.Evaluate(context, this))
            {
                // スキル実行可能チェック
                if (entry.skill.CanExecute(context, this))
                {
                    context.Log($"{Name} の条件 [{entry.condition?.conditionName ?? "Always"}] が満たされた");
                    return entry.skill;
                }
            }
        }

        return null;
    }
}