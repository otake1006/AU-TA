using UnityEngine;
using TacticalCardGame.Core;

public class GameBootstrap : MonoBehaviour
{
    private GameController gameController;

    private void Start()
    {
        gameController = new GameController();
        ShowInstructions();
        gameController.ShowGameState();
    }

    private void Update()
    {
        if (gameController.GameResult != GameResult.Ongoing) return;

        //HandleInput();
    }

    private void HandleInput()
    {
        // ゲーム状態表示
        if (Input.GetKeyDown(KeyCode.S))
        {
            gameController.ShowGameState();
        }
        // ラウンド実行
        else if (Input.GetKeyDown(KeyCode.R))
        {
            gameController.ExecuteRound();
        }
        // プレイヤー1のカード配置 (1-7キー)
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            gameController.PlaceCardOnBoard(1, 0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            gameController.PlaceCardOnBoard(1, 1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            gameController.PlaceCardOnBoard(1, 2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            gameController.PlaceCardOnBoard(1, 3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            gameController.PlaceCardOnBoard(1, 4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            gameController.PlaceCardOnBoard(1, 5);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            gameController.PlaceCardOnBoard(1, 6);
        }
        // プレイヤー2のカード配置 (Q-Uキー)
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            gameController.PlaceCardOnBoard(2, 0);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            gameController.PlaceCardOnBoard(2, 1);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            gameController.PlaceCardOnBoard(2, 2);
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            gameController.PlaceCardOnBoard(2, 3);
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            gameController.PlaceCardOnBoard(2, 4);
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            gameController.PlaceCardOnBoard(2, 5);
        }
    }

    private void ShowInstructions()
    {
        Debug.Log("=== 戦術カードゲーム ===");
        Debug.Log("【勝利条件】相手のHPを0にする");
        Debug.Log("【引き分け】同時HP0 または 5ラウンド経過");
        Debug.Log("");
        Debug.Log("【操作方法】");
        Debug.Log("S - ゲーム状態表示");
        Debug.Log("R - ラウンド実行");
        Debug.Log("1-7 - プレイヤー1 カード配置");
        Debug.Log("Q,W,E,T,Y,U - プレイヤー2 カード配置");
        Debug.Log("=====================");
    }
}