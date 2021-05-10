using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 2.0f;
    private Vector3 direction;
    private Vector3 [] patrolDirections = {Vector3.right, Vector3.left, Vector3.forward, Vector3.back};
    private bool collidedWithWall = false;

    // Start is called before the first frame update
    void Start()
    {
        direction = patrolDirections[Random.Range(0, 3)];
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
            Destroy(collider.gameObject);
            // Upon the destruction of the player, show the gameOver UI.
            GameObject.Find("GameManager (Maze Loader Holder)").GetComponent<MazeLoader>().GameOver();
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
