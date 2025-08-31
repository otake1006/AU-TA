using UnityEngine;

[System.Serializable]
public abstract class TurnBasedBuffEffect : ScriptableObject
{
    [Header("Basic Info")]
    public int buffID;
    public string buffName;
    public BuffType buffType;
    public int duration; // ターン数
    public int stackCount = 1;
    public int maxStacks = 1;
    public bool isPermanent = false;
    public Sprite buffIcon;

    [Header("Timing")]
    public BuffTriggerTiming triggerTiming = BuffTriggerTiming.TurnStart;

    [Header("Description")]
    [TextArea(2, 4)]
    public string description;

    public int remainingTurns;
    protected Character target;

    // プロパティ
    public int RemainingTurns => remainingTurns;
    public bool IsExpired => !isPermanent && remainingTurns <= 0;
    public Character Target => target;

    public virtual void Initialize(Character target, int duration = -1)
    {
        this.target = target;
        this.remainingTurns = duration > 0 ? duration : this.duration;
    }

    // 抽象メソッド - 各バフで実装が必要
    public abstract void OnApply();
    public abstract void OnRemove();
    public abstract void OnTurnStart();
    public abstract void OnTurnEnd();

    // 仮想メソッド - 必要に応じてオーバーライド
    public virtual void OnStack()
    {
        stackCount = Mathf.Min(stackCount + 1, maxStacks);
        OnStackEffect();
    }

    protected virtual void OnStackEffect()
    {
        // スタック時の追加効果（オーバーライド可能）
    }

    public virtual void OnDamageTaken(int damage, DamageType damageType)
    {
        // ダメージを受けた時の処理（オーバーライド可能）
    }

    public virtual void OnDamageDealt(int damage, Character target)
    {
        // ダメージを与えた時の処理（オーバーライド可能）
    }

    public virtual void OnHeal(int healAmount)
    {
        // 回復時の処理（オーバーライド可能）
    }

    public virtual void OnCardUsed(ConditionalSkillCard card)
    {
        // カード使用時の処理（オーバーライド可能）
    }

    public void DecreaseDuration()
    {
        if (!isPermanent)
        {
            remainingTurns--;
        }
    }

    // ターンの特定タイミングで効果を発動
    public void TriggerEffect(BuffTriggerTiming timing)
    {
        if (triggerTiming == timing)
        {
            switch (timing)
            {
                case BuffTriggerTiming.TurnStart:
                    OnTurnStart();
                    break;
                case BuffTriggerTiming.TurnEnd:
                    OnTurnEnd();
                    break;
            }
        }
    }

    // バフの詳細情報を取得
    public virtual string GetDetailedDescription()
    {
        string details = $"<b>{buffName}</b>\n";
        details += $"<color={(buffType == BuffType.Buff ? "green" : "red")}>{buffType}</color>\n";

        if (!isPermanent)
            details += $"Duration: {remainingTurns} turns\n";
        else
            details += "Duration: Permanent\n";

        if (maxStacks > 1)
            details += $"Stacks: {stackCount}/{maxStacks}\n";

        details += $"\n{description}";

        return details;
    }

    // バフの効果値を取得（UIでの表示用）
    public virtual int GetEffectValue()
    {
        return 0; // 各バフでオーバーライド
    }

    // バフの優先度を取得（複数バフの表示順序用）
    public virtual int GetPriority()
    {
        switch (buffType)
        {
            case BuffType.Debuff: return 3; // デバフは最優先表示
            case BuffType.Buff: return 2;
            case BuffType.Neutral: return 1;
            default: return 0;
        }
    }

    // デバッグ用
    public override string ToString()
    {
        return $"{buffName} (ID:{buffID}, Stacks:{stackCount}, Turns:{remainingTurns})";
    }
}