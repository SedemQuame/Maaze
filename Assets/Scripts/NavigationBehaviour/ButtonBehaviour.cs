using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonBehaviour : MonoBehaviour
{
    private AudioSource source;
    public AudioClip clickSound;

    public void Start(){
        source = GetComponent<AudioSource>();
    }
    public void playClickSound(){
        source.PlayOneShot(clickSound, 0.8f);
    }
    public void loadMainMenuScene(){
        SceneManager.LoadScene("Maaze Main Menu");
    }

    public void loadLevelsMenuScene(){
        SceneManager.LoadScene("Maaze Levels Menu");
    }

    public void loadSettingsScene(){
        SceneManager.LoadScene("Maaze Settings");
    }

    public void loadCreditsMenu(){
        SceneManager.LoadScene("Maaze Creator Credits");
    }

    public void loadGamePlayLevelNumber(){
        SceneManager.LoadScene("Maaze Game Play");
    }

    public void quitGame(){
        Application.Quit();
    }
}
