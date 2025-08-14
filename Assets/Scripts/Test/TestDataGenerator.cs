using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CardBattle.Core;

namespace CardBattle.Test
{
    // ========================================
    // �e�X�g�p�̃����^�C���f�[�^�����N���X
    // ========================================

    public class TestDataGenerator : MonoBehaviour
    {
        [Header("�e�X�g�ݒ�")]
        [SerializeField] private bool useScriptableObjects = false;
        [SerializeField] private bool autoStartOnAwake = true;
        [SerializeField] private bool verboseLogging = true;

        [Header("ScriptableObject�Q�Ɓi�I�v�V�����j")]
        [SerializeField] private GameSettings gameSettings;

        private BattleManager _battleManager;
        private Character _player;
        private Character _enemy;

        void Awake()
        {
            if (autoStartOnAwake)
            {
                StartTestBattle();
            }
        }

        [ContextMenu("Start Test Battle")]
        public void StartTestBattle()
        {
            Debug.Log("========================================");
            Debug.Log("=== �J�[�h�o�g�� �e�X�g�J�n ===");
            Debug.Log("========================================");

            // GameSettings����
            if (gameSettings == null)
            {
                gameSettings = CreateTestGameSettings();
            }

            // �L�����N�^�[����
            _player = CreateTestPlayer();
            _enemy = CreateTestEnemy();

            // �o�g���}�l�[�W���[������
            _battleManager = new BattleManager(gameSettings);

            // �C�x���g�o�^
            _battleManager.OnBattleEnd += OnTestBattleEnd;
            _battleManager.OnRoundStart += OnTestRoundStart;
            _battleManager.OnRoundEnd += OnTestRoundEnd;

            // �o�g���J�n
            LogCharacterSetup();
            _battleManager.StartBattle(_player, _enemy);
        }

        private GameSettings CreateTestGameSettings()
        {
            var settings = ScriptableObject.CreateInstance<GameSettings>();
            settings.maxRounds = 3;  // �e�X�g�p��3���E���h�ɒZ�k
            settings.turnsPerRound = 10;
            settings.startingHP = 100;
            settings.startingMP = 50;
            settings.mpRecoveryPerTurn = 10;
            settings.roundEndHPRecover = 10;
            settings.roundEndMPRecover = 20;
            settings.aiThinkTime = 0.1f;
            settings.enableDetailedLogging = verboseLogging;

            Debug.Log($"[Setup] GameSettings����: {settings.maxRounds}���E���h, {settings.turnsPerRound}�^�[��/���E���h");
            return settings;
        }

        private Character CreateTestPlayer()
        {
            var player = new Character("�e�X�g�v���C���[", 100, 50);

            // �X�L���쐬
            var normalAttack = CreateRuntimeDamageSkill("player_normal", "�ʏ�U��", 15, 10);
            var strongAttack = CreateRuntimeDamageSkill("player_strong", "���U��", 30, 20);
            var healSkill = CreateRuntimeHealSkill("player_heal", "��", 25, 15);
            var shieldSkill = CreateRuntimeShieldSkill("player_shield", "�h��", 20, 10);

            // �����쐬
            var alwaysCondition = CreateRuntimeAlwaysCondition();
            var lowEnemyHP = CreateRuntimeHPCondition("enemy_low_hp", "�GHP30%�ȉ�",
                ConditionTarget.Enemy, ComparisonOperator.LessThan, 30f);
            var lowSelfHP = CreateRuntimeHPCondition("self_low_hp", "��HP40%�ȉ�",
                ConditionTarget.Self, ComparisonOperator.LessThan, 40f);
            var highMP = CreateRuntimeMPCondition("high_mp", "MP30�ȏ�",
                ConditionTarget.Self, ComparisonOperator.GreaterOrEqual, 30);

            // ��p�{�[�h�\�z
            player.tacticalBoard.Add(new TacticalBoardEntry
            {
                entryName = "�m������",
                condition = lowSelfHP,
                skill = healSkill,
                priority = 100,  // �ŗD��
                isActive = true
            });

            player.tacticalBoard.Add(new TacticalBoardEntry
            {
                entryName = "�G�m�����K�E",
                condition = lowEnemyHP,
                skill = strongAttack,
                priority = 90,
                isActive = true
            });

            player.tacticalBoard.Add(new TacticalBoardEntry
            {
                entryName = "MP�L�x�����U��",
                condition = highMP,
                skill = strongAttack,
                priority = 70,
                isActive = true
            });

            player.tacticalBoard.Add(new TacticalBoardEntry
            {
                entryName = "�ʏ�U��",
                condition = alwaysCondition,
                skill = normalAttack,
                priority = 50,
                isActive = true
            });

            // �����b�N�ǉ�
            var autoHealRelic = CreateRuntimeHealingRelic("auto_heal", "������", 5, RelicTrigger.TurnEnd);
            var counterRelic = CreateRuntimeCounterRelic("counter", "����", 0.3f);

            player.relics.Add(autoHealRelic);
            player.relics.Add(counterRelic);

            Debug.Log($"[Setup] �v���C���[����: ��p�{�[�h{player.tacticalBoard.Count}��, �����b�N{player.relics.Count}��");

            return player;
        }

