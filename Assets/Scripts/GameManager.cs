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
    public GameObject pointText, w_score_text, l_score_text, w_message, l_message, timer;
    [Tooltip("Displays the current game level.")]
    public AudioClip [] audioClip;
    public TMP_Text gameLevel;
    private int levelTimeLimit = 60;
    private PlayerController playerController;
    private AudioSource audioSource;

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
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        audioSource = this.GetComponent<AudioSource>();
        if(LevelDifficulty.levelDifficulty >= 17 && LevelDifficulty.levelDifficulty <= 32){
            timer.SetActive(true);
            // initialise timer values.
            timer.GetComponent<Timer>().SetDuration(levelTimeLimit)
                .OnEnd (() => playerController.killPlayerOnTimeOut())
                .Begin ();
        }
    }

    void FixedUpdate(){
            // except for level #4, where objectiveManager will be displayed when first enemy is spawned.
        if (!isObjectManagerDisplayed && LevelDifficulty.levelDifficulty != 4)
        {
            showObjectivePanel();
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

    public void showObjectivePanel(){
        StartCoroutine(displayObjectiveManager());  
        isObjectManagerDisplayed = true;
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
            string message = "Cleared!";
            w_score_text.GetComponent<TextMeshProUGUI>().text = "00" + PointsSystem.points;

            if(LevelDifficulty.levelDifficulty%8 == 0){
                message = "Congratulations, Section\n" + message;
            }else{
                message = "Level, " + message;
            }

            w_message.GetComponent<TextMeshProUGUI>().text = message;

            gameWonPanel.SetActive(true);

            // play game win sounds.
            audioSource.PlayOneShot(audioClip[0]);
        }
        else
        {
            l_score_text.GetComponent<TextMeshProUGUI>().text = "00" + PointsSystem.points;
            gameLostPanel.SetActive(true);

            // play game lost sounds.
            audioSource.PlayOneShot(audioClip[1]);
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