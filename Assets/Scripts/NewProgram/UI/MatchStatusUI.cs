using UnityEngine;
using UnityEngine.UI;

public class MatchStatusUI : MonoBehaviour
{
    [Header("UI Components")]
    public Text playerWinsText;
    public Text enemyWinsText;
    public Text currentRoundText;
    public Text winsNeededText;
    public Slider playerWinsSlider;
    public Slider enemyWinsSlider;

    [Header("Visual Elements")]
    public GameObject[] playerWinIndicators;
    public GameObject[] enemyWinIndicators;

    public void UpdateMatchStatus(int playerWins, int enemyWins, int currentRound, int winsNeeded)
    {
        if (playerWinsText != null)
            playerWinsText.text = playerWins.ToString();

        if (enemyWinsText != null)
            enemyWinsText.text = enemyWins.ToString();

        if (currentRoundText != null)
            currentRoundText.text = $"Round {currentRound}";

        if (winsNeededText != null)
            winsNeededText.text = $"First to {winsNeeded}";

        if (playerWinsSlider != null)
        {
            playerWinsSlider.maxValue = winsNeeded;
            playerWinsSlider.value = playerWins;
        }

        if (enemyWinsSlider != null)
        {
            enemyWinsSlider.maxValue = winsNeeded;
            enemyWinsSlider.value = enemyWins;
        }

        // 勝利インジケーターの更新
        UpdateWinIndicators(playerWinIndicators, playerWins);
        UpdateWinIndicators(enemyWinIndicators, enemyWins);
    }

    void UpdateWinIndicators(GameObject[] indicators, int wins)
    {
        if (indicators == null) return;

        for (int i = 0; i < indicators.Length; i++)
        {
            if (indicators[i] != null)
            {
                indicators[i].SetActive(i < wins);
            }
        }
    }
}