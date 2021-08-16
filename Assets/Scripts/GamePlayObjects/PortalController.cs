using System.Collections;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    // ===============PUBLIC VARIABLES===============
    public GameObject player;
    // ===============PRIVATE VARIABLES===============
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

        // game over player won.
        gameManager.GameOver(true);

        yield return new WaitForSeconds(1.0f);
        // hide the player
        player.SetActive(false);
    }
}
