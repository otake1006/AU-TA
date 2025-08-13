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
        // �Q�[����ԕ\��
        if (Input.GetKeyDown(KeyCode.S))
        {
            gameController.ShowGameState();
        }
        // ���E���h���s
        else if (Input.GetKeyDown(KeyCode.R))
        {
            gameController.ExecuteRound();
        }
        // �v���C���[1�̃J�[�h�z�u (1-7�L�[)
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
        // �v���C���[2�̃J�[�h�z�u (Q-U�L�[)
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
        Debug.Log("=== ��p�J�[�h�Q�[�� ===");
        Debug.Log("�y���������z�����HP��0�ɂ���");
        Debug.Log("�y���������z����HP0 �܂��� 5���E���h�o��");
        Debug.Log("");
        Debug.Log("�y������@�z");
        Debug.Log("S - �Q�[����ԕ\��");
        Debug.Log("R - ���E���h���s");
        Debug.Log("1-7 - �v���C���[1 �J�[�h�z�u");
        Debug.Log("Q,W,E,T,Y,U - �v���C���[2 �J�[�h�z�u");
        Debug.Log("=====================");
    }
}