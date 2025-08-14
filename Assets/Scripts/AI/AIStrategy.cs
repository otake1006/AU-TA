using CardBattle.Core;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AIStrategy", menuName = "CardBattle/AI/Strategy")]
public class AIStrategy : ScriptableObject
{
    [Header("AI設定")]
    public string strategyName = "Simple AI";
    public float aggressiveness = 0.7f;

    [Header("戦術ボードテンプレート")]
    public List<TacticalBoardEntry> boardTemplate = new List<TacticalBoardEntry>();

    public void ConfigureCharacter(Character aiCharacter)
    {
        aiCharacter.tacticalBoard.Clear();

        // テンプレートをコピー
        foreach (var entry in boardTemplate)
        {
            aiCharacter.tacticalBoard.Add(new TacticalBoardEntry
            {
                entryName = entry.entryName,
                condition = entry.condition,
                skill = entry.skill,
                priority = entry.priority,
                isActive = entry.isActive
            });
        }
    }
}