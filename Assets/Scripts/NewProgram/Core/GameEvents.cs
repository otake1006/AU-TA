using System;
using UnityEditor.PackageManager;
using UnityEngine;

public static class GameEvents
{
    // バトルイベント
    public static Action<RoundResult, int> OnRoundEnd;
    public static Action<Character> OnMatchEnd;
    public static Action<int, int> OnScoreChanged; // playerWins, enemyWins
    public static Action<int> OnTurnChanged;
    public static Action<BattleState> OnBattleStateChanged;

    // キャラクターイベント
    public static Action<Character> OnCharacterDeath;
    public static Action<Character, int, DamageType> OnCharacterDamaged;
    public static Action<Character, int> OnCharacterHealed;
    public static Action<Character, int> OnManaChanged;
    public static Action<Character, int> OnShieldChanged;

    // 同時撃破イベント
    public static Action<Character, Character> OnSimultaneousDefeat;

    // カードイベント
    public static Action<ConditionalSkillCard, Character, Character> OnCardUsed;
    public static Action<Character, ConditionalSkillCard> OnCardDrawn;
    public static Action<Character> OnHandUpdated;

    // バフイベント
    public static Action<Character, TurnBasedBuffEffect> OnBuffApplied;
    public static Action<Character, TurnBasedBuffEffect> OnBuffRemoved;
    public static Action<Character> OnBuffListChanged;

    // UIイベント
    public static Action OnUIUpdate;
    public static Action<string> OnNotificationShow;

    // デバッグイベント
    public static Action<string> OnDebugMessage;
    public static Action<string, LogLevel> OnLogMessage;

    // オーディオイベント
    public static Action<string> OnSFXPlay;
    public static Action<string> OnBGMPlay;
    public static Action<string, Vector3> OnVoicePlay;

    // エフェクトイベント
    public static Action<string, Vector3> OnEffectPlay;
    public static Action<Vector3, int, DamageType> OnDamageTextShow;
    public static Action<Vector3, int> OnHealTextShow;

    // レリック関連イベント
    public static Action<Character, RelicEffect> OnRelicAcquired;
    public static Action<Character, RelicEffect> OnRelicRemoved;
    public static Action<Character, RelicEffect> OnRelicStacked;
    public static Action<Character> OnRelicListChanged;
}