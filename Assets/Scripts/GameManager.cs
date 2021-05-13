using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class GameManager : MonoBehaviour
{
    public GameObject gameOver;
    public GameObject restartButton;
    public Text gameMessage;
    private bool isGamePaused = false;
    // Start is called before the first frame update
    void Start()
    {
        gameOver.SetActive(false);
		restartButton.GetComponent<Button>().onClick.AddListener(RestartGame);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Pauses the game, by reducing the TimeScale for animations to 0.
    public void PauseGame(){
        if(isGamePaused){
            Time.timeScale = 1.0f;
        }else{
            Time.timeScale = 0.0f;
        }
        isGamePaused = !isGamePaused;
    }

    // Displays Main Menu after a game has been won or lost.
	public void GameOver(bool gameWon){
        gameOver.SetActive(true);
        if(gameWon){
            // Set the game over text to "Game Over /n You Win!!"
            gameMessage.text = "You Win!!";
            gameMessage.color = Color.green;
        }else{
            // Set the game over text to "Game Over /n You Lose!!"
            gameMessage.text = "You Lose!!";
            gameMessage.color = Color.red;
        }
    }

	public void RestartGame(){
 		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
