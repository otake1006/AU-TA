using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RelicManager : MonoBehaviour
{
    [Header("Configuration")]
    public GameConfig gameConfig;

    [Header("Relic Database")]
    public List<RelicEffect> allRelics = new List<RelicEffect>();
    public List<RelicCard> allRelicCards = new List<RelicCard>();

    // アクティブなレリック管理
    private Dictionary<Character, List<RelicEffect>> characterRelics = new Dictionary<Character, List<RelicEffect>>();

    // イベント
    public System.Action<Character, RelicEffect> OnRelicAcquired;
    public System.Action<Character, RelicEffect> OnRelicLost;
    public System.Action<Character, List<RelicEffect>> OnRelicListChanged;

    // シングルトン（オプション）
    public static RelicManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeRelicDatabase();
        SetupEventListeners();
    }

    void InitializeRelicDatabase()
    {
        // リソースからすべてのレリックを読み込み
        if (allRelics.Count == 0)
        {
            allRelics.AddRange(Resources.LoadAll<RelicEffect>("Relics"));
        }

        if (allRelicCards.Count == 0)
        {
            allRelicCards.AddRange(Resources.LoadAll<RelicCard>("Cards/Relics"));
        }

        GameEvents.OnDebugMessage?.Invoke($"RelicManager initialized with {allRelics.Count} relics and {allRelicCards.Count} relic cards");
    }

    void SetupEventListeners()
    {
        GameEvents.OnRelicAcquired += OnRelicAcquiredEvent;
        GameEvents.OnMatchEnd += OnMatchEndEvent;
        GameEvents.OnBattleStateChanged += OnBattleStateChangedEvent;
        GameEvents.OnRoundStart += OnRoundStartEvent;
    }

    // レリック獲得
    public bool AcquireRelic(Character character, RelicEffect relic)
    {
        if (character == null || relic == null) return false;

        // キャラクターのレリックリストを初期化
        if (!characterRelics.ContainsKey(character))
        {
            characterRelics[character] = new List<RelicEffect>();
        }

        var relicList = characterRelics[character];

        // スタック不可能で既に持っている場合はfalse
        if (!relic.isStackable && HasRelic(character, relic.buffID))
        {
            GameEvents.OnDebugMessage?.Invoke($"Character already has non-stackable relic: {relic.buffName}");
            return false;
        }

        // 獲得条件チェック
        if (!relic.CanAcquire(character))
        {
            GameEvents.OnDebugMessage?.Invoke($"Cannot acquire relic {relic.buffName}: conditions not met");
            return false;
        }

        // バフマネージャーにレリックを適用
        var buffManager = character.GetComponent<TurnBasedBuffManager>();
        if (buffManager != null)
        {
            buffManager.ApplyBuff(relic);
        }

        // レリックリストに追加
        relicList.Add(relic);
        relic.OnRelicAcquired();

        // イベント発火
        OnRelicAcquired?.Invoke(character, relic);
        GameEvents.OnRelicAcquired?.Invoke(character, relic);
        OnRelicListChanged?.Invoke(character, relicList);

        GameEvents.OnDebugMessage?.Invoke($"{character.characterName} acquired relic: {relic.buffName}");
        return true;
    }

    // レリック削除
    public bool RemoveRelic(Character character, int relicID)
    {
        if (character == null || !characterRelics.ContainsKey(character)) return false;

        var relicList = characterRelics[character];
        var relicToRemove = relicList.Find(r => r.buffID == relicID);

        if (relicToRemove == null) return false;

        // バフマネージャーからも削除
        var buffManager = character.GetComponent<TurnBasedBuffManager>();
        if (buffManager != null)
        {
            buffManager.RemoveBuff(relicID);
        }

        relicToRemove.OnRelicLost();
        relicList.Remove(relicToRemove);

        // イベント発火
        OnRelicLost?.Invoke(character, relicToRemove);
        GameEvents.OnRelicLost?.Invoke(character, relicToRemove);
        OnRelicListChanged?.Invoke(character, relicList);

        GameEvents.OnDebugMessage?.Invoke($"{character.characterName} lost relic: {relicToRemove.buffName}");
        return true;
    }

    // レリック所持チェック
    public bool HasRelic(Character character, int relicID)
    {
        if (!characterRelics.ContainsKey(character)) return false;
        return characterRelics[character].Any(r => r.buffID == relicID);
    }

    // レリック取得
    public RelicEffect GetRelic(Character character, int relicID)
    {
        if (!characterRelics.ContainsKey(character)) return null;
        return characterRelics[character].Find(r => r.buffID == relicID);
    }

    // キャラクターの全レリック取得
    public List<RelicEffect> GetCharacterRelics(Character character)
    {
        if (!characterRelics.ContainsKey(character)) return new List<RelicEffect>();
        return new List<RelicEffect>(characterRelics[character]);
    }

    // レアリティ別レリック取得
    public List<RelicEffect> GetRelicsByRarity(RelicRarity rarity)
    {
        return allRelics.Where(r => r.rarity == rarity).ToList();
    }

    // カテゴリ別レリック取得
    public List<RelicEffect> GetRelicsByCategory(RelicCategory category)
    {
        return allRelics.Where(r => r.category == category).ToList();
    }

    // ランダムレリック取得（重み付き）
    public RelicEffect GetRandomRelic(RelicRarity? targetRarity = null)
    {
        List<RelicEffect> candidates = targetRarity.HasValue 
            ? GetRelicsByRarity(targetRarity.Value) 
            : allRelics;

        if (candidates.Count == 0) return null;

        // レアリティによる重み付け
        float[] weights = candidates.Select(r => GetRarityWeight(r.rarity)).ToArray();
        int selectedIndex = GetWeightedRandomIndex(weights);

        return candidates[selectedIndex];
    }

    float GetRarityWeight(RelicRarity rarity)
    {
        switch (rarity)
        {
            case RelicRarity.Common: return 50f;
            case RelicRarity.Uncommon: return 30f;
            case RelicRarity.Rare: return 15f;
            case RelicRarity.Epic: return 4f;
            case RelicRarity.Legendary: return 1f;
            default: return 1f;
        }
    }

    int GetWeightedRandomIndex(float[] weights)
    {
        float totalWeight = weights.Sum();
        float randomValue = Random.Range(0f, totalWeight);
        
        float currentWeight = 0f;
        for (int i = 0; i < weights.Length; i++)
        {
            currentWeight += weights[i];
            if (randomValue <= currentWeight)
                return i;
        }
        
        return weights.Length - 1;
    }

    // レリックカードの生成
    public RelicCard GenerateRelicCard(RelicEffect relic)
    {
        var relicCard = allRelicCards.Find(card => card.relicEffect?.buffID == relic.buffID);
        
        if (relicCard != null)
        {
            return Instantiate(relicCard);
        }

        // 動的にレリックカードを作成
        var dynamicCard = ScriptableObject.CreateInstance<RelicCard>();
        dynamicCard.relicEffect = relic;
        dynamicCard.cardName = relic.buffName;
        dynamicCard.description = relic.description;
        dynamicCard.manaCost = relic.acquisitionCost;
        
        return dynamicCard;
    }

    // イベントハンドラ
    void OnRelicAcquiredEvent(Character character, RelicEffect relic)
    {
        // 追加の処理が必要な場合
    }

    void OnMatchEndEvent(Character winner)
    {
        // 全キャラクターのレリックに対してマッチ終了イベント発火
        foreach (var kvp in characterRelics)
        {
            foreach (var relic in kvp.Value)
            {
                relic.OnMatchEnd(winner);
            }
        }
    }

    void OnBattleStateChangedEvent(BattleState newState)
    {
        if (newState == BattleState.Initializing)
        {
            // 戦闘開始時の処理
            foreach (var kvp in characterRelics)
            {
                foreach (var relic in kvp.Value)
                {
                    relic.OnBattleStart();
                }
            }
        }
    }

    void OnRoundStartEvent(int roundNumber)
    {
        // ラウンド開始時の処理
        foreach (var kvp in characterRelics)
        {
            foreach (var relic in kvp.Value)
            {
                relic.OnRoundStart();
            }
        }
    }

    // デバッグ用メソッド
    [ContextMenu("Show All Character Relics")]
    public void ShowAllCharacterRelics()
    {
        foreach (var kvp in characterRelics)
        {
            Debug.Log($"=== {kvp.Key.characterName} Relics ===");
            foreach (var relic in kvp.Value)
            {
                Debug.Log($"  {relic.buffName} (ID: {relic.buffID}, Rarity: {relic.rarity})");
            }
        }
    }

    public void ClearCharacterRelics(Character character)
    {
        if (characterRelics.ContainsKey(character))
        {
            var relics = characterRelics[character].ToList();
            foreach (var relic in relics)
            {
                RemoveRelic(character, relic.buffID);
            }
        }
    }

    void OnDestroy()
    {
        // イベントリスナー削除
        GameEvents.OnRelicAcquired -= OnRelicAcquiredEvent;
        GameEvents.OnMatchEnd -= OnMatchEndEvent;
        GameEvents.OnBattleStateChanged -= OnBattleStateChangedEvent;
        GameEvents.OnRoundStart -= OnRoundStartEvent;
    }
}