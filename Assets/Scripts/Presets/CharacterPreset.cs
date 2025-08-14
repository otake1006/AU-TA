using CardBattle.Core;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterPreset", menuName = "CardBattle/CharacterPreset")]
public class CharacterPreset : ScriptableObject
{
    [Header("�L�����N�^�[���")]
    public string characterName = "New Character";
    public Sprite portrait;

    [Header("�����X�e�[�^�X�iGameSettings�ŏ㏑���j")]
    public int overrideHP = 0;
    public int overrideMP = 0;

    [Header("��p�{�[�h")]
    public List<TacticalBoardEntry> defaultTacticalBoard = new List<TacticalBoardEntry>();

    [Header("���������b�N")]
    public List<RelicBase> startingRelics = new List<RelicBase>();

    public Character CreateCharacter(GameSettings settings)
    {
        int hp = overrideHP > 0 ? overrideHP : settings.startingHP;
        int mp = overrideMP > 0 ? overrideMP : settings.startingMP;

        var character = new Character(characterName, hp, mp)
        {
            portrait = portrait
        };

        // ��p�{�[�h�̃R�s�[
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

        // �����b�N�̃R�s�[
        character.relics.AddRange(startingRelics);

        return character;
    }
}