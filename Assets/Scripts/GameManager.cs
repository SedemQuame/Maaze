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
    // ===============PUBLIC VARIABLES===============
    public bool isGameOver, isGamePaused, isObjectManagerDisplayed, isPanelActive;
    [Tooltip("Displays the overlayPanel text.")]
    public GameObject overlayPanel, touchControlPanel, objectManager, worldInformationBox, gameWonPanel, gameLostPanel, gameControls;
    public GameObject pointText, w_score_text, l_score_text;
    [Tooltip("Displays the current game level.")]
    public TMP_Text gameLevel;

    /// <summary>
    /// Ensures that the game is not paused. Hides the gameOver canvas to false.
    /// </summary>
    void Start()
    {
        isGameOver = false;
        isPanelActive = false;
        isGamePaused = false;
        gameLevel.text = "Level: " + LevelDifficulty.levelDifficulty;
        overlayPanel.SetActive(false);
        isObjectManagerDisplayed = false;
        pointText.GetComponent<TextMeshProUGUI>().text = ("00" + PointsSystem.points);
    }

    void FixedUpdate(){
        if (!isObjectManagerDisplayed)
        {
            StartCoroutine(displayObjectiveManager());  
            isObjectManagerDisplayed = true;
        }
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
    /// Settings show panel
    /// </summary>
    public void ShowOtherButtons(){
        if (isPanelActive)
        {
            gameControls.SetActive(false);
        }else{
            gameControls.SetActive(true);
        }
        isPanelActive = !isPanelActive;
    }

    /// <summary>
    /// Displays Main Menu after a game has been won or lost.
    /// </summary>
    public void GameOver(bool gameWon)
    {
        StartCoroutine(showMenu(gameWon));
    }

    IEnumerator showMenu(bool gameWon){
        yield return new WaitForSeconds(2.0f);
        isGameOver = true;
        overlayPanel.SetActive(true);
        touchControlPanel.SetActive(false);
        worldInformationBox.SetActive(false);
        if (gameWon)
        {
            w_score_text.GetComponent<TextMeshProUGUI>().text = "00" + PointsSystem.points;
            gameWonPanel.SetActive(true);
        }
        else
        {
            l_score_text.GetComponent<TextMeshProUGUI>().text = "00" + PointsSystem.points;
            gameLostPanel.SetActive(true);
        }
    }

    IEnumerator displayObjectiveManager(){
        yield return new WaitForSeconds(1.5f);
        
        // show world info box and hide after 10seconds.
        worldInformationBox.SetActive(true);
        StartCoroutine(hideWorldInfoBox());

        objectManager.SetActive(true);

        // display the objective manager after X amount of time.
        objectManager.GetComponent<ObjectiveManager>().populateObjectiveMenu();
    }

    IEnumerator hideWorldInfoBox(){
        yield return new WaitForSeconds(10.0f);
        worldInformationBox.SetActive(false);
    }
}