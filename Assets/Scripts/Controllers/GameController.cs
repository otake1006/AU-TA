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

    [Header("Addtional Skill/Condition�iTest�j")]
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
            Debug.LogError("�K�v�ȎQ�Ƃ��ݒ肳��Ă��܂���I");
            return;
        }

        InitializeBattle();
        StartBattle();
    }

    private bool ValidateReferences()
    {
        if (gameSettings == null)
        {
            Debug.LogError("GameSettings ���ݒ肳��Ă��܂���");
            return false;
        }

        if (playerPreset == null)
        {
            Debug.LogError("PlayerPreset ���ݒ肳��Ă��܂���");
            return false;
        }

        if (enemyPreset == null)
        {
            Debug.LogError("EnemyPreset ���ݒ肳��Ă��܂���");
            return false;
        }

        return true;
    }

    void InitializeBattle()
    {
        _battleManager = new BattleManager(gameSettings);

        // �v���Z�b�g����L�����N�^�[����
        _player = playerPreset.CreateCharacter(gameSettings);
        _enemy = enemyPreset.CreateCharacter(gameSettings);

        // AI�X�g���e�W�[�K�p
        if (aiStrategy != null)
        {
            aiStrategy.ConfigureCharacter(_enemy);
        }

        // �C�x���g�o�^
        _battleManager.OnBattleEnd += OnBattleEnd;
        _battleManager.OnRoundStart += OnRoundStart;
        _battleManager.OnRoundEnd += OnRoundEnd;
    }

    void StartBattle()
    {
        Debug.Log("=== �Q�[���J�n ===");
        _battleManager.StartBattle(_player, _enemy);
    }

    void OnBattleEnd(Character winner)
    {
        if (winner == null)
        {
            Debug.Log("=== �������� ===");
            // UI�X�V�⌋�ʉ�ʕ\��
        }
        else if (winner == _player)
        {
            Debug.Log("=== �v���C���[�̏����I ===");
            // �������o
        }
        else
        {
            Debug.Log("=== AI�̏��� ===");
            // �s�k���o
        }
    }

    void OnRoundStart(int round)
    {
        Debug.Log($"���E���h {round} �J�n");
        // ���E���h�J�n���o
    }

    void OnRoundEnd(int round)
    {
        Debug.Log($"���E���h {round} �I��");
        // ���E���h�I�����o
    }

    // �G�f�B�^�p�̃e�X�g�@�\
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
        Debug.Log("�G���g����ǉ����܂���");
    }
}