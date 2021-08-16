using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioClip))]
public class PlayerController : MonoBehaviour
{
    // ===============PUBLIC VARIABLES===============
    [Tooltip("Health value of the player")]
    public float health = 100.0f;
    public GameObject bulletPrefab, nozzel, variableJoystick, touchControlPanel;

    [Tooltip("Sounds played when actions occurs.")]
    public AudioClip playerShootingSound, playerMovingSound, shootingSound, hurtSound, dyingSound;
    public HealthBarControl healthBarControl;

    // ===============PRIVATE VARIABLES===============
    private Rigidbody playerBody;
    private GameManager gameManager;
    [SerializeField]
    private GameObject lastFloor;
    [SerializeField]
    private bool isColliding, hasHitGround, isFiring, isRotatingLeft, isRotatingRight;
    [SerializeField]
    private float movementX, movementY, vol, volUp = 1.0f, volDown = 0.6f, rotationAngle, playerSpeed;
    [SerializeField]
    private AudioSource audioSource;

    void Start()
    {
        isColliding = false;
        hasHitGround = false;
        rotationAngle = 7.5f;
        playerSpeed = 0.8f;
        audioSource = this.GetComponent<AudioSource>();
        playerBody = this.GetComponent<Rigidbody>();
        gameManager = GameObject.Find("Manager").GetComponent<GameManager>();
        // get loader and return last spawned floor.
        lastFloor = getLastSpawnedFloor();
        // spawn player at jump offset
        Vector3 playerJumpOffset = new Vector3(0, 10, 0);
        transform.position = lastFloor.transform.position + playerJumpOffset;
        // set health bar
        healthBarControl.SetHealthBarValue(health * 0.02f);
    }

    void FixedUpdate()
    {
        #if UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBGL //running either in Unity editor, standalone build or webgl. 
            keyboardPlayerMovement();
            touchControlPanel.SetActive(false);
        
        #elif UNITY_IOS || UNITY_ANDROID //running on a mobile device 
            touchPlayerMovement();
            touchControlPanel.SetActive(true);

            if (isFiring)
            {
                shootAmunition();
            }

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

    GameObject getLastSpawnedFloor(){
        // get loader and return last spawned floor.
        MazeLoader loader = GameObject.Find("Maze Loader Holder").GetComponent<MazeLoader>();
        string lastCreatedCell = "Floor " + (loader.getRowAndColumnNumber() - 1) + "," + (loader.getRowAndColumnNumber() - 1);
        return GameObject.Find(lastCreatedCell);
    }

    void keyboardPlayerMovement()
    {
        Vector3 movement = new Vector3(movementX, 0, movementY);
        playerBody.AddForce(movement * playerSpeed, ForceMode.VelocityChange);
        // todo: instantiate player movement smoke particle system & play movement sound.
        // audioSource.PlayOneShot(playerMovingSound);
    }

    void touchPlayerMovement(){
        variableJoystick.SetActive(true);
        VariableJoystick joystick = variableJoystick.GetComponent<VariableJoystick>();
        Vector3 movement = new Vector3(joystick.Horizontal, 0, joystick.Vertical);

        // todo: rotate to the direction of movement the move forward.
        playerBody.AddForce(movement * playerSpeed, ForceMode.VelocityChange);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Goal"))
        {
            GoalBehaviour goalBehaviour = other.gameObject.GetComponent<GoalBehaviour>();
            goalBehaviour.Pickup();
        }
    }

    void OnCollisionEnter(Collision collider)
    {
        /* todo: play sound everytime we collide with
        // 1. ground
        // 2. wall
        // 3. enemy
        */

        vol = Random.Range(volDown, volUp);
        switch (collider.gameObject.tag)
        {
            case "Ground":
                // todo: play sound for colliding with "ground"
                break;
            case "Wall":
                // todo: play sound for colliding with "wall"
                break;
            case "Enemy":
                audioSource.PlayOneShot(hurtSound, vol);
                // reduce player health by enemy damage points.
                this.updateHealthBar(collider.gameObject.GetComponent<EnemyController>().damagePoints);
                if (this.health < 1)
                {
                    // instantiate dying particle system
                    collider.gameObject.GetComponent<EnemyController>().DestroyEffect(collider.gameObject);
                    // play dying sound
                    audioSource.PlayOneShot(dyingSound, vol);
                    // destroy player gameObject
                    StartCoroutine(destroyPlayerGameObject(this.gameObject));
                }
                break;
            case "NavMesh":
                // todo: play sound for colliding with "ground"
                hasHitGround = true;
                break;
            default:
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

#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBGL //running either in Unity editor, standalone build or webgl. 
    public void OnFire()
    {
        shootAmunition();
    }
#endif

    void shootAmunition(){
        audioSource.PlayOneShot(playerShootingSound);
        shootBullet();
        // todo: instantiate shooting particle system at the position bullet 
    }

    void shootBullet(){
        Transform bulletProjectile = Instantiate(bulletPrefab.transform, nozzel.transform.position, nozzel.transform.rotation);
        Vector3 shootDir = (nozzel.transform.position - transform.position).normalized;
        bulletProjectile.GetComponent<BulletBehaviour>().Setup(shootDir);
    }

    public void OnRotate(InputValue value)
    {
        Vector2 rotationVector = value.Get<Vector2>();
        Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 15 * -rotationVector.x, 0), 0.5f);
        transform.Rotate(new Vector3(0, 15 * -rotationVector.x, 0));
    }
 
 #if UNITY_IOS || UNITY_ANDROID || UNITY_EDITOR
    void OnRotateLeft()
    {
        float rotationVectorX = 1.0f;
        Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, rotationAngle * rotationVectorX, 0), 0.5f);
        transform.Rotate(new Vector3(0, rotationAngle * rotationVectorX, 0));
    }

    void OnRotateRight()
    {
        float rotationVectorX = -1.0f;
        Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, rotationAngle * rotationVectorX, 0), 0.5f);
        transform.Rotate(new Vector3(0, rotationAngle * rotationVectorX, 0));
    }

    void rotateLeftPointerDown(){
        isRotatingLeft = true;
    }

    void rotateLeftPointerUp(){
        isRotatingLeft = false;
    }

    void rotateRightPointerDown(){
        isRotatingRight = true;
    }

    void rotateRightPointerUp(){
        isRotatingRight = false;
    }

    void firePointerDown(){
        isFiring = true;
    }

    void firePointerUp(){
        isFiring = false;
    }        
#endif

    public void updateHealthBar(float damagePoints)
    {
        health -= damagePoints;
        healthBarControl.SetHealthBarValue(healthBarControl.GetHealthBarValue() - (0.01f * damagePoints));
    }

    void playerOutOfBounds()
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
        yield return new WaitForSeconds(0.5f);
        gameManager.GameOver(false); // Show gameover ui.
        yield return new WaitForSeconds(1.0f);
        playerGameObject.SetActive(false);
    }
}
