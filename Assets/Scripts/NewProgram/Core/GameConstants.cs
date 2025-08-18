using UnityEngine;

public static class GameConstants
{
    // デフォルト値
    public const int DEFAULT_MAX_HEALTH = 100;
    public const int DEFAULT_MAX_MANA = 50;
    public const int DEFAULT_ATTACK = 10;
    public const int DEFAULT_DEFENSE = 5;

    // UI定数
    public const float CARD_ANIMATION_DURATION = 0.5f;
    public const float DAMAGE_TEXT_FADE_TIME = 2f;
    public const float BUFF_ICON_SIZE = 32f;
    public const float UI_FADE_DURATION = 0.3f;

    // バランス調整
    public const float DAMAGE_MULTIPLIER_BASE = 0.1f;
    public const int MAX_BUFF_STACKS = 5;
    public const int MAX_DEBUFF_STACKS = 3;
    public const float CRITICAL_CHANCE_BASE = 0.05f;

    // ファイルパス
    public const string CONFIG_PATH = "GameConfig";
    public const string SAVE_PATH = "BattleData";
    public const string LOG_PATH = "Logs";

    // プレハブパス
    public const string CARD_PREFAB_PATH = "Prefabs/Cards/";
    public const string EFFECT_PREFAB_PATH = "Prefabs/Effects/";
    public const string UI_PREFAB_PATH = "Prefabs/UI/";

    // タグ
    public const string PLAYER_TAG = "Player";
    public const string ENEMY_TAG = "Enemy";
    public const string UI_TAG = "UI";
    public const string CARD_TAG = "Card";

    // レイヤー
    public const int UI_LAYER = 5;
    public const int EFFECT_LAYER = 8;
    public const int CARD_LAYER = 9;

    // 色定数
    public static readonly Color PLAYER_COLOR = new Color(0.2f, 0.6f, 1f);
    public static readonly Color ENEMY_COLOR = new Color(1f, 0.3f, 0.2f);
    public static readonly Color NEUTRAL_COLOR = new Color(0.7f, 0.7f, 0.7f);
    public static readonly Color BUFF_COLOR = new Color(0.2f, 0.8f, 0.2f);
    public static readonly Color DEBUFF_COLOR = new Color(0.8f, 0.2f, 0.2f);

    // アニメーション定数
    public const string ANIM_ATTACK = "Attack";
    public const string ANIM_DAMAGED = "Damaged";
    public const string ANIM_HEAL = "Heal";
    public const string ANIM_DEATH = "Death";
    public const string ANIM_IDLE = "Idle";

    // サウンド定数
    public const string SFX_CARD_PLAY = "CardPlay";
    public const string SFX_DAMAGE = "Damage";
    public const string SFX_HEAL = "Heal";
    public const string SFX_BUFF = "Buff";
    public const string SFX_DEBUFF = "Debuff";
}