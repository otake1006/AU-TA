//// ========================================
//// Core Interfaces & Enums
//// ========================================

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEditor;
//using UnityEngine;

//namespace CardBattle.Core
//{
//    // ========================================
//    // ��{�I�ȗ񋓌^��`
//    // ========================================

//    public enum SkillTiming
//    {
//        Instant,
//        TurnStart,
//        TurnEnd,
//        RoundStart,
//        RoundEnd,
//        OnDamage,
//        OnHeal,
//        OnSkillUse,
//        OnConditionMet
//    }

//    public enum TargetType
//    {
//        Self,
//        Enemy,
//        Both,
//        Random
//    }

//    public enum ComparisonOperator
//    {
//        GreaterThan,    // >
//        LessThan,       // <
//        GreaterOrEqual, // >=
//        LessOrEqual,    // <=
//        Equal,          // ==
//        NotEqual        // !=
//    }

//    public enum ConditionTarget
//    {
//        Self,
//        Enemy,
//        Both
//    }

//    public enum EffectType
//    {
//        Damage,
//        Heal,
//        MPRestore,
//        MPDrain,
//        Shield,
//        AttackBuff,
//        DefenseBuff,
//        SpeedBuff,
//        AttackDebuff,
//        DefenseDebuff,
//        StatusEffect,
//        Custom
//    }

//    public enum RelicTrigger
//    {
//        TurnStart,
//        TurnEnd,
//        RoundStart,
//        RoundEnd,
//        OnDamageDealt,
//        OnDamageTaken,
//        OnHeal,
//        OnSkillNotUsed,
//        OnLowHP,
//        Always
//    }

//    // ========================================
//    // ScriptableObject Base Classes
//    // ========================================

//    [CreateAssetMenu(fileName = "GameSettings", menuName = "CardBattle/GameSettings")]
//    public class GameSettings : ScriptableObject
//    {
//        [Header("���E���h�ݒ�")]
//        public int maxRounds = 5;
//        public int turnsPerRound = 10;

//        [Header("�����X�e�[�^�X")]
//        public int startingHP = 100;
//        public int startingMP = 50;

//        [Header("�Q�[�����J�j�N�X")]
//        public int mpRecoveryPerTurn = 10;
//        public int roundEndHPRecover = 10;
//        public int roundEndMPRecover = 20;

//        [Header("AI�ݒ�")]
//        public float aiThinkTime = 0.5f;

//        [Header("�f�o�b�O")]
//        public bool enableDetailedLogging = true;
//    }

//    // ========================================
//    // Skill ScriptableObjects
//    // ========================================

//    public abstract class SkillBase : ScriptableObject
//    {
//        [Header("��{���")]
//        public string skillId;
//        public string skillName;
//        [TextArea(2, 4)]
//        public string description;
//        public Sprite icon;

//        [Header("�R�X�g")]
//        public int mpCost = 10;

//        [Header("�^�[�Q�b�g")]
//        public TargetType targetType = TargetType.Enemy;

//        [Header("�^�C�~���O")]
//        public SkillTiming timing = SkillTiming.Instant;

//        public abstract bool CanExecute(BattleContext context, Character caster);
//        public abstract void Execute(BattleContext context, Character caster, Character target);
//    }

//    [CreateAssetMenu(fileName = "DamageSkill", menuName = "CardBattle/Skills/Damage")]
//    public class DamageSkill : SkillBase
//    {
//        [Header("�_���[�W�ݒ�")]
//        public int baseDamage = 20;
//        public float damageMultiplier = 1.0f;
//        public bool ignoreShield = false;

//        [Header("�ǉ�����")]
//        public bool hasLifeSteal = false;
//        [Range(0f, 1f)]
//        public float lifeStealPercent = 0.3f;

//        public override bool CanExecute(BattleContext context, Character caster)
//        {
//            return caster.Stats.currentMP >= mpCost;
//        }

//        public override void Execute(BattleContext context, Character caster, Character target)
//        {
//            caster.Stats.currentMP -= mpCost;

//            // �o�t�v�Z
//            float totalMultiplier = damageMultiplier;
//            if (caster.Stats.attackBuff > 0)
//                totalMultiplier *= (1f + caster.Stats.attackBuff);

//            int finalDamage = Mathf.RoundToInt(baseDamage * totalMultiplier);

