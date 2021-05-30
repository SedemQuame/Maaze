using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pauses game play
/// </summary>
public class PauseGame : MonoBehaviour
{
    private bool isGamePaused = false;
    public GameObject pauseMenu;

    /// <summary>
    /// Pauses the game, by reducing the TimeScale for animations to 0.
    /// </summary>
    public void pauseGame()
    {
        if (isGamePaused)
        {
            Time.timeScale = 1.0f;
            pauseMenu.SetActive(false);
        }
        else
        {
            Time.timeScale = 0.0f;
            pauseMenu.SetActive(true);
        }
        isGamePaused = !isGamePaused;
    }
}
