using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ButtonBehaviour : MonoBehaviour
{
    private AudioSource source;
    public AudioClip clickSound;

    public void Start()
    {
        source = GetComponent<AudioSource>();
    }
    public void playClickSound()
    {
        source.PlayOneShot(clickSound, 0.8f);
    }
    public void loadMainMenuScene()
    {
        SceneManager.LoadScene("Maaze Main Menu");
    }

    public void loadLevelsMenuScene()
    {
        SceneManager.LoadScene("Maaze Levels Menu");
    }

    public void loadSettingsScene()
    {
        SceneManager.LoadScene("Maaze Settings");
    }

    public void loadCreditsMenu()
    {
        SceneManager.LoadScene("Maaze Creator Credits");
    }

    public void loadGamePlayLevelNumber()
    {
        GameObject selectedLevelButton = EventSystem.current.currentSelectedGameObject;

        // Get the difficulty value from the clicked level button.
        LevelDifficulty.levelDifficulty = int.Parse(selectedLevelButton.transform.GetChild(0).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text);
        SceneManager.LoadScene("Maaze Game Play");
    }

    public void restartGameLevel()
    {
        Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
