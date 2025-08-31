using System;
using System.Collections.Generic;

[System.Serializable]
public class BattleResult
{
    public Character winner;
    public Character loser;
    public int totalRounds;
    public int playerWins;
    public int enemyWins;
    public float battleDuration;
    public DateTime battleEndTime;
    public List<RoundData> roundResults;

    public BattleResult()
    {
        roundResults = new List<RoundData>();
        battleEndTime = DateTime.Now;
    }

    public void AddRoundResult(RoundResult result, int roundNumber, float roundDuration)
    {
        roundResults.Add(new RoundData
        {
            roundNumber = roundNumber,
            result = result,
            duration = roundDuration
        });
    }

    public string GetSummary()
    {
        return $"Winner: {winner?.characterName ?? "None"}\n" +
               $"Score: {playerWins}-{enemyWins}\n" +
               $"Total Rounds: {totalRounds}\n" +
               $"Duration: {battleDuration:F1}s";
    }
}

[System.Serializable]
public class RoundData
{
    public int roundNumber;
    public RoundResult result;
    public float duration;
    public int playerHPRemaining;
    public int enemyHPRemaining;
    public int turnsPlayed;
}