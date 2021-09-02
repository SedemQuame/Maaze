using System.Collections;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    // ===============PUBLIC VARIABLES===============
    public GameObject player, teleportingFx;
    // ===============PRIVATE VARIABLES===============
    private GameManager gameManager;
    private int portalPoints = 10;
    private GameObject pointText;

    void Start(){
        player = GameObject.Find("Player");
        gameManager = GameObject.Find("Manager").GetComponent<GameManager>();
        pointText = GameObject.Find("pointText");
    }

    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == "Player"){
            // give player X amount of points for using the portal.
            PointsSystem.updatePointSystem(portalPoints, pointText);
            StartCoroutine(gameEndSequence());
        }
    }

    IEnumerator gameEndSequence(){
        yield return new WaitForSeconds(1.0f);

        // instantiate teleport particle Fx
        Instantiate(teleportingFx, transform.position, transform.rotation);
        yield return new WaitForSeconds(0.2f);
        player.SetActive(false);

        yield return new WaitForSeconds(1.0f);
        gameManager.GameOver(true);
    }
}
