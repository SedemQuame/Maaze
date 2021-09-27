using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Modifies the value of the player's health bar.
/// </summary>
public class EnemyController : MonoBehaviour
{
    // ===============PUBLIC VARIABLES===============
    public float speed = 6.0f, damagePoints = 10.0f, health = 100.0f, killPoints;
    public EnemyType enemyType;
    [Range(0, 1)]
    public float damageResistance;
    public HealthBarControl healthBarControl;
    public GameObject destroyEffect;
    public AudioClip beatPlayerSound, hurtSound, dyingSound;

    // ===============PRIVATE VARIABLES===============
    private Vector3 direction;
    private Vector3[] patrolDirections = { Vector3.right, Vector3.left, Vector3.forward, Vector3.back };
    private GameManager gameManager;
    private GameObject pointText;
    private AudioSource audioSource;
    private PlayerController player;
    private EnemyAI enemyAI;
    private float volUp = 1.0f, volDown = 0.6f;
    //used to implement a cool-off time after a sound has been played.
    private bool soundPlayed = false;
    private bool prefabEffectInstantiated = false;


    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        direction = patrolDirections[Random.Range(0, 4)];
        gameManager = GameObject.Find("Manager").GetComponent<GameManager>();
        if (GameObject.Find("Player") != null)
        {
            player = GameObject.Find("Player").GetComponent<PlayerController>();
        }
        enemyAI = transform.GetComponent<EnemyAI>();
        pointText = GameObject.Find("pointText");
    }

    public void updateHealthBar(float damagePoints)
    {
        health -= damagePoints;
        healthBarControl.SetHealthBarValue(healthBarControl.GetHealthBarValue() - (0.01f * damagePoints));
    }

    void OnCollisionEnter(Collision collider)
    {
        float vol = Random.Range(volDown, volUp);
        // set collision boolean to true, and move in new direction.
        if (collider.gameObject.CompareTag("Bullet"))
        {
            if(!soundPlayed){
                audioSource.PlayOneShot(hurtSound, vol);
                soundPlayed = true;
                StartCoroutine(coolOffCounter(1.0f, soundPlayed));
            }
            float bulletDamage = collider.gameObject.GetComponent<BulletBehaviour>().bulletDamage;// find the bullet damage points
            updateHealthBar(bulletDamage);// subtract damage points from enemy health & update enemy health bar

            if (health < 1)
            {
                // instantiate death particle system.
                DestroyEffect(this.gameObject);
                if(!soundPlayed){
                    // play sound for enemy dying, and soul leaving body.
                    audioSource.PlayOneShot(dyingSound, vol);
                    soundPlayed = true;
                    StartCoroutine(coolOffCounter(0.8f, soundPlayed));
                }
                StartCoroutine(destroyEnemyGameObject());
                // update points system
                PointsSystem.updatePointSystem(killPoints, pointText);
            }
        }

        // if collided with the player, kill him.
        if (collider.gameObject.CompareTag("Player"))
        {
            audioSource.PlayOneShot(beatPlayerSound, vol);
        }
    }

    public void DestroyEffect(GameObject gameObject)
    {
        if (!prefabEffectInstantiated)
        {
            Instantiate(destroyEffect, gameObject.transform.position, gameObject.transform.rotation);
            prefabEffectInstantiated = true;    

            // start cool-off counter to soundPlayed variable.
            StartCoroutine(coolOffCounter(2f, prefabEffectInstantiated));
        }
    }

    IEnumerator destroyEnemyGameObject(){
        yield return new WaitForSeconds(0.5f);
        // Hide the enemy gameObject
        transform.gameObject.SetActive(false);
        // Destroy(this.gameObject);
    }

    IEnumerator coolOffCounter(float coolOffSeconds, bool eventFlag){
        yield return new WaitForSeconds(coolOffSeconds);
        eventFlag = false;
    }
}



    