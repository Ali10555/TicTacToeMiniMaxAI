using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public TicTacToeController Controller;

    public void PlayerVsPlayer()
    {
        Controller.p1Ai = false;
        Controller.p2Ai = false;
        StartGame();
    }

    public void PlayerVsAIEasy()
    {
        Controller.p1Ai = false;
        Controller.p2Ai = true;
        Controller.difficulty = TicTacToeController.AI_DIFFICULTY.easy;
        StartGame();
    }

    public void PlayerVsAIHard()
    {
        Controller.p1Ai = false;
        Controller.p2Ai = true;
        Controller.difficulty = TicTacToeController.AI_DIFFICULTY.hard;
        StartGame();
    }

    public void PlayerVsAIInvincible()
    {
        Controller.p1Ai = false;
        Controller.p2Ai = true;
        Controller.difficulty = TicTacToeController.AI_DIFFICULTY.Invinsible;
        StartGame();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void StartGame()
    {
        Controller.StartGame();
    }
}
