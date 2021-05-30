using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private float movementX, movementY;
    public float speed = 15.0f;
    public float playerHealth = 100.0f;
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

        Vector3 playerJumpOffset = new Vector3(0, 10, 0);
        transform.position = lastFloor.transform.position + playerJumpOffset;

        gameManager = GameObject.Find("Manager").GetComponent<GameManager>();

        // set health bar
        HealthBarControl.SetHealthBarValue(playerHealth * 0.02f);
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0, movementY);
        playerBody.AddForce(movement * speed, ForceMode.Impulse);
        endGameOnPlayerFallOff();
    }

    void endGameOnPlayerFallOff()
    {
        if (transform.position.y < -5)
        {
            gameManager.GameOver(true);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Goal"))
        {
            // Destroy the goal object.
            Destroy(other.gameObject);
            // Display Game Won Menu
            bool gameWon = true;
            gameManager.GameOver(gameWon);
        }
    }

    public void OnMove(InputValue value)
    {
        Vector2 movementVector = value.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    public void updateHealthBar(float damagePoints)
    {
        playerHealth -= damagePoints;
        HealthBarControl.SetHealthBarValue(HealthBarControl.GetHealthBarValue() - (0.01f * damagePoints));
    }
}