//            // �_���[�W����
//            if (ignoreShield)
//            {
//                target.TakeTrueDamage(finalDamage, context);
//            }
//            else
//            {
//                target.TakeDamage(finalDamage, context);
//            }

//            // ���C�t�X�e�B�[��
//            if (hasLifeSteal)
//            {
//                int healAmount = Mathf.RoundToInt(finalDamage * lifeStealPercent);
//                caster.Heal(healAmount, context);
//                context.Log($"{caster.Name} �����C�t�X�e�B�[���� {healAmount} ��");
//            }

//            context.Log($"{caster.Name} �� {skillName} �𔭓��I {target.Name} �� {finalDamage} �_���[�W");
//        }
//    }

//    [CreateAssetMenu(fileName = "HealSkill", menuName = "CardBattle/Skills/Heal")]
//    public class HealSkill : SkillBase
//    {
//        [Header("�񕜐ݒ�")]
//        public int baseHealAmount = 25;
//        public bool healPercentage = false;
//        [Range(0f, 1f)]
//        public float healPercent = 0.3f;

//        [Header("�ǉ�����")]
//        public bool removeDebuffs = false;
//        public int bonusShield = 0;

//        public override bool CanExecute(BattleContext context, Character caster)
//        {
//            return caster.Stats.currentMP >= mpCost;
//        }

//        public override void Execute(BattleContext context, Character caster, Character target)
//        {
//            caster.Stats.currentMP -= mpCost;

//            int healAmount = healPercentage ?
//                Mathf.RoundToInt(target.Stats.maxHP * healPercent) :
//                baseHealAmount;

//            target.Heal(healAmount, context);

//            if (bonusShield > 0)
//            {
//                target.Stats.shield += bonusShield;
//                context.Log($"{target.Name} �ɃV�[���h +{bonusShield}");
//            }

//            if (removeDebuffs)
//            {
//                target.Stats.ClearDebuffs();
//                context.Log($"{target.Name} �̃f�o�t������");
//            }

//            context.Log($"{caster.Name} �� {skillName} �𔭓��I {healAmount} ��");
//        }
//    }

//    [CreateAssetMenu(fileName = "BuffSkill", menuName = "CardBattle/Skills/Buff")]
//    public class BuffSkill : SkillBase
//    {
//        [Header("�o�t�ݒ�")]
//        public EffectType buffType = EffectType.AttackBuff;
//        [Range(0f, 2f)]
//        public float buffValue = 0.5f;
//        public int duration = 3;

//        [Header("�ǉ�MP��")]
//        public bool restoreMP = false;
//        public int mpRestoreAmount = 10;

//        public override bool CanExecute(BattleContext context, Character caster)
//        {
//            return caster.Stats.currentMP >= mpCost;
//        }

//        public override void Execute(BattleContext context, Character caster, Character target)
//        {
//            caster.Stats.currentMP -= mpCost;

//            switch (buffType)
//            {
//                case EffectType.AttackBuff:
//                    target.Stats.attackBuff += buffValue;
//                    target.Stats.attackBuffDuration = duration;
//                    context.Log($"{target.Name} �̍U���� +{buffValue * 100}%");
//                    break;

//                case EffectType.DefenseBuff:
//                    target.Stats.defenseBuff += buffValue;
//                    target.Stats.defenseBuffDuration = duration;
//                    context.Log($"{target.Name} �̖h��� +{buffValue * 100}%");
//                    break;

//                case EffectType.SpeedBuff:
//                    target.Stats.speedBuff += buffValue;
//                    target.Stats.speedBuffDuration = duration;
//                    context.Log($"{target.Name} �̑��x +{buffValue * 100}%");
//                    break;
//            }

//            if (restoreMP)
//            {
//                target.RestoreMP(mpRestoreAmount, context);
//            }

//            context.Log($"{caster.Name} �� {skillName} �𔭓��I");
//        }
//    }

//    // ========================================
//    // Condition ScriptableObjects
//    // ========================================

//    public abstract class ConditionBase : ScriptableObject
//    {
//        [Header("��{���")]
//        public string conditionId;
//        public string conditionName;
//        [TextArea(2, 3)]
//        public string description;

//        public abstract bool Evaluate(BattleContext context, Character owner);
//    }

