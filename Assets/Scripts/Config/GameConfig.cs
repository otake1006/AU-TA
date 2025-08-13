using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "CardBattle/GameConfig")]
[System.Serializable]
public class GameConfig : ScriptableObject
{
    [Header("�Q�[����{�ݒ�")]
    [Tooltip("�ő僉�E���h��")]
    public int maxRounds = 5;

    [Tooltip("1���E���h������̍ő�^�[����")]
    public int maxTurnsPerRound = 10;

    [Tooltip("�����ɕK�v�ȃ��E���h��")]
    public int winsNeededForVictory = 3;

    [Header("�v���C���[�����ݒ�")]
    [Tooltip("����HP")]
    public int initialHP = 100;

    [Tooltip("�����ő�HP")]
    public int initialMaxHP = 100;

    [Tooltip("����MP")]
    public int initialMP = 5;

    [Tooltip("�����ő�MP")]
    public int initialMaxMP = 5;

    [Tooltip("������D����")]
    public int initialHandSize = 3;

    [Header("MP�񕜐ݒ�")]
    [Tooltip("�^�[���J�n����MP�񕜗�")]
    public int mpRecoveryPerTurn = 2;

    [Tooltip("�ő�MP�����𒴂��ĉ񕜉\")]
    public bool canOverchargeMP = false;

    [Tooltip("�I�[�o�[�`���[�W���̍ő�MP�{��")]
    [Range(1.0f, 3.0f)]
    public float maxMPOverchargeMultiplier = 1.5f;

    [Header("HP�񕜐ݒ�")]
    [Tooltip("���E���h�Ԃ�HP�����S������")]
    public bool hpResetBetweenRounds = true;

    [Tooltip("HP���Z�b�g���Ȃ��ꍇ�̉񕜗�")]
    public int hpRecoveryBetweenRounds = 50;

    [Tooltip("���E���h�Ԃł�HP�񕜗ʂ̊����i0-1�j")]
    [Range(0f, 1f)]
    public float hpRecoveryPercentage = 0.5f;

    [Header("���╨�ݒ�")]
    [Tooltip("�s�҂ɐ��╨��t�^")]
    public bool giveRelicToLoser = true;

    [Tooltip("���╨��HP������")]
    public int relicHPBonus = 20;

    [Tooltip("���╨��MP������")]
    public int relicMPBonus = 2;

    [Tooltip("���╨�̌��ʔ{��")]
    [Range(0.5f, 2.0f)]
    public float relicEffectMultiplier = 1.0f;

    [Header("��p�X�L���ݒ�")]
    [Tooltip("�ŃX�L���̃_���[�W")]
    public int poisonDamage = 5;

    [Tooltip("�ŃX�L���̎����^�[��")]
    public int poisonDuration = 3;

    [Tooltip("�Đ��X�L���̉񕜗�")]
    public int regenerationHeal = 8;

    [Tooltip("�Đ��X�L���̎����^�[��")]
    public int regenerationDuration = 5;

    [Tooltip("�����t���U���X�L���̃_���[�W")]
    public int conditionalAttackDamage = 30;

    [Tooltip("�����t���U��������HP臒l�i�����j")]
    [Range(0f, 1f)]
    public float conditionalAttackHPThreshold = 0.25f;

    [Header("�J�[�h�ݒ�")]
    [Tooltip("��{�U���J�[�h�̃_���[�W")]
    public int basicAttackDamage = 15;

    [Tooltip("��{�U���J�[�h��MP�R�X�g")]
    public int basicAttackCost = 2;

    [Tooltip("��{�񕜃J�[�h�̉񕜗�")]
    public int basicHealAmount = 20;

    [Tooltip("��{�񕜃J�[�h��MP�R�X�g")]
    public int basicHealCost = 2;

    [Tooltip("���͂ȍU���J�[�h�̃_���[�W")]
    public int powerAttackDamage = 25;

    [Tooltip("���͂ȍU���J�[�h��MP�R�X�g")]
    public int powerAttackCost = 3;

    [Header("AI�ݒ�")]
    [Tooltip("AI�̎v�l���ԁi�b�j")]
    [Range(0f, 5f)]
    public float aiThinkingTime = 1f;

    [Tooltip("AI�̉�臒l�iHP�����j")]
    [Range(0f, 1f)]
    public float aiHealThreshold = 0.3f;

    [Tooltip("AI�̍U���D��x")]
    [Range(0f, 1f)]
    public float aiAggressionLevel = 0.7f;

    [Header("�o�����X����")]
    [Tooltip("�S�̓I�ȃ_���[�W�{��")]
    [Range(0.1f, 3.0f)]
    public float globalDamageMultiplier = 1.0f;

    [Tooltip("�S�̓I�ȉ񕜔{��")]
    [Range(0.1f, 3.0f)]
    public float globalHealMultiplier = 1.0f;

    [Tooltip("MP�R�X�g�{��")]
    [Range(0.1f, 3.0f)]
    public float mpCostMultiplier = 1.0f;

    [Header("�f�o�b�O�ݒ�")]
    [Tooltip("�ڍ׃��O��\��")]
    public bool enableDetailedLogging = true;

    [Tooltip("��p�{�[�h�̏ڍ׃��O")]
    public bool enableTacticalBoardLogging = true;

    [Tooltip("�^�[���I�����ɏ�ԕ\��")]
    public bool showStateAfterTurn = true;

    // �v�Z���ꂽ�v���p�e�B
    public int CalculatedConditionalAttackThreshold => Mathf.RoundToInt(initialHP * conditionalAttackHPThreshold);
    public int CalculatedMaxMPWithOvercharge => Mathf.RoundToInt(initialMaxMP * maxMPOverchargeMultiplier);
    public int CalculatedHPRecovery => Mathf.RoundToInt(initialHP * hpRecoveryPercentage);

    // �X�P�[�����ꂽ�p�����[�^���擾���郁�\�b�h
    public int GetScaledDamage(int baseDamage) => Mathf.RoundToInt(baseDamage * globalDamageMultiplier);
    public int GetScaledHeal(int baseHeal) => Mathf.RoundToInt(baseHeal * globalHealMultiplier);
    public int GetScaledMPCost(int baseCost) => Mathf.Max(1, Mathf.RoundToInt(baseCost * mpCostMultiplier));
    public int GetScaledRelicEffect(int baseEffect) => Mathf.RoundToInt(baseEffect * relicEffectMultiplier);

    // �v���Z�b�g�ݒ�
    [Header("�v���Z�b�g")]
    [Space(10)]
    [Tooltip("�o�����X�����p�̃v���Z�b�g�{�^��")]
    public bool applyEasyMode = false;
    public bool applyNormalMode = false;
    public bool applyHardMode = false;
    public bool applyTestMode = false;

    void OnValidate()
    {
        if (applyEasyMode)
        {
            ApplyEasyModeSettings();
            applyEasyMode = false;
        }
        else if (applyNormalMode)
        {
            ApplyNormalModeSettings();
            applyNormalMode = false;
        }
        else if (applyHardMode)
        {
            ApplyHardModeSettings();
            applyHardMode = false;
        }
        else if (applyTestMode)
        {
            ApplyTestModeSettings();
            applyTestMode = false;
        }
    }

    void ApplyEasyModeSettings()
    {
        initialHP = 150;
        mpRecoveryPerTurn = 3;
        poisonDamage = 3;
        globalDamageMultiplier = 0.8f;
        globalHealMultiplier = 1.2f;
        aiAggressionLevel = 0.5f;
        Debug.Log("�C�[�W�[���[�h�ݒ��K�p���܂���");
    }

    void ApplyNormalModeSettings()
    {
        initialHP = 100;
        mpRecoveryPerTurn = 2;
        poisonDamage = 5;
        globalDamageMultiplier = 1.0f;
        globalHealMultiplier = 1.0f;
        aiAggressionLevel = 0.7f;
        Debug.Log("�m�[�}�����[�h�ݒ��K�p���܂���");
    }

    void ApplyHardModeSettings()
    {
        initialHP = 80;
        mpRecoveryPerTurn = 1;
        poisonDamage = 8;
        globalDamageMultiplier = 1.3f;
        globalHealMultiplier = 0.8f;
        aiAggressionLevel = 0.9f;
        Debug.Log("�n�[�h���[�h�ݒ��K�p���܂���");
    }

    void ApplyTestModeSettings()
    {
        initialHP = 50;
        maxTurnsPerRound = 5;
        mpRecoveryPerTurn = 5;
        globalDamageMultiplier = 2.0f;
        enableDetailedLogging = true;
        aiThinkingTime = 0.1f;
        Debug.Log("�e�X�g���[�h�ݒ��K�p���܂���");
    }
}