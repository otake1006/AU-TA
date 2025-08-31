using System.Collections;
using UnityEngine;

[System.Serializable]
public abstract class BaseCard : ScriptableObject
{

    [Header("Basic Info")]
    public int cardID;
    public string cardName;
    public string description;
    public Sprite cardImage;
    public CardRarity rarity = CardRarity.Common;

    [Header("Cost")]
    public int manaCost;

    [Header("Animation")]
    public int animationID;
    public float animationDuration = 1f;

    [Header("Effects")]
    public TurnBasedSkillEffect[] effects;

    [Header("Audio")]
    public AudioClip skillSound;

    [Header("Visual")]
    public Color cardColor = Color.white;
    public Sprite cardFrame;

}