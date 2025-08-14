using CardBattle.Core;
using System.Collections.Generic;
using UnityEngine;

public class BattleContext
{
    public int CurrentRound { get; set; }
    public int CurrentTurn { get; set; }
    public Character Player { get; set; }
    public Character Enemy { get; set; }
    public GameSettings Settings { get; set; }
    public List<string> BattleLog { get; set; } = new List<string>();

    public void Log(string message)
    {
        BattleLog.Add($"[R{CurrentRound}T{CurrentTurn}] {message}");
        if (Settings.enableDetailedLogging)
            Debug.Log($"[Battle] {message}");
    }
}