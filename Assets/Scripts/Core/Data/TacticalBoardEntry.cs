using System;
using UnityEngine;

[Serializable]
public class TacticalBoardEntry
{
    public string entryName = "New Entry";
    public ConditionBase condition;
    public SkillBase skill;
    [Range(0, 100)]
    public int priority = 50;
    public bool isActive = true;
}