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
    // private bool collidedWithWall = false;
    private GameManager gameManager;
    private PlayerController player;
    private EnemyAI enemyAI;

    public GameObject destroyEffect;


    // Start is called before the first frame update
    void Start()
    {
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
        Debug.Log("Enemy health: " + health);
        healthBarControl.SetHealthBarValue(healthBarControl.GetHealthBarValue() - (0.01f * damagePoints));
    }

    void OnCollisionEnter(Collision collider)
    {
        // // set collision boolean to true, and move in new direction.
        if (collider.gameObject.CompareTag("Bullet"))
        {
            // get gameObject
            // find the damage points.
            // subtract damage points from enemy health.
            // update enemy health bar.
            float bulletDamage = collider.gameObject.GetComponent<BulletBehaviour>().bulletDamage;
            updateHealthBar(bulletDamage);
            if (health < 1)
            {
/*                // instantiate player particle system.
                DestroyEffect();*/

                // Destroy enemy gameObject
                Destroy(transform.gameObject);
            }
        }

        // if collided with the player, kill him.
        if (collider.gameObject.CompareTag("Player"))
        {
            // reduce player health by number of damage points.
            player.updateHealthBar(damagePoints);

            // if player health is 0, destory the player.
            if (player.health < 1)
            {
                // Destroy(collider.gameObject);
                gameManager.PauseGame();

                // Show the gameOver UI.
                bool gameWon = false;
                gameManager.GameOver(gameWon);

                // Destroy enemy gameObject
                // Destroy(transform.gameObject);
                transform.gameObject.SetActive(false);
            }
        }
    }

    void OnDestroy()
    {
/*        DestroyEffect();
*/    }

    public void DestroyEffect()
    {
        Instantiate(destroyEffect, transform.position, transform.rotation);
        // ParticleSystem destroyParticle = destroyEffect.GetComponent<ParticleSystem>();
        // float totalDuration = destroyParticle.duration + destroyParticle.startLifetime;
        // Destroy(destroyEffect, totalDuration);
        DestroyImmediate(destroyEffect, true);

    }
}