//    [CreateAssetMenu(fileName = "HPCondition", menuName = "CardBattle/Conditions/HP")]
//    public class HPCondition : ConditionBase
//    {
//        [Header("HP����")]
//        public ConditionTarget checkTarget = ConditionTarget.Enemy;
//        public ComparisonOperator comparison = ComparisonOperator.LessThan;
//        [Range(0f, 100f)]
//        public float hpPercentThreshold = 30f;

//        public override bool Evaluate(BattleContext context, Character owner)
//        {
//            Character target = GetTarget(context, owner, checkTarget);
//            if (target == null) return false;

//            float hpPercent = (target.Stats.currentHP / (float)target.Stats.maxHP) * 100f;

//            return comparison switch
//            {
//                ComparisonOperator.GreaterThan => hpPercent > hpPercentThreshold,
//                ComparisonOperator.LessThan => hpPercent < hpPercentThreshold,
//                ComparisonOperator.GreaterOrEqual => hpPercent >= hpPercentThreshold,
//                ComparisonOperator.LessOrEqual => hpPercent <= hpPercentThreshold,
//                ComparisonOperator.Equal => Mathf.Abs(hpPercent - hpPercentThreshold) < 0.01f,
//                ComparisonOperator.NotEqual => Mathf.Abs(hpPercent - hpPercentThreshold) >= 0.01f,
//                _ => false
//            };
//        }

//        private Character GetTarget(BattleContext context, Character owner, ConditionTarget target)
//        {
//            return target switch
//            {
//                ConditionTarget.Self => owner,
//                ConditionTarget.Enemy => owner == context.Player ? context.Enemy : context.Player,
//                _ => null
//            };
//        }
//    }

//    [CreateAssetMenu(fileName = "MPCondition", menuName = "CardBattle/Conditions/MP")]
//    public class MPCondition : ConditionBase
//    {
//        [Header("MP����")]
//        public ConditionTarget checkTarget = ConditionTarget.Self;
//        public ComparisonOperator comparison = ComparisonOperator.GreaterThan;
//        public int mpThreshold = 30;

//        public override bool Evaluate(BattleContext context, Character owner)
//        {
//            Character target = checkTarget == ConditionTarget.Self ? owner :
//                             (owner == context.Player ? context.Enemy : context.Player);

//            return comparison switch
//            {
//                ComparisonOperator.GreaterThan => target.Stats.currentMP > mpThreshold,
//                ComparisonOperator.LessThan => target.Stats.currentMP < mpThreshold,
//                ComparisonOperator.GreaterOrEqual => target.Stats.currentMP >= mpThreshold,
//                ComparisonOperator.LessOrEqual => target.Stats.currentMP <= mpThreshold,
//                ComparisonOperator.Equal => target.Stats.currentMP == mpThreshold,
//                ComparisonOperator.NotEqual => target.Stats.currentMP != mpThreshold,
//                _ => false
//            };
//        }
//    }

//    [CreateAssetMenu(fileName = "TurnCondition", menuName = "CardBattle/Conditions/Turn")]
//    public class TurnCondition : ConditionBase
//    {
//        [Header("�^�[������")]
//        public ComparisonOperator comparison = ComparisonOperator.GreaterOrEqual;
//        public int turnNumber = 5;
//        public bool checkRoundInstead = false;

//        public override bool Evaluate(BattleContext context, Character owner)
//        {
//            int value = checkRoundInstead ? context.CurrentRound : context.CurrentTurn;

//            return comparison switch
//            {
//                ComparisonOperator.GreaterThan => value > turnNumber,
//                ComparisonOperator.LessThan => value < turnNumber,
//                ComparisonOperator.GreaterOrEqual => value >= turnNumber,
//                ComparisonOperator.LessOrEqual => value <= turnNumber,
//                ComparisonOperator.Equal => value == turnNumber,
//                ComparisonOperator.NotEqual => value != turnNumber,
//                _ => false
//            };
//        }
//    }

//    [CreateAssetMenu(fileName = "AlwaysCondition", menuName = "CardBattle/Conditions/Always")]
//    public class AlwaysCondition : ConditionBase
//    {
//        public override bool Evaluate(BattleContext context, Character owner)
//        {
//            return true;
//        }
//    }

//    // ========================================
//    // Relic ScriptableObjects
//    // ========================================

