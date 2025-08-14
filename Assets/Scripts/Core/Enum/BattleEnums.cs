// File: Assets/CardBattle/Scripts/Core/Enums/BattleEnums.cs

namespace CardBattle.Core
{
    /// <summary>
    /// �X�L�������^�C�~���O
    /// </summary>
    public enum SkillTiming
    {
        Instant,        // ��������
        TurnStart,      // �^�[���J�n��
        TurnEnd,        // �^�[���I����
        RoundStart,     // ���E���h�J�n��
        RoundEnd,       // ���E���h�I����
        OnDamage,       // �_���[�W��
        OnHeal,         // �񕜎�
        OnSkillUse,     // �X�L���g�p��
        OnConditionMet  // ��������������
    }

    /// <summary>
    /// �^�[�Q�b�g�^�C�v
    /// </summary>
    public enum TargetType
    {
        Self,    // ����
        Enemy,   // �G
        Both,    // ����
        Random   // �����_��
    }

    /// <summary>
    /// ��r���Z�q
    /// </summary>
    public enum ComparisonOperator
    {
        GreaterThan,    // >
        LessThan,       // <
        GreaterOrEqual, // >=
        LessOrEqual,    // <=
        Equal,          // ==
        NotEqual        // !=
    }

    /// <summary>
    /// �����`�F�b�N�Ώ�
    /// </summary>
    public enum ConditionTarget
    {
        Self,   // ����
        Enemy,  // �G
        Both    // ����
    }

    /// <summary>
    /// �G�t�F�N�g�^�C�v
    /// </summary>
    public enum EffectType
    {
        Damage,         // �_���[�W
        Heal,           // ��
        MPRestore,      // MP��
        MPDrain,        // MP�z��
        Shield,         // �V�[���h
        AttackBuff,     // �U���o�t
        DefenseBuff,    // �h��o�t
        SpeedBuff,      // ���x�o�t
        AttackDebuff,   // �U���f�o�t
        DefenseDebuff,  // �h��f�o�t
        StatusEffect,   // ��Ԉُ�
        Custom          // �J�X�^��
    }

    /// <summary>
    /// �����b�N�����g���K�[
    /// </summary>
    public enum RelicTrigger
    {
        TurnStart,      // �^�[���J�n��
        TurnEnd,        // �^�[���I����
        RoundStart,     // ���E���h�J�n��
        RoundEnd,       // ���E���h�I����
        OnDamageDealt,  // �_���[�W��^������
        OnDamageTaken,  // �_���[�W���󂯂���
        OnHeal,         // �񕜎�
        OnSkillNotUsed, // �X�L�����g�p��
        OnLowHP,        // HP�ቺ��
        Always          // �펞
    }
}