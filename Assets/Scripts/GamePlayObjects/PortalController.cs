using System.Collections;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    // ===============PUBLIC VARIABLES===============
    public GameObject player, teleportingFx;
    public AudioClip teleportationSound;
    // ===============PRIVATE VARIABLES===============
    private AudioSource audioSource;
    private GameManager gameManager;
    private int portalPoints = 10;
    private GameObject pointText;

    void Start(){
        player = GameObject.Find("Player");
        gameManager = GameObject.Find("Manager").GetComponent<GameManager>();
        pointText = GameObject.Find("pointText");
        audioSource = this.GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == "Player"){
            // give player X amount of points for using the portal.
            PointsSystem.updatePointSystem(portalPoints, pointText);
            StartCoroutine(gameEndSequence(other.gameObject));
        }

        if(other.gameObject.tag == "Enemy"){
            // teleport enemy gameobjects to next level.
            EnemyType enemyType = other.gameObject.GetComponent<EnemyController>().enemyType;
            Debug.Log(enemyType);
            LevelDifficulty.objectsToTeleport.Add(enemyType);
            StartCoroutine(gameEndSequence(other.gameObject));
        }
        
    }

    IEnumerator gameEndSequence(GameObject gameObject){
        yield return new WaitForSeconds(1.0f);

        // instantiate teleport particle Fx
        Instantiate(teleportingFx, transform.position, transform.rotation);
        // todo play telportation sound
        audioSource.PlayOneShot(teleportationSound);
        yield return new WaitForSeconds(0.2f);
        gameObject.SetActive(false);

        if(gameObject.tag == "Player"){
            yield return new WaitForSeconds(1.0f);
            gameManager.GameOver(true);
        }
    }
}
