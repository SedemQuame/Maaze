using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(GameObject))]
[RequireComponent(typeof(TMP_Text))]
/// <summary>
/// The game manager class is used for managing all the states of the game, across all scenes.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Tooltip("Menu that shows when button is clicked.")]
    public GameObject gameMenu;
    [Tooltip("Displays the gameover text.")]
    public GameObject gameOver;
    [Tooltip("Displays the gameover state(win or lose).")]
    public TMP_Text gameMessage;
    [Tooltip("Displays the current game level.")]
    public TMP_Text gameLevel;
    private bool isGamePaused;
    public bool isGameOver;

    /// <summary>
    /// Ensures that the game is not paused. Hides the gameOver canvas to false.
    /// </summary>
    void Start()
    {
        isGameOver = false;
        isGamePaused = false;
        gameLevel.text = "Level: " + LevelDifficulty.levelDifficulty;
        gameOver.SetActive(false);
    }

    /// <summary>
    /// Pauses the game, by setting the TimeScale for animations to 0.
    /// </summary>
    public void PauseGame()
    {
        if (isGamePaused)
        {
            Time.timeScale = 1.0f;
        }
        else
        {
            Time.timeScale = 0.0f;
        }
        isGamePaused = !isGamePaused;
    }


    /// <summary>
    /// Displays Main Menu after a game has been won or lost.
    /// </summary>
    public void GameOver(bool gameWon)
    {
        isGameOver = true;
        gameMenu.SetActive(true);
        gameOver.SetActive(true);

        if (gameWon)
        {
            gameMessage.text = "You Win!!";
        }
        else
        {
            gameMessage.text = "You Lose!!";
        }
    }
}