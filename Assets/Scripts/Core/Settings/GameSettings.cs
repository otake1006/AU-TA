using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "CardBattle/GameSettings")]
public class GameSettings : ScriptableObject
{
    [Header("���E���h�ݒ�")]
    public int maxRounds = 5;
    public int turnsPerRound = 10;

    [Header("�����X�e�[�^�X")]
    public int startingHP = 100;
    public int startingMP = 50;

    [Header("�Q�[�����J�j�N�X")]
    public int mpRecoveryPerTurn = 10;
    public int roundEndHPRecover = 10;
    public int roundEndMPRecover = 20;

    [Header("AI�ݒ�")]
    public float aiThinkTime = 0.5f;

    [Header("�f�o�b�O")]
    public bool enableDetailedLogging = true;
}