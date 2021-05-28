using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 6.0f;
    public float damagePoints = 10.0f;
    private Vector3 direction;
    private Vector3 [] patrolDirections = {Vector3.right, Vector3.left, Vector3.forward, Vector3.back};
    private bool collidedWithWall = false;
    private GameManager gameManager;
    private PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        direction = patrolDirections[Random.Range(0, 3)];
        gameManager = GameObject.Find("Manager").GetComponent<GameManager>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(collidedWithWall){
            changePatrolDirection();
        }
        transform.Translate(direction * Time.deltaTime * speed);
    }

    void OnCollisionEnter(Collision collider){
        // set collision boolean to true, and move in new direction.
        collidedWithWall = true;

        // if collided with the player, kill him.
        if(collider.gameObject.CompareTag("Player")){
            // reduce player health by number of damage points.
            player.updateHealthBar(damagePoints);

            // if player health is 0, destory the player.
            if(player.playerHealth < 1){
                Destroy(collider.gameObject);

                // Show the gameOver UI.
                bool gameWon = false;
                gameManager.GameOver(gameWon);
            }
            // // Pause Movement.
            // gameManager.PauseGame();
        }
    }

    void OnCollisionExit(Collision coll){
        collidedWithWall = false;
    }

    void changePatrolDirection(){
        // get random number from 0 inclusive to 4 exclusive.
        int directionIndex = Random.Range(0, 4);
        direction = patrolDirections[directionIndex];

    }
}
