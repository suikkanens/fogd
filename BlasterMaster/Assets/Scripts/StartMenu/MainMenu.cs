using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void HighScores()
    {
        SceneManager.LoadScene("HighScores");
    }

    public void Minigame()
    {
        SceneManager.LoadScene("Minigame");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
