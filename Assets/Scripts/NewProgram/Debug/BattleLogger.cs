using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BattleLogger : MonoBehaviour
{
    public static BattleLogger Instance { get; private set; }

    [Header("Logging Settings")]
    public bool enableLogging = true;
    public bool saveToFile = false;
    public string logFileName = "battle_log";

    private List<BattleLogEntry> battleLog = new List<BattleLogEntry>();

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
        }
    }

    void Start()
    {
        if (enableLogging)
        {
            SetupEventListeners();
        }
    }

    void SetupEventListeners()
    {
        GameEvents.OnRoundEnd += LogRoundEnd;
        GameEvents.OnMatchEnd += LogMatchEnd;
        GameEvents.OnCharacterDeath += LogCharacterDeath;
        GameEvents.OnCardUsed += LogCardUsed;
        GameEvents.OnCharacterDamaged += LogDamage;
        GameEvents.OnCharacterHealed += LogHealing;
    }

    public void LogEvent(string eventType, string description)
    {
        if (!enableLogging) return;

        var entry = new BattleLogEntry
        {
            timestamp = Time.time,
            eventType = eventType,
            description = description
        };

        battleLog.Add(entry);

        if (saveToFile && battleLog.Count % 10 == 0)
        {
            SaveLogToFile();
        }
    }

    void LogRoundEnd(RoundResult result, int round)
    {
        LogEvent("ROUND_END", $"Round {round} ended with result: {result}");
    }

    void LogMatchEnd(Character winner)
    {
        LogEvent("MATCH_END", $"Match ended. Winner: {winner?.characterName ?? "None"}");
    }

    void LogCharacterDeath(Character character)
    {
        LogEvent("CHARACTER_DEATH", $"{character.characterName} was defeated");
    }

    void LogCardUsed(ConditionalSkillCard card, Character caster, Character target)
    {
        LogEvent("CARD_USED", $"{caster.characterName} used {card.cardName} on {target.characterName}");
    }

    void LogDamage(Character character, int damage, DamageType damageType)
    {
        LogEvent("DAMAGE", $"{character.characterName} took {damage} {damageType} damage");
    }

    void LogHealing(Character character, int healing)
    {
        LogEvent("HEALING", $"{character.characterName} healed {healing} HP");
    }

    void SaveLogToFile()
    {
        string fileName = $"{logFileName}_{System.DateTime.Now:yyyyMMdd_HHmmss}.txt";
        string path = Path.Combine(Application.persistentDataPath, fileName);

        var logLines = new List<string>();
        foreach (var entry in battleLog)
        {
            logLines.Add($"[{entry.timestamp:F2}] {entry.eventType}: {entry.description}");
        }

        File.WriteAllLines(path, logLines);
        Debug.Log($"Battle log saved to: {path}");
    }

    [ContextMenu("Export Battle Log")]
    public void ExportBattleLog()
    {
        SaveLogToFile();
    }

    public void ClearLog()
    {
        battleLog.Clear();
        Debug.Log("Battle log cleared");
    }

    void OnDestroy()
    {
        if (enableLogging)
        {
            GameEvents.OnRoundEnd -= LogRoundEnd;
            GameEvents.OnMatchEnd -= LogMatchEnd;
            GameEvents.OnCharacterDeath -= LogCharacterDeath;
            GameEvents.OnCardUsed -= LogCardUsed;
            GameEvents.OnCharacterDamaged -= LogDamage;
            GameEvents.OnCharacterHealed -= LogHealing;
        }
    }
}

[System.Serializable]
public class BattleLogEntry
{
    public float timestamp;
    public string eventType;
    public string description;
}