//    public abstract class RelicBase : ScriptableObject
//    {
//        [Header("��{���")]
//        public string relicId;
//        public string relicName;
//        [TextArea(2, 4)]
//        public string description;
//        public Sprite icon;

//        [Header("��������")]
//        public RelicTrigger trigger = RelicTrigger.TurnStart;

//        [Header("�g�p����")]
//        public bool hasUsageLimit = false;
//        public int maxUses = 3;

//        [HideInInspector]
//        public int remainingUses;
//        [HideInInspector]
//        public bool isActive = true;

//        public virtual void Initialize()
//        {
//            remainingUses = maxUses;
//            isActive = true;
//        }

//        public abstract void OnTrigger(BattleContext context, Character owner, TriggerEvent evt);

//        protected bool ConsumeUse()
//        {
//            if (!isActive) return false;
//            if (!hasUsageLimit) return true;

//            remainingUses--;
//            if (remainingUses <= 0)
//            {
//                isActive = false;
//                return true;
//            }
//            return true;
//        }
//    }

//    [CreateAssetMenu(fileName = "HealingRelic", menuName = "CardBattle/Relics/Healing")]
//    public class HealingRelic : RelicBase
//    {
//        [Header("�񕜐ݒ�")]
//        public int healAmount = 5;
//        public bool healPercentage = false;
//        [Range(0f, 0.5f)]
//        public float healPercent = 0.1f;

//        public override void OnTrigger(BattleContext context, Character owner, TriggerEvent evt)
//        {
//            if (!ConsumeUse()) return;

//            int amount = healPercentage ?
//                Mathf.RoundToInt(owner.Stats.maxHP * healPercent) :
//                healAmount;

//            owner.Heal(amount, context);
//            context.Log($"{owner.Name} �� {relicName} �������I {amount} ��");
//        }
//    }

//    [CreateAssetMenu(fileName = "CounterRelic", menuName = "CardBattle/Relics/Counter")]
//    public class CounterRelic : RelicBase
//    {
//        [Header("�J�E���^�[�ݒ�")]
//        [Range(0f, 2f)]
//        public float counterDamageMultiplier = 0.5f;
//        public int fixedCounterDamage = 0;

//        public override void OnTrigger(BattleContext context, Character owner, TriggerEvent evt)
//        {
//            if (!ConsumeUse()) return;
//            if (!evt.Data.ContainsKey("Damage")) return;

//            int damage = (int)evt.Data["Damage"];
//            int counterDamage = fixedCounterDamage > 0 ?
//                fixedCounterDamage :
//                Mathf.RoundToInt(damage * counterDamageMultiplier);

//            Character attacker = owner == context.Player ? context.Enemy : context.Player;
//            attacker.TakeDamage(counterDamage, context);

//            context.Log($"{relicName} �����I {counterDamage} �̔����_���[�W");
//        }
//    }

//    [CreateAssetMenu(fileName = "MPRegenRelic", menuName = "CardBattle/Relics/MPRegen")]
//    public class MPRegenRelic : RelicBase
//    {
//        [Header("MP�񕜐ݒ�")]
//        public int mpRestoreAmount = 5;

//        public override void OnTrigger(BattleContext context, Character owner, TriggerEvent evt)
//        {
//            if (!ConsumeUse()) return;

//            owner.RestoreMP(mpRestoreAmount, context);
//            context.Log($"{relicName} �����I MP +{mpRestoreAmount}");
//        }
//    }

//    // ========================================
//    // Character Stats (Serializable)
//    // ========================================

//    [Serializable]
//    public class CharacterStats
//    {
//        [Header("HP/MP")]
//        public int maxHP = 100;
//        public int currentHP = 100;
//        public int maxMP = 50;
//        public int currentMP = 50;

//        [Header("�h��")]
//        public int shield = 0;

//        [Header("�o�t/�f�o�t")]
//        public float attackBuff = 0f;
//        public int attackBuffDuration = 0;

//        public float defenseBuff = 0f;
//        public int defenseBuffDuration = 0;

//        public float speedBuff = 0f;
//        public int speedBuffDuration = 0;

//        [Header("�X�e�[�^�X�ُ�")]
//        public int poisonStacks = 0;
//        public int burnStacks = 0;
//        public int freezeDuration = 0;

//        public void Initialize(int hp, int mp)
//        {
//            maxHP = currentHP = hp;
//            maxMP = currentMP = mp;
//            shield = 0;
//            ClearBuffs();
//            ClearDebuffs();
//        }

