using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioClip))]
public class PlayerController : MonoBehaviour
{
    [Tooltip("Forward speed of the player game object.")]
    // [Range(3, 12)]
    [SerializeField]
    private float speed;

    [Tooltip("Health value of the player")]
    public float health = 100.0f;
    [Tooltip("The sound played when player collides with a given game object.")]
    public GameObject bulletPrefab;
    public AudioClip playerShootingSound;
    public AudioClip playerMovingSound;
    public HealthBarControl healthBarControl;
    public GameObject nozzel;
    [Tooltip("Variable joy stick used to control player movement.")]
    // public VariableJoystick variableJoystick;
    public GameObject variableJoystick;
    public GameObject spawnManager;
    /// <summary>
    /// Represents the source that plays the audio sound.
    /// </summary>
    public AudioClip shootingSound;
    private Rigidbody playerBody;
    private GameManager gameManager;
    private GameObject lastFloor;
    private string lastCreatedCell;
    private float movementX, movementY;
    private bool isColliding;
    private bool hasHitGround;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        isColliding = false;
        hasHitGround = false;

        speed = 0.8f;

        audioSource = this.GetComponent<AudioSource>();

        GameObject mazeLoader = GameObject.Find("Maze Loader Holder");
        MazeLoader loader = mazeLoader.GetComponent<MazeLoader>();

        lastCreatedCell = "Floor " + (loader.getRowAndColumnNumber() - 1) + "," + (loader.getRowAndColumnNumber() - 1);
        lastFloor = GameObject.Find(lastCreatedCell);

        playerBody = this.GetComponent<Rigidbody>();

        Vector3 playerJumpOffset = new Vector3(0, 10, 0);
        transform.position = lastFloor.transform.position + playerJumpOffset;

        gameManager = GameObject.Find("Manager").GetComponent<GameManager>();

        // set health bar
        healthBarControl.SetHealthBarValue(health * 0.02f);
    }

    void FixedUpdate()
    {
        // keyboardPlayerMovement();
        // touchPlayerMovement();
        // Check if we are running either in the Unity editor or in a standalone build. 
        #if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR
            keyboardPlayerMovement();
        // Check if we are running on a mobile device 
        #elif UNITY_IOS || UNITY_ANDROID
            touchPlayerMovement();
            // add touch movement for mobile phone screens.
        #endif
        playerOutOfBounds();
    }

    void keyboardPlayerMovement()
    {
        // play player's movement audio clip.
        // audioSource.PlayOneShot(playerMovingSound);

        // todo: instantiate player movement smoke particle system.

        Vector3 movement = new Vector3(movementX, 0, movementY);
        playerBody.AddForce(movement * speed, ForceMode.VelocityChange);
    }

    void touchPlayerMovement(){
        variableJoystick.SetActive(true);
        VariableJoystick joystick = variableJoystick.GetComponent<VariableJoystick>();
        Vector3 movement = new Vector3(joystick.Horizontal, 0, joystick.Vertical);
        playerBody.AddForce(movement * speed, ForceMode.VelocityChange);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Goal"))
        {
            // pickup the goal object.
            GoalBehaviour goalBehaviour = other.gameObject.GetComponent<GoalBehaviour>();
            goalBehaviour.Pickup();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // play sound everytime we collide with
        // 1. ground
        // 2. wall
        // 3. enemy
        // set collision with ground to true
        switch (collision.gameObject.tag)
        {
            case "Ground":
                // Debug.Log("Collided with the ground");
                break;
            case "Wall":
                // Debug.Log("Collided with the wall");
                break;
            case "Enemy":
                // Debug.Log("Collided with the enemy");
                break;
            case "NavMesh":
                // Debug.Log("Collided with the NavMesh");
                hasHitGround = true;
                break;
            default:
                // Debug.Log("Colliding with empty space.");
                isColliding = false;
                break;
        }
    }

    public void OnMove(InputValue value)
    {
        Vector2 movementVector = value.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    public void OnFire()
    {
        // play player's shooting audio clip.
        // audioSource.PlayOneShot(playerShootingSound);

        // todo: instantiate shooting particle system.

        // instantiate bullet prefab and move towards pointed direction.
        Transform bulletProjectile = Instantiate(bulletPrefab.transform, nozzel.transform.position, nozzel.transform.rotation);
        Vector3 shootDir = (nozzel.transform.position - transform.position).normalized;
        bulletProjectile.GetComponent<BulletBehaviour>().Setup(shootDir);

        // todo: shake camera slightly
    }

    public void OnRotate(InputValue value)
    {
        Vector2 rotationVector = value.Get<Vector2>();
        Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 15 * rotationVector.x, 0), 0.5f);
        transform.Rotate(new Vector3(0, 15 * rotationVector.x, 0));
    }

    public void updateHealthBar(float damagePoints)
    {
        health -= damagePoints;
        healthBarControl.SetHealthBarValue(healthBarControl.GetHealthBarValue() - (0.01f * damagePoints));
    }

    public void playerOutOfBounds()
    {
        // check if the player is bounds.

        // If player is falling.
        if (transform.position.y < -5)
        {
            gameManager.GameOver(false);
        }

        if (hasHitGround && isColliding)
            // Display Game Won Menu
            gameManager.GameOver(false);
    }
}
