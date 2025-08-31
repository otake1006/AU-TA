using UnityEngine;
using UnityEngine.UI;

public class TurnIndicatorUI : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject playerTurnIndicator;
    public GameObject enemyTurnIndicator;
    public Text turnNumberText;
    public Text currentPlayerText;
    public Slider turnProgressSlider;

    [Header("Colors")]
    public Color playerTurnColor = Color.blue;
    public Color enemyTurnColor = Color.red;

    public void SetCurrentPlayer(bool isPlayerTurn)
    {
        if (playerTurnIndicator != null)
            playerTurnIndicator.SetActive(isPlayerTurn);

        if (enemyTurnIndicator != null)
            enemyTurnIndicator.SetActive(!isPlayerTurn);

        if (currentPlayerText != null)
        {
            currentPlayerText.text = isPlayerTurn ? "Your Turn" : "Enemy Turn";
            currentPlayerText.color = isPlayerTurn ? playerTurnColor : enemyTurnColor;
        }
    }

    public void SetTurnNumber(int turnNumber)
    {
        if (turnNumberText != null)
            turnNumberText.text = $"Turn {turnNumber}";
    }

    public void SetTurnProgress(int currentTurn, int maxTurns)
    {
        if (turnProgressSlider != null)
        {
            turnProgressSlider.value = (float)currentTurn / maxTurns;
        }
    }
}