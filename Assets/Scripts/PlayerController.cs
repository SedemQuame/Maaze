using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private float movementX, movementY;
    public float speed = 5.0f;
    private GameObject lastFloor;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        GameObject gameManager = GameObject.Find("GameManager (Maze Loader Holder)");
        MazeLoader loader = gameManager.GetComponent<MazeLoader>();

        string lastCreatedCell = "Floor " + --loader.mazeRows + "," + --loader.mazeColumns;
        lastFloor = GameObject.Find(lastCreatedCell);

        rb = GetComponent<Rigidbody>();
        Vector3 playerJumpOffset = new Vector3(-5, 30, -5);
        rb.transform.position = lastFloor.transform.position + playerJumpOffset;
    }

    void FixedUpdate(){
        Vector3 movement = new Vector3(movementX, 0, movementY);
        rb.AddForce(movement * speed * Time.deltaTime, ForceMode.Impulse);
    }


    void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Goal")){
            // Destroy the goal object.
            Destroy(other.gameObject);
            Debug.Log("Goal reached, game over");
        }

        if(other.gameObject.CompareTag("Enemy")){
            // Destroy the goal object.
            Destroy(gameObject);
            Debug.Log("Player killed, game over");
        }
    }

    public void OnMove(InputValue value){
        Vector2 movementVector = value.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }
}
