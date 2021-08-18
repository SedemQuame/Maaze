using System.Collections;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    // ===============PUBLIC VARIABLES===============
    public GameObject player, teleportingFx;
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
    
        Instantiate(teleportingFx, transform.position, transform.rotation);
        yield return new WaitForSeconds(0.2f);
        player.SetActive(false);

        yield return new WaitForSeconds(1.0f);
        gameManager.GameOver(true);
    }
}
