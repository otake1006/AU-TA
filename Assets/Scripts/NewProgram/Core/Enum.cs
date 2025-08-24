// �o�g�����
public enum BattleState
{
    Initializing,      // ��������
    PlayerTurn,        // �v���C���[�^�[��
    EnemyTurn,         // �G�^�[��
    SkillExecution,    // �X�L�����s��
    BuffProcessing,    // �o�t������
    RoundEnd,          // ���E���h�I��
    GameOver           // �Q�[���I��
}

// ���E���h����
public enum RoundResult
{
    PlayerWin,         // �v���C���[����
    EnemyWin,          // �G����
    Draw,              // ��������
    Timeout            // ���Ԑ؂�
}

// �������j���[��
public enum SimultaneousDefeatRule
{
    Draw,              // ���������i���E���h��蒼���j
    PlayerWins,        // �v���C���[����
    EnemyWins,         // �G����
    HigherHPWins,      // �c��HP������������������
    FirstToActWins     // ��ɍs��������������
}

// �X�L�����ʃ^�C�v
public enum SkillEffectType
{
    Damage,            // �_���[�W
    Heal,              // ��
    ApplyBuff,         // �o�t�t�^
    RemoveBuff,        // �o�t����
    Shield,            // �V�[���h
    Stun,              // �X�^��
    Teleport,          // �e���|�[�g
    DrawCard,          // �J�[�h�h���[
    DiscardCard,       // �J�[�h�j��
    ManaRestore,       // �}�i��
    ManaReduce         // �}�i����
}

// �^�[�Q�b�g�^�C�v
public enum TargetType
{
    Self,              // ����
    Enemy,             // �G
    AllEnemies,        // �S�Ă̓G
    AllAllies,         // �S�Ă̖���
    All,               // �S��
    Random             // �����_��
}

// �o�t�^�C�v
public enum BuffType
{
    Buff,              // �L���Ȍ���
    Debuff,            // �s���Ȍ���
    Neutral,           // �����Ȍ���
    Relic              // レリック効果
}

// �o�t�����^�C�~���O
public enum BuffTriggerTiming
{
    TurnStart,         // �^�[���J�n��
    TurnEnd,           // �^�[���I����
    OnDamage,          // �_���[�W���󂯂���
    OnAttack,          // �U����
    OnHeal,            // �񕜎�
    OnCardUse,         // �J�[�h�g�p��
    OnBuffApply,       // �o�t�t�^��
    OnBuffRemove       // �o�t������
}

// �����^�C�v
public enum ConditionType
{
    Health,            // HP�l
    HealthPercentage,  // HP����(%)
    Mana,              // �}�i�l
    ManaPercentage,    // �}�i����(%)
    Attack,            // �U����
    Defense,           // �h���
    Shield,            // �V�[���h�l
    BuffCount,         // �o�t��
    DebuffCount,       // �f�o�t��
    TotalBuffCount,    // �S��Ԉُ퐔
    HandSize,          // ��D����
    TurnNumber         // �^�[����
}

// �����^�[�Q�b�g
public enum ConditionTarget
{
    Self,              // ����
    Enemy              // ����
}

// ��r���Z�q
public enum ComparisonOperator
{
    GreaterThan,       // >
    GreaterThanOrEqual,// >=
    LessThan,          // <
    LessThanOrEqual,   // <=
    Equal,             // ==
    NotEqual           // !=
}

// �_���[�W�^�C�v
public enum DamageType
{
    Normal,            // �ʏ�
    Critical,          // �N���e�B�J��
    Shield,            // �V�[���h
    Poison,            // ��
    Burn,              // �Ώ�
    Magic,             // ���@
    True               // �^�_���[�W�i�h�䖳���j
}

// ���O���x��
public enum LogLevel
{
    Info,              // ���
    Warning,           // �x��
    Error,             // �G���[
    Debug              // �f�o�b�O
}

// AI��Փx
public enum AIDifficulty
{
    Easy,              // �ȒP
    Normal,            // ����
    Hard,              // ���
    Expert             // �G�L�X�p�[�g
}

// �J�[�h���A���e�B
public enum CardRarity
{
    Common,            // �R����
    Uncommon,          // �A���R����
    Rare,              // ���A
    Epic,              // �G�s�b�N
    Legendary          // ���W�F���_���[
}

// カードタイプ
public enum CardType
{
    SkillCard,         // スキルカード
    ConditionalCard,   // 条件付きカード
    RelicCard          // レリックカード
}
