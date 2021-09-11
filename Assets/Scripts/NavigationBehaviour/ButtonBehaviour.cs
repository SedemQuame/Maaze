using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioClip))]
/// <summary>
/// The ButtonBehaviour class controls all button related behaviour.
/// </summary>
public class ButtonBehaviour : MonoBehaviour
{
/*    [Tooltip("The object responsible for the crossfade effect.")]
    public LevelLoader levelLoader;*/
    [Tooltip("The sound played when a UI element is clicked.")]
    public AudioClip clickSound;
    private AdsManager adsManager;
    /// <summary>
    /// Represents the source that plays the audio sound.
    /// </summary>
    private AudioSource source;
    
    void Awake()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("unity_ads");
        if (objects.Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// Assigns a value to all the private components
    /// </summary>
    public void Start()
    {
        source = GetComponent<AudioSource>();
        adsManager = GameObject.FindGameObjectsWithTag("unity_ads")[0].GetComponent<AdsManager>();
    }

    /// <summary>
    /// Plays an audio clip when any button is clicked.
    /// </summary>
    public void playClickSound()
    {
        source.PlayOneShot(clickSound, 0.8f);
    }

    /// <summary>
    /// Loads the main menu scene, and destroys the old scene.
    /// </summary>
    public void loadMainMenuScene()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Maaze Main Menu");
    }

    /// <summary>
    /// Loads the levels menu scene, and destroys the old scene.
    /// </summary>
    public void loadLevelsMenuScene()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Maaze Levels Menu");
    }   

     /// <summary>
    /// Loads the previous scene if available level menu scene, and destroys the old scene.
    /// </summary>
    public void loadPreviousGamePlayLevelScene()
    {
        Time.timeScale = 1.0f;
        if (LevelDifficulty.levelDifficulty == 1)
        {
            // do nothing
        }
        else
        {
            LevelDifficulty.levelDifficulty -= 1;
            SceneManager.LoadScene("Maaze Game Play");
        }
    }

    public void decideAdToShow(){
        if(LevelDifficulty.levelDifficulty <= 8){
            // do not show ads
            SceneManager.LoadScene("Maaze Game Play");
        }else{
            // show interstitial ads.
            adsManager.PlayAds();
        }
    }

    /// <summary>
    /// Loads the next level menu scene, and destroys the old scene.
    /// </summary>
    public void loadNextGamePlayLevelcene()
    {
        Time.timeScale = 1.0f;
        LevelDifficulty.levelDifficulty += 1;
        if(LevelDifficulty.levelDifficulty > LevelDifficulty.maxLevelReached){
            LevelDifficulty.maxLevelReached = LevelDifficulty.levelDifficulty;
            PlayerPrefs.SetInt("maxLevelReached", LevelDifficulty.maxLevelReached);
        }
        // decided the type of Ad to show
        decideAdToShow();
    }

    /// <summary>
    /// Loads the settings scene, and destroys the old scene.
    /// </summary>
    public void loadSettingsScene()
    {
        SceneManager.LoadScene("Maaze Settings");
    }

    /// <summary>
    /// Loads the credits menu scene, and destroys the old scene.
    /// </summary>
    public void loadCreditsMenu()
    {
        SceneManager.LoadScene("Maaze Creator Credits");
    }

    /// <summary>
    /// Loads the game play using the value of the button clicked as the difficulty.
    /// </summary>
    public void loadGamePlayLevelNumber()
    {
        Time.timeScale = 1.0f;
        // Get the difficulty value from the clicked level button.
        GameObject selectedLevelButton = EventSystem.current.currentSelectedGameObject;
        LevelDifficulty.levelDifficulty = int.Parse(selectedLevelButton.transform.GetChild(0).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text);

        if(LevelDifficulty.levelDifficulty >= LevelDifficulty.maxLevelReached){
            LevelDifficulty.maxLevelReached = LevelDifficulty.levelDifficulty;
            PlayerPrefs.SetInt("maxLevelReached", LevelDifficulty.maxLevelReached);
        }

       if(LevelDifficulty.levelDifficulty >= 33 && LevelDifficulty.levelDifficulty <= 48){
            SceneManager.LoadScene("Maaze Game Play Lighting Dark");
            return;
        }

        // Load the game play scene
        SceneManager.LoadScene("Maaze Game Play");
    }

    /// <summary>
    /// Restarts the game level, with the already selected difficulty.
    /// </summary>
    public void restartGameLevel()
    {
        Time.timeScale = 1.0f;
        Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
    }

    /// <summary>
    /// Quits the game on various devices, this doesn't work in the unity Editor.
    /// </summary>
    public void quitGame()
    {
        Application.Quit();
    }
}