        private Character CreateTestEnemy()
        {
            var enemy = new Character("�e�X�gAI�G", 100, 50);

            // AI�X�L���쐬
            var aiNormal = CreateRuntimeDamageSkill("ai_normal", "AI�ʏ�U��", 12, 8);
            var aiStrong = CreateRuntimeDamageSkill("ai_strong", "AI���U��", 25, 18);
            var aiHeal = CreateRuntimeHealSkill("ai_heal", "AI��", 20, 12);
            var aiDrain = CreateRuntimeDrainSkill("ai_drain", "�h���C���U��", 18, 0.5f, 15);

            // AI�����쐬
            var alwaysCondition = CreateRuntimeAlwaysCondition();
            var aiLowHP = CreateRuntimeHPCondition("ai_low_hp", "AI��HP30%�ȉ�",
                ConditionTarget.Self, ComparisonOperator.LessThan, 30f);
            var playerLowHP = CreateRuntimeHPCondition("player_low", "�v���C���[HP40%�ȉ�",
                ConditionTarget.Enemy, ComparisonOperator.LessThan, 40f);
            var turnLate = CreateRuntimeTurnCondition("late_turn", "5�^�[���ڈȍ~",
                ComparisonOperator.GreaterOrEqual, 5);

            // AI��p�{�[�h�\�z�i�D��x�ōs�����ς��j
            enemy.tacticalBoard.Add(new TacticalBoardEntry
            {
                entryName = "AI�m������",
                condition = aiLowHP,
                skill = aiHeal,
                priority = 95,
                isActive = true
            });

            enemy.tacticalBoard.Add(new TacticalBoardEntry
            {
                entryName = "�v���C���[�m�������U��",
                condition = playerLowHP,
                skill = aiStrong,
                priority = 85,
                isActive = true
            });

            enemy.tacticalBoard.Add(new TacticalBoardEntry
            {
                entryName = "�I�Ճh���C��",
                condition = turnLate,
                skill = aiDrain,
                priority = 75,
                isActive = true
            });

            enemy.tacticalBoard.Add(new TacticalBoardEntry
            {
                entryName = "AI�ʏ�U��",
                condition = alwaysCondition,
                skill = aiNormal,
                priority = 50,
                isActive = true
            });

            // AI�����b�N
            var aiMPRegen = CreateRuntimeMPRegenRelic("ai_mp_regen", "AI MP��", 3, RelicTrigger.TurnStart);
            enemy.relics.Add(aiMPRegen);

            Debug.Log($"[Setup] AI�G����: ��p�{�[�h{enemy.tacticalBoard.Count}��, �����b�N{enemy.relics.Count}��");

            return enemy;
        }

        // ========================================
        // �����^�C���X�L������
        // ========================================

        private DamageSkill CreateRuntimeDamageSkill(string id, string name, int damage, int cost)
        {
            var skill = ScriptableObject.CreateInstance<DamageSkill>();
            skill.skillId = id;
            skill.skillName = name;
            skill.description = $"{damage}�_���[�W��^����i�R�X�g: {cost}MP�j";
            skill.baseDamage = damage;
            skill.mpCost = cost;
            skill.targetType = TargetType.Enemy;
            skill.timing = SkillTiming.Instant;
            return skill;
        }

        private HealSkill CreateRuntimeHealSkill(string id, string name, int heal, int cost)
        {
            var skill = ScriptableObject.CreateInstance<HealSkill>();
            skill.skillId = id;
            skill.skillName = name;
            skill.description = $"{heal}HP�񕜂���i�R�X�g: {cost}MP�j";
            skill.baseHealAmount = heal;
            skill.mpCost = cost;
            skill.targetType = TargetType.Self;
            skill.timing = SkillTiming.Instant;
            return skill;
        }

