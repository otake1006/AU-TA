using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CardBattle.Core;

namespace CardBattle.Test
{
    // ========================================
    // テスト用のランタイムデータ生成クラス
    // ========================================

    public class TestDataGenerator : MonoBehaviour
    {
        [Header("テスト設定")]
        [SerializeField] private bool useScriptableObjects = false;
        [SerializeField] private bool autoStartOnAwake = true;
        [SerializeField] private bool verboseLogging = true;

        [Header("ScriptableObject参照（オプション）")]
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
            Debug.Log("=== カードバトル テスト開始 ===");
            Debug.Log("========================================");

            // GameSettings生成
            if (gameSettings == null)
            {
                gameSettings = CreateTestGameSettings();
            }

            // キャラクター生成
            _player = CreateTestPlayer();
            _enemy = CreateTestEnemy();

            // バトルマネージャー初期化
            _battleManager = new BattleManager(gameSettings);

            // イベント登録
            _battleManager.OnBattleEnd += OnTestBattleEnd;
            _battleManager.OnRoundStart += OnTestRoundStart;
            _battleManager.OnRoundEnd += OnTestRoundEnd;

            // バトル開始
            LogCharacterSetup();
            _battleManager.StartBattle(_player, _enemy);
        }

        private GameSettings CreateTestGameSettings()
        {
            var settings = ScriptableObject.CreateInstance<GameSettings>();
            settings.maxRounds = 3;  // テスト用に3ラウンドに短縮
            settings.turnsPerRound = 10;
            settings.startingHP = 100;
            settings.startingMP = 50;
            settings.mpRecoveryPerTurn = 10;
            settings.roundEndHPRecover = 10;
            settings.roundEndMPRecover = 20;
            settings.aiThinkTime = 0.1f;
            settings.enableDetailedLogging = verboseLogging;

            Debug.Log($"[Setup] GameSettings生成: {settings.maxRounds}ラウンド, {settings.turnsPerRound}ターン/ラウンド");
            return settings;
        }

        private Character CreateTestPlayer()
        {
            var player = new Character("テストプレイヤー", 100, 50);

            // スキル作成
            var normalAttack = CreateRuntimeDamageSkill("player_normal", "通常攻撃", 15, 10);
            var strongAttack = CreateRuntimeDamageSkill("player_strong", "強攻撃", 30, 20);
            var healSkill = CreateRuntimeHealSkill("player_heal", "回復", 25, 15);
            var shieldSkill = CreateRuntimeShieldSkill("player_shield", "防御", 20, 10);

            // 条件作成
            var alwaysCondition = CreateRuntimeAlwaysCondition();
            var lowEnemyHP = CreateRuntimeHPCondition("enemy_low_hp", "敵HP30%以下",
                ConditionTarget.Enemy, ComparisonOperator.LessThan, 30f);
            var lowSelfHP = CreateRuntimeHPCondition("self_low_hp", "自HP40%以下",
                ConditionTarget.Self, ComparisonOperator.LessThan, 40f);
            var highMP = CreateRuntimeMPCondition("high_mp", "MP30以上",
                ConditionTarget.Self, ComparisonOperator.GreaterOrEqual, 30);

            // 戦術ボード構築
            player.tacticalBoard.Add(new TacticalBoardEntry
            {
                entryName = "瀕死時回復",
                condition = lowSelfHP,
                skill = healSkill,
                priority = 100,  // 最優先
                isActive = true
            });

            player.tacticalBoard.Add(new TacticalBoardEntry
            {
                entryName = "敵瀕死時必殺",
                condition = lowEnemyHP,
                skill = strongAttack,
                priority = 90,
                isActive = true
            });

            player.tacticalBoard.Add(new TacticalBoardEntry
            {
                entryName = "MP豊富時強攻撃",
                condition = highMP,
                skill = strongAttack,
                priority = 70,
                isActive = true
            });

            player.tacticalBoard.Add(new TacticalBoardEntry
            {
                entryName = "通常攻撃",
                condition = alwaysCondition,
                skill = normalAttack,
                priority = 50,
                isActive = true
            });

            // レリック追加
            var autoHealRelic = CreateRuntimeHealingRelic("auto_heal", "自動回復", 5, RelicTrigger.TurnEnd);
            var counterRelic = CreateRuntimeCounterRelic("counter", "反撃", 0.3f);

            player.relics.Add(autoHealRelic);
            player.relics.Add(counterRelic);

            Debug.Log($"[Setup] プレイヤー生成: 戦術ボード{player.tacticalBoard.Count}個, レリック{player.relics.Count}個");

            return player;
        }

