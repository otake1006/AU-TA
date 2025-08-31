// バトル状態
public enum BattleState
{
    Initializing,      // 初期化中
    PlayerTurn,        // プレイヤーターン
    EnemyTurn,         // 敵ターン
    SkillExecution,    // スキル実行中
    BuffProcessing,    // バフ処理中
    RoundEnd,          // ラウンド終了
    GameOver           // ゲーム終了
}

// ラウンド結果
public enum RoundResult
{
    PlayerWin,         // プレイヤー勝利
    EnemyWin,          // 敵勝利
    Draw,              // 引き分け
    Timeout            // 時間切れ
}

// 同時撃破ルール
public enum SimultaneousDefeatRule
{
    Draw,              // 引き分け（ラウンドやり直し）
    PlayerWins,        // プレイヤー勝利
    EnemyWins,         // 敵勝利
    HigherHPWins,      // 残りHP割合が高い方が勝利
    FirstToActWins     // 先に行動した方が勝利
}

// スキル効果タイプ
public enum SkillEffectType
{
    Damage,            // ダメージ
    Heal,              // 回復
    ApplyBuff,         // バフ付与
    RemoveBuff,        // バフ解除
    Shield,            // シールド
    Stun,              // スタン
    Teleport,          // テレポート
    DrawCard,          // カードドロー
    DiscardCard,       // カード破棄
    ManaRestore,       // マナ回復
    ManaReduce         // マナ減少
}

// ターゲットタイプ
public enum TargetType
{
    Self,              // 自分
    Enemy,             // 敵
    AllEnemies,        // 全ての敵
    AllAllies,         // 全ての味方
    All,               // 全て
    Random             // ランダム
}

// バフタイプ
public enum BuffType
{
    Buff,              // 有利な効果
    Debuff,            // 不利な効果
    Neutral            // 中立な効果
}

// バフ発動タイミング
public enum BuffTriggerTiming
{
    TurnStart,         // ターン開始時
    TurnEnd,           // ターン終了時
    OnDamage,          // ダメージを受けた時
    OnAttack,          // 攻撃時
    OnHeal,            // 回復時
    OnCardUse,         // カード使用時
    OnBuffApply,       // バフ付与時
    OnBuffRemove       // バフ解除時
}

// 条件タイプ
public enum ConditionType
{
    Health,            // HP値
    HealthPercentage,  // HP割合(%)
    Mana,              // マナ値
    ManaPercentage,    // マナ割合(%)
    Attack,            // 攻撃力
    Defense,           // 防御力
    Shield,            // シールド値
    BuffCount,         // バフ数
    DebuffCount,       // デバフ数
    TotalBuffCount,    // 全状態異常数
    HandSize,          // 手札枚数
    TurnNumber         // ターン数
}

// 条件ターゲット
public enum ConditionTarget
{
    Self,              // 自分
    Enemy              // 相手
}

// 比較演算子
public enum ComparisonOperator
{
    GreaterThan,       // >
    GreaterThanOrEqual,// >=
    LessThan,          // <
    LessThanOrEqual,   // <=
    Equal,             // ==
    NotEqual           // !=
}

// ダメージタイプ
public enum DamageType
{
    Normal,            // 通常
    Critical,          // クリティカル
    Shield,            // シールド
    Poison,            // 毒
    Burn,              // 火傷
    Magic,             // 魔法
    True               // 真ダメージ（防御無視）
}

// ログレベル
public enum LogLevel
{
    Info,              // 情報
    Warning,           // 警告
    Error,             // エラー
    Debug              // デバッグ
}

// AI難易度
public enum AIDifficulty
{
    Easy,              // 簡単
    Normal,            // 普通
    Hard,              // 難しい
    Expert             // エキスパート
}

// カードレアリティ
public enum CardRarity
{
    Common,            // コモン
    Uncommon,          // アンコモン
    Rare,              // レア
    Epic,              // エピック
    Legendary          // レジェンダリー
}
