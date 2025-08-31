using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TurnBasedBuffManager : MonoBehaviour
{
    private List<TurnBasedBuffEffect> activeBuffs = new List<TurnBasedBuffEffect>();
    private Character character;

    // イベント
    public System.Action<List<TurnBasedBuffEffect>> OnBuffListChanged;

    void Start()
    {
        character = GetComponent<Character>();
        SetupEventListeners();
    }

    void SetupEventListeners()
    {
        // キャラクターのイベントを監視
        if (character != null)
        {
            character.OnHealthChanged += (current, max) => TriggerBuffEvent(BuffTriggerTiming.OnDamage);
        }
    }

    public void ApplyBuff(TurnBasedBuffEffect buffEffect, int duration = -1)
    {
        if (buffEffect == null) return;

        // 同じバフが既に存在するかチェック
        var existingBuff = activeBuffs.Find(b => b.buffID == buffEffect.buffID);

        if (existingBuff != null)
        {
            // スタック可能な場合は重ね掛け
            if (existingBuff.stackCount < existingBuff.maxStacks)
            {
                existingBuff.OnStack();
                existingBuff.remainingTurns = Mathf.Max(existingBuff.remainingTurns,
                                                       duration > 0 ? duration : buffEffect.duration);
            }
            else
            {
                // 持続ターン数をリセット
                existingBuff.remainingTurns = duration > 0 ? duration : buffEffect.duration;
            }
        }
        else
        {
            // 新しいバフを追加
            var newBuff = Instantiate(buffEffect);
            newBuff.Initialize(character, duration);
            newBuff.OnApply();
            activeBuffs.Add(newBuff);

            // イベント発火
            GameEvents.OnBuffApplied?.Invoke(character, newBuff);
        }

        SortBuffs();
        OnBuffListChanged?.Invoke(activeBuffs);
        GameEvents.OnBuffListChanged?.Invoke(character);
    }

    public void RemoveBuff(int buffID)
    {
        var buffToRemove = activeBuffs.Find(b => b.buffID == buffID);
        if (buffToRemove != null)
        {
            buffToRemove.OnRemove();
            activeBuffs.Remove(buffToRemove);

            GameEvents.OnBuffRemoved?.Invoke(character, buffToRemove);
            OnBuffListChanged?.Invoke(activeBuffs);
            GameEvents.OnBuffListChanged?.Invoke(character);
        }
    }

    public void RemoveBuffsByType(BuffType buffType)
    {
        var buffsToRemove = activeBuffs.FindAll(b => b.buffType == buffType);
        foreach (var buff in buffsToRemove)
        {
            buff.OnRemove();
            activeBuffs.Remove(buff);
            GameEvents.OnBuffRemoved?.Invoke(character, buff);
        }

        if (buffsToRemove.Count > 0)
        {
            OnBuffListChanged?.Invoke(activeBuffs);
            GameEvents.OnBuffListChanged?.Invoke(character);
        }
    }

    public void ClearAllBuffs()
    {
        foreach (var buff in activeBuffs)
        {
            buff.OnRemove();
            GameEvents.OnBuffRemoved?.Invoke(character, buff);
        }
        activeBuffs.Clear();
        OnBuffListChanged?.Invoke(activeBuffs);
        GameEvents.OnBuffListChanged?.Invoke(character);
    }

    // ターン開始時に呼び出す
    public void OnTurnStart()
    {
        GameEvents.OnDebugMessage?.Invoke($"=== {character.characterName} Turn Start Buff Processing ===");

        // ターン開始時のバフ効果を発動
        foreach (var buff in activeBuffs.ToArray()) // ToArrayで安全にイテレート
        {
            buff.TriggerEffect(BuffTriggerTiming.TurnStart);
        }

        ProcessExpiredBuffs();
    }

    // ターン終了時に呼び出す
    public void OnTurnEnd()
    {
        GameEvents.OnDebugMessage?.Invoke($"=== {character.characterName} Turn End Buff Processing ===");

        // ターン終了時のバフ効果を発動
        foreach (var buff in activeBuffs.ToArray())
        {
            buff.TriggerEffect(BuffTriggerTiming.TurnEnd);
        }

        // ターン数を減らす
        foreach (var buff in activeBuffs)
        {
            buff.DecreaseDuration();
        }

        ProcessExpiredBuffs();
    }

    void ProcessExpiredBuffs()
    {
        var buffsToRemove = new List<TurnBasedBuffEffect>();

        foreach (var buff in activeBuffs)
        {
            if (buff.IsExpired)
            {
                buffsToRemove.Add(buff);
            }
        }

        // 期限切れのバフを削除
        foreach (var buffToRemove in buffsToRemove)
        {
            buffToRemove.OnRemove();
            activeBuffs.Remove(buffToRemove);
            GameEvents.OnBuffRemoved?.Invoke(character, buffToRemove);
        }

        if (buffsToRemove.Count > 0)
        {
            OnBuffListChanged?.Invoke(activeBuffs);
            GameEvents.OnBuffListChanged?.Invoke(character);
        }
    }

    void TriggerBuffEvent(BuffTriggerTiming timing)
    {
        foreach (var buff in activeBuffs)
        {
            buff.TriggerEffect(timing);
        }
    }

    void SortBuffs()
    {
        // バフを優先度順でソート
        activeBuffs.Sort((a, b) => b.GetPriority().CompareTo(a.GetPriority()));
    }

    // ??方法
    public bool HasBuff(int buffID)
    {
        return activeBuffs.Exists(b => b.buffID == buffID);
    }

    public TurnBasedBuffEffect GetBuff(int buffID)
    {
        return activeBuffs.Find(b => b.buffID == buffID);
    }

    public List<TurnBasedBuffEffect> GetActiveBuffs()
    {
        return new List<TurnBasedBuffEffect>(activeBuffs);
    }

    public List<TurnBasedBuffEffect> GetBuffsByType(BuffType buffType)
    {
        return activeBuffs.FindAll(b => b.buffType == buffType);
    }

    public int GetBuffCount(BuffType buffType)
    {
        return activeBuffs.Count(b => b.buffType == buffType);
    }

    public int GetTotalBuffCount()
    {
        return activeBuffs.Count;
    }

    // バフの効果値の合計を取得
    public int GetBuffEffectSum(int buffID)
    {
        var buff = GetBuff(buffID);
        return buff?.GetEffectValue() * buff?.stackCount ?? 0;
    }

    // デバッグ用
    [ContextMenu("Show Active Buffs")]
    public void ShowActiveBuffs()
    {
        Debug.Log($"=== {character.characterName} Active Buffs ===");
        foreach (var buff in activeBuffs)
        {
            Debug.Log(buff.ToString());
        }
    }

    [ContextMenu("Clear All Buffs")]
    public void DebugClearAllBuffs()
    {
        ClearAllBuffs();
    }

    void OnDestroy()
    {
        // クリーンアップ
        foreach (var buff in activeBuffs)
        {
            if (buff != null)
                buff.OnRemove();
        }
        activeBuffs.Clear();
    }
}
