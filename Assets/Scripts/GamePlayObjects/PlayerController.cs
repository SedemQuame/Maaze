using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private float movementX, movementY;
    public float speed = 15.0f;
    private float playerHealth = 100.0f;
    private GameObject lastFloor;
    private Rigidbody playerBody;
    private string lastCreatedCell;
    private GameManager gameManager;

        // Start is called before the first frame update
        void Start()
    {
        GameObject mazeLoader = GameObject.Find("Maze Loader Holder");
        MazeLoader loader = mazeLoader.GetComponent<MazeLoader>();

        lastCreatedCell = "Floor " + (loader.getRowAndColumnNumber() - 1) + "," + (loader.getRowAndColumnNumber() - 1);
        lastFloor = GameObject.Find(lastCreatedCell);

        playerBody = GetComponent<Rigidbody>();

        Vector3 playerJumpOffset = new Vector3(-5, 30, -5);
        transform.position = lastFloor.transform.position + playerJumpOffset;

        gameManager = GameObject.Find("Manager").GetComponent<GameManager>();
    }

    void FixedUpdate(){
        Vector3 movement = new Vector3(movementX, 0, movementY);
        playerBody.AddForce(movement * speed, ForceMode.Impulse);
        // playerBody.AddForce(movement * speed * Time.deltaTime, ForceMode.Impulse);
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
