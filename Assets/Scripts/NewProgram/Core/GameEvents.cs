using System;
using UnityEditor.PackageManager;
using UnityEngine;

public static class GameEvents
{
    // �o�g���C�x���g
    public static Action<RoundResult, int> OnRoundEnd;
    public static Action<Character> OnMatchEnd;
    public static Action<int, int> OnScoreChanged; // playerWins, enemyWins
    public static Action<int> OnTurnChanged;
    public static Action<BattleState> OnBattleStateChanged;

    // �L�����N�^�[�C�x���g
    public static Action<Character> OnCharacterDeath;
    public static Action<Character, int, DamageType> OnCharacterDamaged;
    public static Action<Character, int> OnCharacterHealed;
    public static Action<Character, int> OnManaChanged;
    public static Action<Character, int> OnShieldChanged;

    // �������j�C�x���g
    public static Action<Character, Character> OnSimultaneousDefeat;

    // �J�[�h�C�x���g
    public static Action<ConditionalSkillCard, Character, Character> OnCardUsed;
    public static Action<Character, ConditionalSkillCard> OnCardDrawn;
    public static Action<Character> OnHandUpdated;
    public static Action<RelicCard, Character> OnCardConsumed;

    // �o�t�C�x���g
    public static Action<Character, TurnBasedBuffEffect> OnBuffApplied;
    public static Action<Character, TurnBasedBuffEffect> OnBuffRemoved;
    public static Action<Character> OnBuffListChanged;

    // UI�C�x���g
    public static Action OnUIUpdate;
    public static Action<string> OnNotificationShow;

    // �f�o�b�O�C�x���g
    public static Action<string> OnDebugMessage;
    public static Action<string, LogLevel> OnLogMessage;

    // �I�[�f�B�I�C�x���g
    public static Action<string> OnSFXPlay;
    public static Action<string> OnBGMPlay;
    public static Action<string, Vector3> OnVoicePlay;

    // レリックイベント
    public static Action<Character, RelicEffect> OnRelicAcquired;
    public static Action<Character, RelicEffect> OnRelicLost;
    public static Action<int> OnRoundStart; // ラウンド番号

    // �G�t�F�N�g�C�x���g
    public static Action<string, Vector3> OnEffectPlay;
    public static Action<Vector3, int, DamageType> OnDamageTextShow;
    public static Action<Vector3, int> OnHealTextShow;
    public static Action<Character, int> OnHealEffect;
}