//        public void ClearBuffs()
//        {
//            attackBuff = defenseBuff = speedBuff = 0f;
//            attackBuffDuration = defenseBuffDuration = speedBuffDuration = 0;
//        }

//        public void ClearDebuffs()
//        {
//            poisonStacks = burnStacks = 0;
//            freezeDuration = 0;
//        }

//        public void UpdateBuffDurations()
//        {
//            if (attackBuffDuration > 0)
//            {
//                attackBuffDuration--;
//                if (attackBuffDuration == 0) attackBuff = 0;
//            }

//            if (defenseBuffDuration > 0)
//            {
//                defenseBuffDuration--;
//                if (defenseBuffDuration == 0) defenseBuff = 0;
//            }

//            if (speedBuffDuration > 0)
//            {
//                speedBuffDuration--;
//                if (speedBuffDuration == 0) speedBuff = 0;
//            }

//            if (freezeDuration > 0) freezeDuration--;
//        }
//    }

//    // ========================================
//    // Tactical Board Entry
//    // ========================================

//    [Serializable]
//    public class TacticalBoardEntry
//    {
//        public string entryName = "New Entry";
//        public ConditionBase condition;
//        public SkillBase skill;
//        [Range(0, 100)]
//        public int priority = 50;
//        public bool isActive = true;
//    }

//    // ========================================
//    // Core Game Classes
//    // ========================================

//    public class TriggerEvent
//    {
//        public string EventType { get; set; }
//        public Character Source { get; set; }
//        public Character Target { get; set; }
//        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
//    }

//    public class BattleContext
//    {
//        public int CurrentRound { get; set; }
//        public int CurrentTurn { get; set; }
//        public Character Player { get; set; }
//        public Character Enemy { get; set; }
//        public GameSettings Settings { get; set; }
//        public List<string> BattleLog { get; set; } = new List<string>();

//        public void Log(string message)
//        {
//            BattleLog.Add($"[R{CurrentRound}T{CurrentTurn}] {message}");
//            if (Settings.enableDetailedLogging)
//                Debug.Log($"[Battle] {message}");
//        }
//    }

//    // ========================================
//    // Character Class
//    // ========================================

//    [Serializable]
//    public class Character
//    {
//        [Header("��{���")]
//        public string Name;
//        public Sprite portrait;

//        [Header("�X�e�[�^�X")]
//        public CharacterStats Stats;

//        [Header("��p�{�[�h")]
//        public List<TacticalBoardEntry> tacticalBoard = new List<TacticalBoardEntry>();

//        [Header("�����b�N")]
//        public List<RelicBase> relics = new List<RelicBase>();

//        public Character(string name, int hp, int mp)
//        {
//            Name = name;
//            Stats = new CharacterStats();
//            Stats.Initialize(hp, mp);
//        }

//        public void Initialize(GameSettings settings)
//        {
//            Stats.Initialize(settings.startingHP, settings.startingMP);

//            // �����b�N�̏�����
//            foreach (var relic in relics)
//            {
//                relic.Initialize();
//            }
//        }

//        public void TakeDamage(int damage, BattleContext context)
//        {
//            int actualDamage = damage;

//            // �h��o�t�K�p
//            if (Stats.defenseBuff > 0)
//            {
//                actualDamage = Mathf.RoundToInt(actualDamage * (1f - Stats.defenseBuff));
//            }

//            // �V�[���h����
//            if (Stats.shield > 0)
//            {
//                int shieldAbsorb = Mathf.Min(Stats.shield, actualDamage);
//                Stats.shield -= shieldAbsorb;
//                actualDamage -= shieldAbsorb;
//                context.Log($"{Name} �̃V�[���h�� {shieldAbsorb} �_���[�W���z��");
//            }

//            Stats.currentHP = Mathf.Max(0, Stats.currentHP - actualDamage);
//            context.Log($"{Name} �� {actualDamage} �_���[�W���󂯂� (HP: {Stats.currentHP}/{Stats.maxHP})");
//        }

//        public void TakeTrueDamage(int damage, BattleContext context)
//        {
//            Stats.currentHP = Mathf.Max(0, Stats.currentHP - damage);
//            context.Log($"{Name} �� {damage} �̊ђʃ_���[�W���󂯂� (HP: {Stats.currentHP}/{Stats.maxHP})");
//        }

