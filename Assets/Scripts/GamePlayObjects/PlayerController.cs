using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

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
    /// <summary>
    /// Represents the source that plays the audio sound.
    /// </summary>
    public AudioClip shootingSound;
    private Rigidbody playerBody;
    private GameManager gameManager;
    private GameObject lastFloor;
    [Tooltip("Displays the component used for touch surfaces")]
    public GameObject touchControlPanel;
    private string lastCreatedCell;
    private float movementX, movementY;
    private bool isColliding;
    private bool hasHitGround;
    private float volUp = 1.0f;
    private float volDown = 0.6f;
    private AudioSource audioSource;
    public AudioClip hurtSound;
    public AudioClip dyingSound;
    private bool isFiring, isRotatingLeft, isRotatingRight;
    private float rotationAngle;

    // Start is called before the first frame update
    void Start()
    {
        isColliding = false;
        hasHitGround = false;

        rotationAngle = 7.5f;

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
        // Check if we are running either in the Unity editor or in a standalone build. 
        #if UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBGL
            keyboardPlayerMovement();
            touchControlPanel.SetActive(false);
        // Check if we are running on a mobile device 
        #elif UNITY_IOS || UNITY_ANDROID
            touchPlayerMovement();
            touchControlPanel.SetActive(true);

            if (isFiring)
            {
                OnFire();
            }
v  
            if (isRotatingLeft)
            {
                OnRotateLeft();
            }

            if (isRotatingRight)
            {
                OnRotateRight();
            }
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

    void OnCollisionEnter(Collision collider)
    {
        // play sound everytime we collide with
        // 1. ground
        // 2. wall
        // 3. enemy
        // set collision with ground to true
        float vol = Random.Range(volDown, volUp);
        switch (collider.gameObject.tag)
        {
            case "Ground":
                // Debug.Log("Collided with the ground");
                break;
            case "Wall":
                // Debug.Log("Collided with the wall");
                break;
            case "Enemy":
                // Debug.Log("Collided with the enemy");
                // play sound for receiving damage
                audioSource.PlayOneShot(hurtSound, vol);
                
                // reduce player health by number of damage points.
                this.updateHealthBar(collider.gameObject.GetComponent<EnemyController>().damagePoints);

                // if player health is 0, destory the player.
                if (this.health < 1)
                {
                    // instantiate death particle system.
                    collider.gameObject.GetComponent<EnemyController>().DestroyEffect(collider.gameObject);

                    // play sound for enemy dying, and soul leaving body.
                    audioSource.PlayOneShot(dyingSound, vol);

                    // destroy player gameObject
                    StartCoroutine(destroyPlayerGameObject(this.gameObject));
                }

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
        audioSource.PlayOneShot(playerShootingSound);

        // todo: instantiate shooting particle system at the position bullet 
        // spawned

        // instantiate bullet prefab and move towards pointed direction.
        Transform bulletProjectile = Instantiate(bulletPrefab.transform, nozzel.transform.position, nozzel.transform.rotation);
        Vector3 shootDir = (nozzel.transform.position - transform.position).normalized;
        bulletProjectile.GetComponent<BulletBehaviour>().Setup(shootDir);

        // todo: shake camera slightly
    }

    public void OnRotate(InputValue value)
    {
        Vector2 rotationVector = value.Get<Vector2>();
        Debug.Log("Rotation Vector: " + rotationVector.x);
        Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 15 * -rotationVector.x, 0), 0.5f);
        transform.Rotate(new Vector3(0, 15 * -rotationVector.x, 0));
    }
 
 #if UNITY_IOS || UNITY_ANDROID || UNITY_EDITOR
    public void OnRotateLeft()
    {
        float rotationVectorX = 1.0f;
        Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, rotationAngle * rotationVectorX, 0), 0.5f);
        transform.Rotate(new Vector3(0, rotationAngle * rotationVectorX, 0));
    }

    public void OnRotateRight()
    {
        float rotationVectorX = -1.0f;
        Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, rotationAngle * rotationVectorX, 0), 0.5f);
        transform.Rotate(new Vector3(0, rotationAngle * rotationVectorX, 0));
    }

    public void rotateLeftPointerDown(){
        isRotatingLeft = true;
    }

    public void rotateLeftPointerUp(){
        isRotatingLeft = false;
    }

    public void rotateRightPointerDown(){
        isRotatingRight = true;
    }

    public void rotateRightPointerUp(){
        isRotatingRight = false;
    }

    public void firePointerDown(){
        isFiring = true;
    }

    public void firePointerUp(){
        isFiring = false;
    }        
#endif

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

        if (hasHitGround && isColliding){
            // Display Game Won Menu
            gameManager.GameOver(false);
        }
    }

    IEnumerator destroyPlayerGameObject(GameObject playerGameObject){        
        yield return new WaitForSeconds(1.0f);
    
        // Show the gameOver UI.
        bool gameWon = false;
        gameManager.GameOver(gameWon);

        yield return new WaitForSeconds(1.0f);
        // Hide the enemy gameObject
        playerGameObject.SetActive(false);
    }
}
