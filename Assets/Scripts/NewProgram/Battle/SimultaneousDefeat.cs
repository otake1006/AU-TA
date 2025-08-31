using UnityEngine;

public static class SimultaneousDefeat
{
    public static RoundResult DetermineResult(SimultaneousDefeatRule rule, Character player, Character enemy, Character firstToAct)
    {
        switch (rule)
        {
            case SimultaneousDefeatRule.Draw:
                return RoundResult.Draw;

            case SimultaneousDefeatRule.PlayerWins:
                return RoundResult.PlayerWin;

            case SimultaneousDefeatRule.EnemyWins:
                return RoundResult.EnemyWin;

            case SimultaneousDefeatRule.HigherHPWins:
                return DetermineByHPPercentage(player, enemy);

            case SimultaneousDefeatRule.FirstToActWins:
                return DetermineByFirstToAct(player, enemy, firstToAct);

            default:
                return RoundResult.Draw;
        }
    }

    static RoundResult DetermineByHPPercentage(Character player, Character enemy)
    {
        // Œ‚”j‚³‚ê‚é’¼‘O‚ÌHPŠ„‡‚Å”äŠriŠÈˆÕŽÀ‘•j
        float playerHPPercent = (float)player.CurrentHealth / player.maxHealth;
        float enemyHPPercent = (float)enemy.CurrentHealth / enemy.maxHealth;

        if (playerHPPercent > enemyHPPercent)
            return RoundResult.PlayerWin;
        else if (enemyHPPercent > playerHPPercent)
            return RoundResult.EnemyWin;
        else
            return RoundResult.Draw;
    }

    static RoundResult DetermineByFirstToAct(Character player, Character enemy, Character firstToAct)
    {
        if (firstToAct == player)
            return RoundResult.PlayerWin;
        else if (firstToAct == enemy)
            return RoundResult.EnemyWin;
        else
            return RoundResult.Draw;
    }

    public static void Handle(SimultaneousDefeatRule rule, Character player, Character enemy)
    {
        var turnManager = Object.FindObjectOfType<TurnManager>();
        RoundResult result = DetermineResult(rule, player, enemy, turnManager?.FirstToAct);

        GameEvents.OnDebugMessage?.Invoke($"Simultaneous defeat resolved as: {result}");

        var roundManager = Object.FindObjectOfType<RoundManager>();
        roundManager?.EndRound(result);
    }
}
