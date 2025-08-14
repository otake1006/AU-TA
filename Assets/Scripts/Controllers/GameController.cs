using CardBattle.Core;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class GameController : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private GameSettings gameSettings;

    [Header("Player Settings")]
    [SerializeField] private CharacterPreset playerPreset;

    [Header("AI Settings")]
    [SerializeField] private CharacterPreset enemyPreset;
    [SerializeField] private AIStrategy aiStrategy;

    [Header("Addtional Skill/Condition（Test）")]
    [SerializeField] private List<SkillBase> additionalSkills;
    [SerializeField] private List<ConditionBase> additionalConditions;
    [SerializeField] private List<RelicBase> additionalRelics;

    private BattleManager _battleManager;
    private Character _player;
    private Character _enemy;

    [Header("Debug")]
    [SerializeField] private bool autoStartBattle = true;

    void Start()
    {
        if (autoStartBattle)
        {
            InitializeAndStartBattle();
        }
    }

    public void InitializeAndStartBattle()
    {
        if (!ValidateReferences())
        {
            Debug.LogError("必要な参照が設定されていません！");
            return;
        }

        InitializeBattle();
        StartBattle();
    }

    private bool ValidateReferences()
    {
        if (gameSettings == null)
        {
            Debug.LogError("GameSettings が設定されていません");
            return false;
        }

        if (playerPreset == null)
        {
            Debug.LogError("PlayerPreset が設定されていません");
            return false;
        }

        if (enemyPreset == null)
        {
            Debug.LogError("EnemyPreset が設定されていません");
            return false;
        }

        return true;
    }

    void InitializeBattle()
    {
        _battleManager = new BattleManager(gameSettings);

        // プリセットからキャラクター生成
        _player = playerPreset.CreateCharacter(gameSettings);
        _enemy = enemyPreset.CreateCharacter(gameSettings);

        // AIストラテジー適用
        if (aiStrategy != null)
        {
            aiStrategy.ConfigureCharacter(_enemy);
        }

        // イベント登録
        _battleManager.OnBattleEnd += OnBattleEnd;
        _battleManager.OnRoundStart += OnRoundStart;
        _battleManager.OnRoundEnd += OnRoundEnd;
    }

    void StartBattle()
    {
        Debug.Log("=== ゲーム開始 ===");
        _battleManager.StartBattle(_player, _enemy);
    }

    void OnBattleEnd(Character winner)
    {
        if (winner == null)
        {
            Debug.Log("=== 引き分け ===");
            // UI更新や結果画面表示
        }
        else if (winner == _player)
        {
            Debug.Log("=== プレイヤーの勝利！ ===");
            // 勝利演出
        }
        else
        {
            Debug.Log("=== AIの勝利 ===");
            // 敗北演出
        }
    }

    void OnRoundStart(int round)
    {
        Debug.Log($"ラウンド {round} 開始");
        // ラウンド開始演出
    }

    void OnRoundEnd(int round)
    {
        Debug.Log($"ラウンド {round} 終了");
        // ラウンド終了演出
    }

    // エディタ用のテスト機能
    [ContextMenu("Add Sample Entry to Player")]
    void AddSampleEntryToPlayer()
    {
        if (_player == null) return;

        var entry = new TacticalBoardEntry
        {
            entryName = "Test Entry",
            condition = additionalConditions.FirstOrDefault(),
            skill = additionalSkills.FirstOrDefault(),
            priority = 50,
            isActive = true
        };

        _player.tacticalBoard.Add(entry);
        Debug.Log("エントリを追加しました");
    }
}