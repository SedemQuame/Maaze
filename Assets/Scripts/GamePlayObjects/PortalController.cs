using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    public GameObject player;
    private GameManager gameManager;

    void Start(){
        player = GameObject.Find("Player");
        gameManager = GameObject.Find("Manager").GetComponent<GameManager>();
    }

    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == "Player"){
            StartCoroutine(showGameMenu());
        }
    }

    IEnumerator showGameMenu(){
        yield return new WaitForSeconds(1.0f);
        // Show the gameOver UI.
        bool gameWon = true;
        
        // game over player won.
        gameManager.GameOver(gameWon);

        yield return new WaitForSeconds(1.0f);
        // hide the player
        player.SetActive(false);
    }
}