//        public void Heal(int amount, BattleContext context)
//        {
//            int actualHeal = Mathf.Min(amount, Stats.maxHP - Stats.currentHP);
//            Stats.currentHP += actualHeal;
//            context.Log($"{Name} �� {actualHeal} �񕜂��� (HP: {Stats.currentHP}/{Stats.maxHP})");
//        }

//        public void RestoreMP(int amount, BattleContext context)
//        {
//            int actualRestore = Mathf.Min(amount, Stats.maxMP - Stats.currentMP);
//            Stats.currentMP += actualRestore;
//            context.Log($"{Name} ��MP�� {actualRestore} �� (MP: {Stats.currentMP}/{Stats.maxMP})");
//        }

//        public bool IsAlive() => Stats.currentHP > 0;

//        public SkillBase EvaluateTacticalBoard(BattleContext context)
//        {
//            // �D��x�Ń\�[�g
//            var sortedEntries = tacticalBoard
//                .Where(e => e.isActive && e.skill != null)
//                .OrderByDescending(e => e.priority)
//                .ToList();

//            foreach (var entry in sortedEntries)
//            {
//                // �����]��
//                if (entry.condition == null || entry.condition.Evaluate(context, this))
//                {
//                    // �X�L�����s�\�`�F�b�N
//                    if (entry.skill.CanExecute(context, this))
//                    {
//                        context.Log($"{Name} �̏��� [{entry.condition?.conditionName ?? "Always"}] ���������ꂽ");
//                        return entry.skill;
//                    }
//                }
//            }

//            return null;
//        }
//    }

//    // ========================================
//    // Battle Manager
//    // ========================================

//    public class BattleManager
//    {
//        private BattleContext _context;
//        private GameSettings _settings;
//        private bool _battleEnded;

//        public event Action<Character> OnBattleEnd;
//        public event Action<int> OnRoundStart;
//        public event Action<int> OnRoundEnd;

//        public BattleManager(GameSettings settings)
//        {
//            _settings = settings;
//        }

//        public void StartBattle(Character player, Character enemy)
//        {
//            _context = new BattleContext
//            {
//                Settings = _settings,
//                Player = player,
//                Enemy = enemy,
//                CurrentRound = 1,
//                CurrentTurn = 1
//            };

//            // �L�����N�^�[������
//            player.Initialize(_settings);
//            enemy.Initialize(_settings);

//            Debug.Log("=== BATTLE START ===");
//            Debug.Log($"{player.Name} VS {enemy.Name}");

//            RunBattle();
//        }

//        private void RunBattle()
//        {
//            _battleEnded = false;

//            for (int round = 1; round <= _settings.maxRounds && !_battleEnded; round++)
//            {
//                _context.CurrentRound = round;
//                RunRound();

//                if (_battleEnded) break;

//                if (round < _settings.maxRounds)
//                {
//                    ProcessRoundEnd();
//                }
//            }

//            if (!_battleEnded)
//            {
//                DetermineFinalWinner();
//            }
//        }

//        private void RunRound()
//        {
//            Debug.Log($"\n=== ROUND {_context.CurrentRound} START ===");
//            OnRoundStart?.Invoke(_context.CurrentRound);

//            ProcessRelics(RelicTrigger.RoundStart);

//            for (int turn = 1; turn <= _settings.turnsPerRound && !_battleEnded; turn++)
//            {
//                _context.CurrentTurn = turn;
//                RunTurn();
//            }

//            if (!_battleEnded && _context.CurrentTurn >= _settings.turnsPerRound)
//            {
//                Debug.Log($"���E���h {_context.CurrentRound} �͈�������");
//            }

//            OnRoundEnd?.Invoke(_context.CurrentRound);
//        }

//        private void RunTurn()
//        {
//            Debug.Log($"\n--- Turn {_context.CurrentTurn} ---");

//            // MP��
//            _context.Player.RestoreMP(_settings.mpRecoveryPerTurn, _context);
//            _context.Enemy.RestoreMP(_settings.mpRecoveryPerTurn, _context);

//            // �o�t���ԍX�V
//            _context.Player.Stats.UpdateBuffDurations();
//            _context.Enemy.Stats.UpdateBuffDurations();

//            ProcessRelics(RelicTrigger.TurnStart);

