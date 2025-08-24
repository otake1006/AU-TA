using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

public class DefeatRescueSystem : MonoBehaviour
{
    [Header("Configuration")]
    public GameConfig gameConfig;

    [Header("UI References")]
    public DefeatRescueUI rescueUI;

    // 設定値はGameConfigから取得

    private RelicManager relicManager;
    private bool isRescueInProgress = false;
    private Character rescuedPlayer;

    // イベント
    public System.Action<Character> OnRescueCompleted;
    public System.Action<Character> OnRescueSkipped;

    void Start()
    {
        relicManager = RelicManager.Instance;
        SetupEventListeners();
    }

    void SetupEventListeners()
    {
        GameEvents.OnCharacterDeath += OnCharacterDeath;
    }

    void OnCharacterDeath(Character deadCharacter)
    {
        // プレイヤーが死亡し、救済システムが有効な場合
        if (IsPlayerCharacter(deadCharacter) && CanTriggerRescue(deadCharacter))
        {
            StartCoroutine(TriggerRescueSequence(deadCharacter));
        }
    }

    bool IsPlayerCharacter(Character character)
    {
        // プレイヤーキャラクターかどうかの判定
        return character != null && character.CompareTag("Player");
    }

    bool CanTriggerRescue(Character character)
    {
        if (isRescueInProgress) return false;
        if (!gameConfig.enableRelicSystem) return false;
        if (!gameConfig.enableDefeatRescue) return false;

        // 救済回数制限チェック
        int currentRescueCount = GetCharacterRescueCount(character);
        int maxRescue = gameConfig?.maxRescueCount ?? 1;
        return currentRescueCount < maxRescue;
    }

    IEnumerator TriggerRescueSequence(Character player)
    {
        isRescueInProgress = true;
        rescuedPlayer = player;

        GameEvents.OnDebugMessage?.Invoke($"Triggering rescue for {player.characterName}");

        // 少し待機してから救済UI表示
        yield return new WaitForSeconds(2f);

        // 救済用レリックカードを生成
        List<RelicCard> rescueCards = GenerateRescueCards(player);

        if (rescueCards.Count > 0 && rescueUI != null)
        {
            //rescueUI.ShowRescueOptions(rescueCards, OnRelicSelected, OnRescueSkipped);
        }
        else
        {
            // 救済カードが生成できない場合はスキップ
            CompleteRescueSequence(false);
        }
    }

    List<RelicCard> GenerateRescueCards(Character player)
    {
        var availableRelics = GetAvailableRescueRelics(player);
        var rescueCards = new List<RelicCard>();

        // 指定された数だけランダムに選択
        int cardCount = gameConfig?.rescueRelicCount ?? 3;
        for (int i = 0; i < Mathf.Min(cardCount, availableRelics.Count); i++)
        {
            RelicEffect selectedRelic = SelectRandomRelic(availableRelics);
            if (selectedRelic != null)
            {
                RelicCard rescueCard = relicManager.GenerateRelicCard(selectedRelic);
                rescueCards.Add(rescueCard);

                // 重複を避ける場合は使用済みを削除
                if (!(gameConfig?.allowDuplicateRescueRelics ?? false))
                {
                    availableRelics.Remove(selectedRelic);
                }
            }
        }

        GameEvents.OnDebugMessage?.Invoke($"Generated {rescueCards.Count} rescue cards");
        return rescueCards;
    }

    List<RelicEffect> GetAvailableRescueRelics(Character player)
    {
        var allRelics = relicManager.allRelics;
        var playerRelics = relicManager.GetCharacterRelics(player);

        // プレイヤーが持っていないレリック、または スタック可能なレリックのみ
        var availableRelics = allRelics.Where(relic => 
        {
            if (relic.isStackable) return true;
            return !playerRelics.Any(pr => pr.buffID == relic.buffID);
        }).ToList();

        // 救済に適したレリックをフィルタリング（オプション）
        availableRelics = FilterRescueAppropriateRelics(availableRelics);

        return availableRelics;
    }

    List<RelicEffect> FilterRescueAppropriateRelics(List<RelicEffect> relics)
    {
        // 救済に適したレリック（回復、防御、生存系を優先）
        var rescueAppropriate = relics.Where(relic =>
        {
            return relic.category == RelicCategory.Defense ||
                   relic.category == RelicCategory.Utility ||
                   relic.rarity >= RelicRarity.Uncommon;
        }).ToList();

        // 適切なレリックが少ない場合は全て含める
        int cardCount = gameConfig?.rescueRelicCount ?? 3;
        return rescueAppropriate.Count >= cardCount ? rescueAppropriate : relics;
    }

