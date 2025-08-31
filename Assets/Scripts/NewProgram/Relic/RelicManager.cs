using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RelicManager : MonoBehaviour
{
    private List<RelicEffect> activeRelics = new List<RelicEffect>();
    private Character character;

    // イベント
    public System.Action<List<RelicEffect>> OnRelicListChanged;

    void Start()
    {
        character = GetComponent<Character>();
        SetupEventListeners();
    }

    void SetupEventListeners()
    {
        if (character != null)
        {
            // キャラクターのイベントを監視
            character.OnHealthChanged += (current, max) => {
                float percentage = (float)current / max;
                if (percentage <= 0.3f) // 30%以下で低体力扱い
                {
                    TriggerRelicEvent(RelicTriggerType.LowHealth, percentage);
                }
            };
        }

        // ゲームイベントを監視
        //GameEvents.OnBattleStart += () => TriggerRelicEvent(RelicTriggerType.BattleStart);
        //GameEvents.OnBattleEnd += () => TriggerRelicEvent(RelicTriggerType.BattleEnd);
        //GameEvents.OnCardUsed += () => TriggerRelicEvent(RelicTriggerType.CardPlayed, card);
        GameEvents.OnBuffApplied += (target, buff) => {
            if (target == character)
                TriggerRelicEvent(RelicTriggerType.BuffApplied, buff);
        };
        GameEvents.OnBuffRemoved += (target, buff) => {
            if (target == character)
                TriggerRelicEvent(RelicTriggerType.BuffRemoved, buff);
        };
    }

    public void AcquireRelic(RelicEffect relicEffect)
    {
        if (relicEffect == null) return;

        // 同じレリックが既に存在するかチェック
        var existingRelic = activeRelics.Find(r => r.relicID == relicEffect.relicID);

        if (existingRelic != null)
        {
            // スタック可能な場合は重ね掛け
            if (existingRelic.canStack && existingRelic.stackCount < existingRelic.maxStacks)
            {
                existingRelic.OnStack();
                GameEvents.OnRelicStacked?.Invoke(character, existingRelic);
            }
            else
            {
                Debug.Log($"Cannot stack relic: {relicEffect.relicName}");
                return;
            }
        }
        else
        {
            // 新しいレリックを追加
            var newRelic = Instantiate(relicEffect);
            newRelic.Initialize(character);
            newRelic.OnAcquired();
            activeRelics.Add(newRelic);

            GameEvents.OnRelicAcquired?.Invoke(character, newRelic);
        }

        SortRelics();
        OnRelicListChanged?.Invoke(activeRelics);
        GameEvents.OnRelicListChanged?.Invoke(character);
    }

    public void RemoveRelic(int relicID)
    {
        var relicToRemove = activeRelics.Find(r => r.relicID == relicID);
        if (relicToRemove != null)
        {
            relicToRemove.OnRemoved();
            activeRelics.Remove(relicToRemove);

            GameEvents.OnRelicRemoved?.Invoke(character, relicToRemove);
            OnRelicListChanged?.Invoke(activeRelics);
            GameEvents.OnRelicListChanged?.Invoke(character);
        }
    }

    public void RemoveRelicsByRarity(RelicRarity rarity)
    {
        var relicsToRemove = activeRelics.FindAll(r => r.rarity == rarity);
        foreach (var relic in relicsToRemove)
        {
            relic.OnRemoved();
            activeRelics.Remove(relic);
            GameEvents.OnRelicRemoved?.Invoke(character, relic);
        }

        if (relicsToRemove.Count > 0)
        {
            OnRelicListChanged?.Invoke(activeRelics);
            GameEvents.OnRelicListChanged?.Invoke(character);
        }
    }

    public void ClearAllRelics()
    {
        foreach (var relic in activeRelics)
        {
            relic.OnRemoved();
            GameEvents.OnRelicRemoved?.Invoke(character, relic);
        }
        activeRelics.Clear();
        OnRelicListChanged?.Invoke(activeRelics);
        GameEvents.OnRelicListChanged?.Invoke(character);
    }

    // ターン開始時に呼び出す
    public void OnTurnStart()
    {
        GameEvents.OnDebugMessage?.Invoke($"=== {character.characterName} Relic Turn Start Processing ===");

        foreach (var relic in activeRelics.ToArray())
        {
            relic.OnTurnStart();
        }
    }

    // ターン終了時に呼び出す
    public void OnTurnEnd()
    {
        GameEvents.OnDebugMessage?.Invoke($"=== {character.characterName} Relic Turn End Processing ===");

        foreach (var relic in activeRelics.ToArray())
        {
            relic.OnTurnEnd();
        }
    }

    void TriggerRelicEvent(RelicTriggerType triggerType, params object[] parameters)
    {
        foreach (var relic in activeRelics)
        {
            switch (triggerType)
            {
                case RelicTriggerType.BattleStart:
                    relic.OnBattleStart();
                    break;
                case RelicTriggerType.BattleEnd:
                    relic.OnBattleEnd();
                    break;
                case RelicTriggerType.TurnStart:
                    relic.OnTurnStart();
                    break;
                case RelicTriggerType.TurnEnd:
                    relic.OnTurnEnd();
                    break;
                case RelicTriggerType.CardPlayed:
                    if (parameters.Length > 0 && parameters[0] is ConditionalSkillCard card)
                        relic.OnCardPlayed(card);
                    break;
                case RelicTriggerType.DamageDealt:
                    if (parameters.Length >= 3 && parameters[0] is int damage &&
                        parameters[1] is Character target && parameters[2] is DamageType damageType)
                        relic.OnDamageDealt(damage, target, damageType);
                    break;
                case RelicTriggerType.DamageTaken:
                    if (parameters.Length >= 3 && parameters[0] is int damage2 &&
                        parameters[1] is Character attacker && parameters[2] is DamageType damageType2)
                        relic.OnDamageTaken(damage2, attacker, damageType2);
                    break;
                case RelicTriggerType.HealReceived:
                    if (parameters.Length > 0 && parameters[0] is int healAmount)
                        relic.OnHealReceived(healAmount);
                    break;
                case RelicTriggerType.BuffApplied:
                    if (parameters.Length > 0 && parameters[0] is TurnBasedBuffEffect buff)
                        relic.OnBuffApplied(buff);
                    break;
                case RelicTriggerType.BuffRemoved:
                    if (parameters.Length > 0 && parameters[0] is TurnBasedBuffEffect buff2)
                        relic.OnBuffRemoved(buff2);
                    break;
                case RelicTriggerType.EnemyDefeated:
                    if (parameters.Length > 0 && parameters[0] is Character enemy)
                        relic.OnEnemyDefeated(enemy);
                    break;
                case RelicTriggerType.LowHealth:
                    if (parameters.Length > 0 && parameters[0] is float percentage)
                        relic.OnLowHealth(percentage);
                    break;
            }
        }
    }

    void SortRelics()
    {
        activeRelics.Sort((a, b) => b.GetPriority().CompareTo(a.GetPriority()));
    }

    // 取得メソッド
    public bool HasRelic(int relicID)
    {
        return activeRelics.Exists(r => r.relicID == relicID);
    }

    public RelicEffect GetRelic(int relicID)
    {
        return activeRelics.Find(r => r.relicID == relicID);
    }

    public List<RelicEffect> GetActiveRelics()
    {
        return new List<RelicEffect>(activeRelics);
    }

    public List<RelicEffect> GetRelicsByRarity(RelicRarity rarity)
    {
        return activeRelics.FindAll(r => r.rarity == rarity);
    }

    public int GetRelicCount(RelicRarity rarity)
    {
        return activeRelics.Count(r => r.rarity == rarity);
    }

    public int GetTotalRelicCount()
    {
        return activeRelics.Count;
    }

    // レリックの効果値の合計を取得
    public float GetRelicEffectSum(int relicID)
    {
        var relic = GetRelic(relicID);
        return relic?.GetEffectValue() * relic?.stackCount ?? 0f;
    }

    // デバッグ用
    [ContextMenu("Show Active Relics")]
    public void ShowActiveRelics()
    {
        Debug.Log($"=== {character.characterName} Active Relics ===");
        foreach (var relic in activeRelics)
        {
            Debug.Log(relic.ToString());
        }
    }

    [ContextMenu("Clear All Relics")]
    public void DebugClearAllRelics()
    {
        ClearAllRelics();
    }

    void OnDestroy()
    {
        // クリーンアップ
        foreach (var relic in activeRelics)
        {
            if (relic != null)
                relic.OnRemoved();
        }
        activeRelics.Clear();
    }
}