//            // ��p�{�[�h�]��
//            var playerSkill = _context.Player.EvaluateTacticalBoard(_context);
//            var enemySkill = _context.Enemy.EvaluateTacticalBoard(_context);

//            // �X�L���������s
//            if (playerSkill != null)
//            {
//                ExecuteSkill(_context.Player, playerSkill);
//            }
//            else
//            {
//                ProcessSkillNotUsed(_context.Player);
//            }

//            if (enemySkill != null)
//            {
//                ExecuteSkill(_context.Enemy, enemySkill);
//            }
//            else
//            {
//                ProcessSkillNotUsed(_context.Enemy);
//            }

//            ProcessRelics(RelicTrigger.TurnEnd);

//            CheckBattleEnd();
//            DisplayStatus();
//        }

//        private void ExecuteSkill(Character caster, SkillBase skill)
//        {
//            Character target = skill.targetType == TargetType.Enemy ?
//                (caster == _context.Player ? _context.Enemy : _context.Player) : caster;

//            skill.Execute(_context, caster, target);
//        }

//        private void ProcessSkillNotUsed(Character character)
//        {
//            foreach (var relic in character.relics.Where(r => r.trigger == RelicTrigger.OnSkillNotUsed && r.isActive))
//            {
//                relic.OnTrigger(_context, character, new TriggerEvent
//                {
//                    EventType = "SkillNotUsed",
//                    Source = character
//                });
//            }
//        }

//        private void ProcessRelics(RelicTrigger trigger)
//        {
//            foreach (var character in new[] { _context.Player, _context.Enemy })
//            {
//                foreach (var relic in character.relics.Where(r => r.trigger == trigger && r.isActive))
//                {
//                    relic.OnTrigger(_context, character, new TriggerEvent
//                    {
//                        EventType = trigger.ToString(),
//                        Source = character
//                    });
//                }
//            }
//        }

//        private void CheckBattleEnd()
//        {
//            bool playerAlive = _context.Player.IsAlive();
//            bool enemyAlive = _context.Enemy.IsAlive();

//            if (!playerAlive && !enemyAlive)
//            {
//                Debug.Log("=== �������� (�������j) ===");
//                _battleEnded = true;
//                OnBattleEnd?.Invoke(null);
//            }
//            else if (!playerAlive)
//            {
//                Debug.Log($"=== {_context.Enemy.Name} �̏����I ===");
//                _battleEnded = true;
//                OnBattleEnd?.Invoke(_context.Enemy);
//            }
//            else if (!enemyAlive)
//            {
//                Debug.Log($"=== {_context.Player.Name} �̏����I ===");
//                _battleEnded = true;
//                OnBattleEnd?.Invoke(_context.Player);
//            }
//        }

//        private void ProcessRoundEnd()
//        {
//            Debug.Log($"\n=== ���E���h {_context.CurrentRound} �I������ ===");

//            // HP/MP������
//            _context.Player.Heal(_settings.roundEndHPRecover, _context);
//            _context.Enemy.Heal(_settings.roundEndHPRecover, _context);
//            _context.Player.RestoreMP(_settings.roundEndMPRecover, _context);
//            _context.Enemy.RestoreMP(_settings.roundEndMPRecover, _context);

//            // �s�Ҕ���ƃ����b�N�t�^�i�~�σV�X�e���j
//            Character loser = null;
//            if (_context.Player.Stats.currentHP < _context.Enemy.Stats.currentHP)
//                loser = _context.Player;
//            else if (_context.Enemy.Stats.currentHP < _context.Player.Stats.currentHP)
//                loser = _context.Enemy;

//            if (loser != null)
//            {
//                // ���I�Ƀ����b�N�𐶐�����ꍇ�̗�
//                // ���ۂɂ̓v���Z�b�g�̃����b�N����I�����邱�Ƃ𐄏�
//                _context.Log($"{loser.Name} �ɋ~�ϑ[�u��K�p");
//            }
//        }

//        private void DetermineFinalWinner()
//        {
//            Debug.Log("\n=== �ŏI���� ===");

