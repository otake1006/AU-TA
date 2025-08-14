using CardBattle.Core;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterPreset", menuName = "CardBattle/CharacterPreset")]
public class CharacterPreset : ScriptableObject
{
    [Header("キャラクター情報")]
    public string characterName = "New Character";
    public Sprite portrait;

    [Header("初期ステータス（GameSettingsで上書き可）")]
    public int overrideHP = 0;
    public int overrideMP = 0;

    [Header("戦術ボード")]
    public List<TacticalBoardEntry> defaultTacticalBoard = new List<TacticalBoardEntry>();

    [Header("初期レリック")]
    public List<RelicBase> startingRelics = new List<RelicBase>();

    public Character CreateCharacter(GameSettings settings)
    {
        int hp = overrideHP > 0 ? overrideHP : settings.startingHP;
        int mp = overrideMP > 0 ? overrideMP : settings.startingMP;

        var character = new Character(characterName, hp, mp)
        {
            portrait = portrait
        };

        // 戦術ボードのコピー
        foreach (var entry in defaultTacticalBoard)
        {
            character.tacticalBoard.Add(new TacticalBoardEntry
            {
                entryName = entry.entryName,
                condition = entry.condition,
                skill = entry.skill,
                priority = entry.priority,
                isActive = entry.isActive
            });
        }

        // レリックのコピー
        character.relics.AddRange(startingRelics);

        return character;
    }
}