        private ShieldSkill CreateRuntimeShieldSkill(string id, string name, int shield, int cost)
        {
            var skill = ScriptableObject.CreateInstance<ShieldSkill>();
            skill.skillId = id;
            skill.skillName = name;
            skill.description = $"�V�[���h{shield}���l���i�R�X�g: {cost}MP�j";
            skill.shieldAmount = shield;
            skill.mpCost = cost;
            skill.targetType = TargetType.Self;
            return skill;
        }

        private DrainSkill CreateRuntimeDrainSkill(string id, string name, int damage, float drainPercent, int cost)
        {
            var skill = ScriptableObject.CreateInstance<DrainSkill>();
            skill.skillId = id;
            skill.skillName = name;
            skill.description = $"{damage}�_���[�W & {drainPercent * 100}%�z���i�R�X�g: {cost}MP�j";
            skill.baseDamage = damage;
            skill.drainPercent = drainPercent;
            skill.mpCost = cost;
            skill.targetType = TargetType.Enemy;
            return skill;
        }

        // ========================================
        // �����^�C����������
        // ========================================

        private AlwaysCondition CreateRuntimeAlwaysCondition()
        {
            var condition = ScriptableObject.CreateInstance<AlwaysCondition>();
            condition.conditionId = "always";
            condition.conditionName = "�펞";
            condition.description = "��ɏ����𖞂���";
            return condition;
        }

        private HPCondition CreateRuntimeHPCondition(string id, string name,
            ConditionTarget target, ComparisonOperator op, float threshold)
        {
            var condition = ScriptableObject.CreateInstance<HPCondition>();
            condition.conditionId = id;
            condition.conditionName = name;
            condition.checkTarget = target;
            condition.comparison = op;
            condition.hpPercentThreshold = threshold;
            condition.description = $"{target}��HP {op} {threshold}%";
            return condition;
        }

        private MPCondition CreateRuntimeMPCondition(string id, string name,
            ConditionTarget target, ComparisonOperator op, int threshold)
        {
            var condition = ScriptableObject.CreateInstance<MPCondition>();
            condition.conditionId = id;
            condition.conditionName = name;
            condition.checkTarget = target;
            condition.comparison = op;
            condition.mpThreshold = threshold;
            condition.description = $"{target}��MP {op} {threshold}";
            return condition;
        }

        private TurnCondition CreateRuntimeTurnCondition(string id, string name,
            ComparisonOperator op, int turn)
        {
            var condition = ScriptableObject.CreateInstance<TurnCondition>();
            condition.conditionId = id;
            condition.conditionName = name;
            condition.comparison = op;
            condition.turnNumber = turn;
            condition.description = $"�^�[�� {op} {turn}";
            return condition;
        }

        // ========================================
        // �����^�C�������b�N����
        // ========================================

        private HealingRelic CreateRuntimeHealingRelic(string id, string name, int heal, RelicTrigger trigger)
        {
            var relic = ScriptableObject.CreateInstance<HealingRelic>();
            relic.relicId = id;
            relic.relicName = name;
            relic.healAmount = heal;
            relic.trigger = trigger;
            relic.hasUsageLimit = false;
            relic.description = $"{trigger}����{heal}��";
            relic.Initialize();
            return relic;
        }

        private CounterRelic CreateRuntimeCounterRelic(string id, string name, float multiplier)
        {
            var relic = ScriptableObject.CreateInstance<CounterRelic>();
            relic.relicId = id;
            relic.relicName = name;
            relic.counterDamageMultiplier = multiplier;
            relic.trigger = RelicTrigger.OnDamageTaken;
            relic.hasUsageLimit = false;
            relic.description = $"��_���[�W��{multiplier * 100}%�𔽌�";
            relic.Initialize();
            return relic;
        }

        private MPRegenRelic CreateRuntimeMPRegenRelic(string id, string name, int mp, RelicTrigger trigger)
        {
            var relic = ScriptableObject.CreateInstance<MPRegenRelic>();
            relic.relicId = id;
            relic.relicName = name;
            relic.mpRestoreAmount = mp;
            relic.trigger = trigger;
            relic.hasUsageLimit = false;
            relic.description = $"{trigger}����MP{mp}��";
            relic.Initialize();
            return relic;
        }

        // ========================================
        // �f�o�b�O�o��
        // ========================================