        private Character CreateTestEnemy()
        {
            var enemy = new Character("テストAI敵", 100, 50);

            // AIスキル作成
            var aiNormal = CreateRuntimeDamageSkill("ai_normal", "AI通常攻撃", 12, 8);
            var aiStrong = CreateRuntimeDamageSkill("ai_strong", "AI強攻撃", 25, 18);
            var aiHeal = CreateRuntimeHealSkill("ai_heal", "AI回復", 20, 12);
            var aiDrain = CreateRuntimeDrainSkill("ai_drain", "ドレイン攻撃", 18, 0.5f, 15);

            // AI条件作成
            var alwaysCondition = CreateRuntimeAlwaysCondition();
            var aiLowHP = CreateRuntimeHPCondition("ai_low_hp", "AI自HP30%以下",
                ConditionTarget.Self, ComparisonOperator.LessThan, 30f);
            var playerLowHP = CreateRuntimeHPCondition("player_low", "プレイヤーHP40%以下",
                ConditionTarget.Enemy, ComparisonOperator.LessThan, 40f);
            var turnLate = CreateRuntimeTurnCondition("late_turn", "5ターン目以降",
                ComparisonOperator.GreaterOrEqual, 5);

            // AI戦術ボード構築（優先度で行動が変わる）
            enemy.tacticalBoard.Add(new TacticalBoardEntry
            {
                entryName = "AI瀕死時回復",
                condition = aiLowHP,
                skill = aiHeal,
                priority = 95,
                isActive = true
            });

            enemy.tacticalBoard.Add(new TacticalBoardEntry
            {
                entryName = "プレイヤー瀕死時強攻撃",
                condition = playerLowHP,
                skill = aiStrong,
                priority = 85,
                isActive = true
            });

            enemy.tacticalBoard.Add(new TacticalBoardEntry
            {
                entryName = "終盤ドレイン",
                condition = turnLate,
                skill = aiDrain,
                priority = 75,
                isActive = true
            });

            enemy.tacticalBoard.Add(new TacticalBoardEntry
            {
                entryName = "AI通常攻撃",
                condition = alwaysCondition,
                skill = aiNormal,
                priority = 50,
                isActive = true
            });

            // AIレリック
            var aiMPRegen = CreateRuntimeMPRegenRelic("ai_mp_regen", "AI MP回復", 3, RelicTrigger.TurnStart);
            enemy.relics.Add(aiMPRegen);

            Debug.Log($"[Setup] AI敵生成: 戦術ボード{enemy.tacticalBoard.Count}個, レリック{enemy.relics.Count}個");

            return enemy;
        }

        // ========================================
        // ランタイムスキル生成
        // ========================================

        private DamageSkill CreateRuntimeDamageSkill(string id, string name, int damage, int cost)
        {
            var skill = ScriptableObject.CreateInstance<DamageSkill>();
            skill.skillId = id;
            skill.skillName = name;
            skill.description = $"{damage}ダメージを与える（コスト: {cost}MP）";
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
            skill.description = $"{heal}HP回復する（コスト: {cost}MP）";
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
            skill.description = $"シールド{shield}を獲得（コスト: {cost}MP）";
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
            skill.description = $"{damage}ダメージ & {drainPercent * 100}%吸収（コスト: {cost}MP）";
            skill.baseDamage = damage;
            skill.drainPercent = drainPercent;
            skill.mpCost = cost;
            skill.targetType = TargetType.Enemy;
            return skill;
        }

        // ========================================
        // ランタイム条件生成
        // ========================================

        private AlwaysCondition CreateRuntimeAlwaysCondition()
        {
            var condition = ScriptableObject.CreateInstance<AlwaysCondition>();
            condition.conditionId = "always";
            condition.conditionName = "常時";
            condition.description = "常に条件を満たす";
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
            condition.description = $"{target}のHP {op} {threshold}%";
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
            condition.description = $"{target}のMP {op} {threshold}";
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
            condition.description = $"ターン {op} {turn}";
            return condition;
        }

        // ========================================
        // ランタイムレリック生成
        // ========================================

