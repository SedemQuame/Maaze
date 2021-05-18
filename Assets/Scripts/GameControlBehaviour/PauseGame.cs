using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    private bool isGamePaused = false;
    public GameObject pauseMenu;
    // Start is called before the first frame update
    void Start()
    {
        // gameOver.SetActive(false);
    }

    // Pauses the game, by reducing the TimeScale for animations to 0.
    public void pauseGame(){
        if(isGamePaused){
            Time.timeScale = 1.0f;
            Debug.Log("Resume game");
            pauseMenu.SetActive(false);
        }else{
            Time.timeScale = 0.0f;
            Debug.Log("Pause game");
            pauseMenu.SetActive(true);
        }
        isGamePaused = !isGamePaused;
    }
}