    RelicEffect SelectRandomRelic(List<RelicEffect> availableRelics)
    {
        if (availableRelics.Count == 0) return null;

        // レアリティによる重み付き選択
        var weights = availableRelics.Select(r => GetRescueRarityWeight(r.rarity)).ToArray();
        int selectedIndex = GetWeightedRandomIndex(weights);

        return availableRelics[selectedIndex];
    }

    float GetRescueRarityWeight(RelicRarity rarity)
    {
        // 救済時はより良いレリックが出やすい
        switch (rarity)
        {
            case RelicRarity.Common: return 30f;
            case RelicRarity.Uncommon: return 40f;
            case RelicRarity.Rare: return 25f;
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

    void OnRelicSelected(RelicCard selectedCard)
    {
        if (selectedCard != null && rescuedPlayer != null)
        {
            GameEvents.OnDebugMessage?.Invoke($"Player selected rescue relic: {selectedCard.cardName}");

            // レリック効果を即座に適用
            ApplyRescueRelic(rescuedPlayer, selectedCard);

            // プレイヤーを復活させる
            RevivePlayer(rescuedPlayer);

            // 救済回数を記録
            IncrementRescueCount(rescuedPlayer);

            CompleteRescueSequence(true);
        }
    }

    void ApplyRescueRelic(Character player, RelicCard selectedCard)
    {
        if (selectedCard.relicEffect != null && relicManager != null)
        {
            // レリックを獲得
            bool acquired = relicManager.AcquireRelic(player, selectedCard.relicEffect);

            if (acquired)
            {
                GameEvents.OnDebugMessage?.Invoke($"{player.characterName} acquired rescue relic: {selectedCard.relicEffect.buffName}");
                
                // 特別な救済効果を適用
                ApplyBonusRescueEffects(player, selectedCard.relicEffect);
            }
        }
    }

    void ApplyBonusRescueEffects(Character player, RelicEffect relic)
    {
        // 救済時の追加効果（HPを設定％回復など）
        float healPercentage = gameConfig?.rescueHealPercentage ?? 0.5f;
        int bonusHeal = Mathf.RoundToInt(player.characterStats.MaxHealth * healPercentage);
        player.characterStats.RestoreHealth(bonusHeal);

        // マナも設定値分回復
        int manaRestore = gameConfig?.rescueManaRestore ?? 5;
        player.characterStats.RestoreMana(manaRestore);

        GameEvents.OnDebugMessage?.Invoke($"Rescue bonus: {player.characterName} healed {bonusHeal}HP and {manaRestore}MP");
        GameEvents.OnHealEffect?.Invoke(player, bonusHeal);
    }

    void RevivePlayer(Character player)
    {
        // プレイヤーを復活状態にする
        player.characterStats.SetAlive(true);
        GameEvents.OnDebugMessage?.Invoke($"{player.characterName} has been revived by divine intervention!");
    }

    //void OnRescueSkipped()
    //{
    //    GameEvents.OnDebugMessage?.Invoke("Player skipped rescue opportunity");
    //    CompleteRescueSequence(false);
    //}

    void CompleteRescueSequence(bool wasRescued)
    {
        isRescueInProgress = false;

        if (wasRescued)
        {
            OnRescueCompleted?.Invoke(rescuedPlayer);
            GameEvents.OnSFXPlay?.Invoke("RescueSuccess");
        }
        else
        {
            OnRescueSkipped?.Invoke(rescuedPlayer);
        }

        rescuedPlayer = null;
    }

    // 救済回数管理
    Dictionary<Character, int> rescueCountDict = new Dictionary<Character, int>();

    int GetCharacterRescueCount(Character character)
    {
        return rescueCountDict.ContainsKey(character) ? rescueCountDict[character] : 0;
    }

    void IncrementRescueCount(Character character)
    {
        if (!rescueCountDict.ContainsKey(character))
        {
            rescueCountDict[character] = 0;
        }
        rescueCountDict[character]++;
    }

    public void ResetRescueCount(Character character)
    {
        if (rescueCountDict.ContainsKey(character))
        {
            rescueCountDict[character] = 0;
        }
    }

    // デバッグ用メソッド
    [ContextMenu("Force Trigger Rescue")]
    public void DebugTriggerRescue()
    {
        if (gameConfig.enableDebugMode)
        {
            var player = FindObjectOfType<Character>();
            if (player != null)
            {
                StartCoroutine(TriggerRescueSequence(player));
            }
        }
    }

    void OnDestroy()
    {
        GameEvents.OnCharacterDeath -= OnCharacterDeath;
    }
}