//            if (_context.Player.Stats.currentHP > _context.Enemy.Stats.currentHP)
//            {
//                Debug.Log($"{_context.Player.Name} �̏����I (HP��)");
//                OnBattleEnd?.Invoke(_context.Player);
//            }
//            else if (_context.Enemy.Stats.currentHP > _context.Player.Stats.currentHP)
//            {
//                Debug.Log($"{_context.Enemy.Name} �̏����I (HP��)");
//                OnBattleEnd?.Invoke(_context.Enemy);
//            }
//            else
//            {
//                Debug.Log("���S��������");
//                OnBattleEnd?.Invoke(null);
//            }
//        }

//        private void DisplayStatus()
//        {
//            var p = _context.Player.Stats;
//            var e = _context.Enemy.Stats;

//            Debug.Log($"[Player] HP:{p.currentHP}/{p.maxHP} MP:{p.currentMP}/{p.maxMP} Shield:{p.shield}");
//            Debug.Log($"[Enemy]  HP:{e.currentHP}/{e.maxHP} MP:{e.currentMP}/{e.maxMP} Shield:{e.shield}");
//        }
//    }

//    // ========================================
//    // AI Strategy ScriptableObject
//    // ========================================

//    [CreateAssetMenu(fileName = "AIStrategy", menuName = "CardBattle/AI/Strategy")]
//    public class AIStrategy : ScriptableObject
//    {
//        [Header("AI�ݒ�")]
//        public string strategyName = "Simple AI";
//        public float aggressiveness = 0.7f;

//        [Header("��p�{�[�h�e���v���[�g")]
//        public List<TacticalBoardEntry> boardTemplate = new List<TacticalBoardEntry>();

//        public void ConfigureCharacter(Character aiCharacter)
//        {
//            aiCharacter.tacticalBoard.Clear();

//            // �e���v���[�g���R�s�[
//            foreach (var entry in boardTemplate)
//            {
//                aiCharacter.tacticalBoard.Add(new TacticalBoardEntry
//                {
//                    entryName = entry.entryName,
//                    condition = entry.condition,
//                    skill = entry.skill,
//                    priority = entry.priority,
//                    isActive = entry.isActive
//                });
//            }
//        }
//    }

//    // ========================================
//    // Character Preset ScriptableObject
//    // ========================================

//    // ========================================
//    // Game Controller (Unity Component)
//    // ========================================

    
    
//    // ========================================
//    // Editor Helper Classes
//    // ========================================
    
//#if UNITY_EDITOR
    
//    [CustomPropertyDrawer(typeof(TacticalBoardEntry))]
//    public class TacticalBoardEntryDrawer : PropertyDrawer
//    {
//        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//        {
//            EditorGUI.BeginProperty(position, label, property);

//            var indent = EditorGUI.indentLevel;
//            EditorGUI.indentLevel = 0;

//            // �e�t�B�[���h�̈ʒu�v�Z
//            float lineHeight = EditorGUIUtility.singleLineHeight;
//            float spacing = EditorGUIUtility.standardVerticalSpacing;
//            float yPos = position.y;

//            // Entry Name
//            var nameRect = new Rect(position.x, yPos, position.width, lineHeight);
//            EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("entryName"), GUIContent.none);
//            yPos += lineHeight + spacing;

//            // Condition & Skill (������)
//            float halfWidth = (position.width - 10) / 2;
//            var conditionRect = new Rect(position.x, yPos, halfWidth, lineHeight);
//            var skillRect = new Rect(position.x + halfWidth + 10, yPos, halfWidth, lineHeight);

//            EditorGUI.PropertyField(conditionRect, property.FindPropertyRelative("condition"), GUIContent.none);
//            EditorGUI.PropertyField(skillRect, property.FindPropertyRelative("skill"), GUIContent.none);
//            yPos += lineHeight + spacing;

//            // Priority & Active (������)
//            var priorityRect = new Rect(position.x, yPos, halfWidth, lineHeight);
//            var activeRect = new Rect(position.x + halfWidth + 10, yPos, halfWidth, lineHeight);

//            EditorGUI.PropertyField(priorityRect, property.FindPropertyRelative("priority"));
//            EditorGUI.PropertyField(activeRect, property.FindPropertyRelative("isActive"));

//            EditorGUI.indentLevel = indent;
//            EditorGUI.EndProperty();
//        }

//        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//        {
//            float lineHeight = EditorGUIUtility.singleLineHeight;
//            float spacing = EditorGUIUtility.standardVerticalSpacing;
//            return (lineHeight + spacing) * 3 + spacing;
//        }
//    }
//#endif
//}