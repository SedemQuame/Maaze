using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private float movementX, movementY;
    public float speed = 5.0f;
    private GameObject lastFloor;
    private Rigidbody rigidbody;
    private string lastCreatedCell;
    private GameManager gameManager;

        // Start is called before the first frame update
        void Start()
    {
        GameObject mazeLoader = GameObject.Find("Maze Loader Holder");
        MazeLoader loader = mazeLoader.GetComponent<MazeLoader>();

        lastCreatedCell = "Floor " + --loader.mazeRows + "," + --loader.mazeColumns;
        lastFloor = GameObject.Find(lastCreatedCell);

        rigidbody = GetComponent<Rigidbody>();
        Vector3 playerJumpOffset = new Vector3(-5, 30, -5);
        rigidbody.transform.position = lastFloor.transform.position + playerJumpOffset;

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void FixedUpdate(){
        Vector3 movement = new Vector3(movementX, 0, movementY);
        rigidbody.AddForce(movement * speed * Time.deltaTime, ForceMode.Impulse);
    }


    void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Goal")){
            // Destroy the goal object.
            Destroy(other.gameObject);
            // Display Game Won Menu
            bool gameWon = true;
            gameManager.GameOver(gameWon);
        }
    }

    public void OnMove(InputValue value){
        Vector2 movementVector = value.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }
}