        private void LogCharacterSetup()
        {
            Debug.Log("\n=== �L�����N�^�[�ݒ�ڍ� ===");

            // �v���C���[���
            Debug.Log($"\n[{_player.Name}]");
            Debug.Log($"  HP: {_player.Stats.currentHP}/{_player.Stats.maxHP}");
            Debug.Log($"  MP: {_player.Stats.currentMP}/{_player.Stats.maxMP}");
            Debug.Log("  ��p�{�[�h:");
            foreach (var entry in _player.tacticalBoard)
            {
                Debug.Log($"    - [{entry.priority}] {entry.entryName}: " +
                         $"{entry.condition?.conditionName ?? "�펞"} �� {entry.skill?.skillName}");
            }
            Debug.Log("  �����b�N:");
            foreach (var relic in _player.relics)
            {
                Debug.Log($"    - {relic.relicName}: {relic.description}");
            }

            // AI���
            Debug.Log($"\n[{_enemy.Name}]");
            Debug.Log($"  HP: {_enemy.Stats.currentHP}/{_enemy.Stats.maxHP}");
            Debug.Log($"  MP: {_enemy.Stats.currentMP}/{_enemy.Stats.maxMP}");
            Debug.Log("  ��p�{�[�h:");
            foreach (var entry in _enemy.tacticalBoard)
            {
                Debug.Log($"    - [{entry.priority}] {entry.entryName}: " +
                         $"{entry.condition?.conditionName ?? "�펞"} �� {entry.skill?.skillName}");
            }
            Debug.Log("  �����b�N:");
            foreach (var relic in _enemy.relics)
            {
                Debug.Log($"    - {relic.relicName}: {relic.description}");
            }

            Debug.Log("\n========================================\n");
        }

        // ========================================
        // �C�x���g�n���h���[
        // ========================================

        private void OnTestBattleEnd(Character winner)
        {
            Debug.Log("\n========================================");
            if (winner == null)
            {
                Debug.Log("=== �o�g������: �������� ===");
            }
            else if (winner == _player)
            {
                Debug.Log("=== �o�g������: �v���C���[�̏����I ===");
            }
            else
            {
                Debug.Log("=== �o�g������: AI�̏��� ===");
            }

            // �ŏI�X�e�[�^�X
            Debug.Log($"\n�ŏI�X�e�[�^�X:");
            Debug.Log($"  {_player.Name}: HP {_player.Stats.currentHP}/{_player.Stats.maxHP}");
            Debug.Log($"  {_enemy.Name}: HP {_enemy.Stats.currentHP}/{_enemy.Stats.maxHP}");
            Debug.Log("========================================\n");
        }

        private void OnTestRoundStart(int round)
        {
            Debug.Log($"\n������ ���E���h {round} �J�n ������");
        }

        private void OnTestRoundEnd(int round)
        {
            Debug.Log($"������ ���E���h {round} �I�� ������\n");
        }
    }

    // ========================================
    // �ǉ��̃X�L���N���X�i�e�X�g�p�j
    // ========================================

    [CreateAssetMenu(fileName = "ShieldSkill", menuName = "CardBattle/Skills/Shield")]
    public class ShieldSkill : SkillBase
    {
        [Header("�V�[���h�ݒ�")]
        public int shieldAmount = 20;

        public override bool CanExecute(BattleContext context, Character caster)
        {
            return caster.Stats.currentMP >= mpCost;
        }

        public override void Execute(BattleContext context, Character caster, Character target)
        {
            caster.Stats.currentMP -= mpCost;
            target.Stats.shield += shieldAmount;
            context.Log($"{caster.Name} �� {skillName} �𔭓��I �V�[���h +{shieldAmount} (����: {target.Stats.shield})");
        }
    }

    [CreateAssetMenu(fileName = "DrainSkill", menuName = "CardBattle/Skills/Drain")]
    public class DrainSkill : SkillBase
    {
        [Header("�h���C���ݒ�")]
        public int baseDamage = 20;
        [Range(0f, 1f)]
        public float drainPercent = 0.5f;

        public override bool CanExecute(BattleContext context, Character caster)
        {
            return caster.Stats.currentMP >= mpCost;
        }

        public override void Execute(BattleContext context, Character caster, Character target)
        {
            caster.Stats.currentMP -= mpCost;

            target.TakeDamage(baseDamage, context);
            int healAmount = Mathf.RoundToInt(baseDamage * drainPercent);
            caster.Heal(healAmount, context);

            context.Log($"{caster.Name} �� {skillName} �𔭓��I {baseDamage}�_���[�W & {healAmount}�z��");
        }
    }
}