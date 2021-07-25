using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Modifies the value of the player's health bar.
/// </summary>
public class EnemyController : MonoBehaviour
{
    public float speed = 6.0f;
    public float damagePoints = 10.0f;
    [Range(0, 1)]
    public float damageResistance;
    public HealthBarControl healthBarControl;
    public float health = 100.0f;
    private Vector3 direction;
    private Vector3[] patrolDirections = { Vector3.right, Vector3.left, Vector3.forward, Vector3.back };
    private GameManager gameManager;
    private AudioSource audioSource;
    private PlayerController player;
    private EnemyAI enemyAI;
    private float volUp = 1.0f;
    private float volDown = 0.6f;
    public GameObject destroyEffect;
    public AudioClip beatPlayerSound;
    public AudioClip hurtSound;
    public AudioClip dyingSound;
    private bool soundPlayed = false; //used to implement a cool-off time after a sound has been played.
    private bool prefabEffectInstantiated = false;


    // Start is called before the first frame update
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
                // play sound for receiving damage
                audioSource.PlayOneShot(hurtSound, vol);
                soundPlayed = true;

                // start cool-off counter to soundPlayed variable.
                StartCoroutine(coolOffCounter(1.0f, soundPlayed));
            }

            // get gameObject
            // find the damage points from bullet
            float bulletDamage = collider.gameObject.GetComponent<BulletBehaviour>().bulletDamage;

            // subtract damage points from enemy health.
            // update enemy health bar.
            updateHealthBar(bulletDamage);

            if (health < 1)
            {
                // instantiate death particle system.
                DestroyEffect(this.gameObject);

                if(!soundPlayed){
                    // play sound for enemy dying, and soul leaving body.
                    audioSource.PlayOneShot(dyingSound, vol);
                    soundPlayed = true;

                    // start cool-off counter to soundPlayed variable.
                    StartCoroutine(coolOffCounter(0.8f, soundPlayed));
                }

                // destroy enemy game object
                StartCoroutine(destroyEnemyGameObject());
            }
        }

        // if collided with the player, kill him.
        if (collider.gameObject.CompareTag("Player"))
        {
            // play sound for receiving damage
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



    