        private HealingRelic CreateRuntimeHealingRelic(string id, string name, int heal, RelicTrigger trigger)
        {
            var relic = ScriptableObject.CreateInstance<HealingRelic>();
            relic.relicId = id;
            relic.relicName = name;
            relic.healAmount = heal;
            relic.trigger = trigger;
            relic.hasUsageLimit = false;
            relic.description = $"{trigger}時に{heal}回復";
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
            relic.description = $"被ダメージの{multiplier * 100}%を反撃";
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
            relic.description = $"{trigger}時にMP{mp}回復";
            relic.Initialize();
            return relic;
        }

        // ========================================
        // デバッグ出力
        // ========================================

        private void LogCharacterSetup()
        {
            Debug.Log("\n=== キャラクター設定詳細 ===");

            // プレイヤー情報
            Debug.Log($"\n[{_player.Name}]");
            Debug.Log($"  HP: {_player.Stats.currentHP}/{_player.Stats.maxHP}");
            Debug.Log($"  MP: {_player.Stats.currentMP}/{_player.Stats.maxMP}");
            Debug.Log("  戦術ボード:");
            foreach (var entry in _player.tacticalBoard)
            {
                Debug.Log($"    - [{entry.priority}] {entry.entryName}: " +
                         $"{entry.condition?.conditionName ?? "常時"} → {entry.skill?.skillName}");
            }
            Debug.Log("  レリック:");
            foreach (var relic in _player.relics)
            {
                Debug.Log($"    - {relic.relicName}: {relic.description}");
            }

            // AI情報
            Debug.Log($"\n[{_enemy.Name}]");
            Debug.Log($"  HP: {_enemy.Stats.currentHP}/{_enemy.Stats.maxHP}");
            Debug.Log($"  MP: {_enemy.Stats.currentMP}/{_enemy.Stats.maxMP}");
            Debug.Log("  戦術ボード:");
            foreach (var entry in _enemy.tacticalBoard)
            {
                Debug.Log($"    - [{entry.priority}] {entry.entryName}: " +
                         $"{entry.condition?.conditionName ?? "常時"} → {entry.skill?.skillName}");
            }
            Debug.Log("  レリック:");
            foreach (var relic in _enemy.relics)
            {
                Debug.Log($"    - {relic.relicName}: {relic.description}");
            }

            Debug.Log("\n========================================\n");
        }

        // ========================================
        // イベントハンドラー
        // ========================================

        private void OnTestBattleEnd(Character winner)
        {
            Debug.Log("\n========================================");
            if (winner == null)
            {
                Debug.Log("=== バトル結果: 引き分け ===");
            }
            else if (winner == _player)
            {
                Debug.Log("=== バトル結果: プレイヤーの勝利！ ===");
            }
            else
            {
                Debug.Log("=== バトル結果: AIの勝利 ===");
            }

            // 最終ステータス
            Debug.Log($"\n最終ステータス:");
            Debug.Log($"  {_player.Name}: HP {_player.Stats.currentHP}/{_player.Stats.maxHP}");
            Debug.Log($"  {_enemy.Name}: HP {_enemy.Stats.currentHP}/{_enemy.Stats.maxHP}");
            Debug.Log("========================================\n");
        }

        private void OnTestRoundStart(int round)
        {
            Debug.Log($"\n★★★ ラウンド {round} 開始 ★★★");
        }

        private void OnTestRoundEnd(int round)
        {
            Debug.Log($"★★★ ラウンド {round} 終了 ★★★\n");
        }
    }

    // ========================================
    // 追加のスキルクラス（テスト用）
    // ========================================

    [CreateAssetMenu(fileName = "ShieldSkill", menuName = "CardBattle/Skills/Shield")]
    public class ShieldSkill : SkillBase
    {
        [Header("シールド設定")]
        public int shieldAmount = 20;

        public override bool CanExecute(BattleContext context, Character caster)
        {
            return caster.Stats.currentMP >= mpCost;
        }

        public override void Execute(BattleContext context, Character caster, Character target)
        {
            caster.Stats.currentMP -= mpCost;
            target.Stats.shield += shieldAmount;
            context.Log($"{caster.Name} が {skillName} を発動！ シールド +{shieldAmount} (現在: {target.Stats.shield})");
        }
    }

    [CreateAssetMenu(fileName = "DrainSkill", menuName = "CardBattle/Skills/Drain")]
    public class DrainSkill : SkillBase
    {
        [Header("ドレイン設定")]
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

            context.Log($"{caster.Name} が {skillName} を発動！ {baseDamage}ダメージ & {healAmount}吸収");
        }
    }
}