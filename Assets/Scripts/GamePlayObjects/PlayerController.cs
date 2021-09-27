using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioClip))]
public class PlayerController : MonoBehaviour
{
    // ===============PUBLIC VARIABLES===============
    [Tooltip("Health value of the player")]
    public float health = 100.0f;
    public GameObject bulletPrefab, nozzel, touchControlPanel;
    public VariableJoystick variableJoystick;
    [Tooltip("Sounds played when actions occurs.")]
    public AudioClip playerShootingSound, playerMovingSound, shootingSound, hurtSound, dyingSound, landInMaze, wallHitSound;
    public HealthBarControl healthBarControl;

    // ===============PRIVATE VARIABLES===============
    private Rigidbody playerBody;
    private GameManager gameManager;
    [SerializeField]
    private GameObject lastFloor;
    [SerializeField]
    private bool isColliding, hasHitGround;
    [SerializeField]
    private float movementX, movementY, vol, volUp = 1.0f, volDown = 0.6f, rotationAngle, playerSpeed;
    [SerializeField]
    private AudioSource audioSource;

    #if UNITY_IOS || UNITY_ANDROID
    private bool isRotatingLeft, isRotatingRight, isFiring;
    #endif

    void Start()
    {
        isColliding = false;
        hasHitGround = false;
        rotationAngle = 5.0f;
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
        touchPlayerMovement();
        #if UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBGL //running either in Unity editor, standalone build or webgl. 
            keyboardPlayerMovement();
            touchControlPanel.SetActive(false);
        #elif UNITY_IOS || UNITY_ANDROID //running on a mobile device 
            touchControlPanel.SetActive(true);
            if (isFiring)
            {
                if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
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
        if(health > 0){
            Vector3 movement = new Vector3(movementX, 0, movementY);
            // rotate player body in put direction.
            // add force to the player body to move in the given direction.

            playerBody.AddForce(movement * playerSpeed, ForceMode.VelocityChange);
            // todo: instantiate player movement smoke particle system & play movement sound.
            // audioSource.PlayOneShot(playerMovingSound);
        }
    }

    void touchPlayerMovement(){
        Vector3 movement = new Vector3(variableJoystick.Horizontal, 0, variableJoystick.Vertical);
        playerBody.AddForce(movement * playerSpeed, ForceMode.VelocityChange);

        // todo: rotate to the direction of movement the move forward.
        // todo: instantiate player movement smoke particle system & play movement sound.
        // audioSource.PlayOneShot(playerMovingSound);
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
        vol = Random.Range(volDown, volUp);
        switch (collider.gameObject.tag)
        {
            case "NavMesh":
                break;
            case "Wall":
                // play sound when play hits walls
                audioSource.PlayOneShot(wallHitSound, vol);
                break;
            case "Enemy":
                EnemyController enemyController = collider.gameObject.GetComponent<EnemyController>();
                audioSource.PlayOneShot(hurtSound, vol);
                // cinemachine camera shake.
                CinemachineScreenShake.Instance.ShakeCamera(1.0f, 0.50f);
                // reduce player health by enemy damage points.
                this.updateHealthBar(enemyController.damagePoints);
                if (this.health < 1)
                {
                    // dying sequence
                    collider.gameObject.GetComponent<EnemyController>().DestroyEffect(collider.gameObject);
                    dyingSequence();
                }
                break;
            case "Ground":
                if(!hasHitGround){
                    CinemachineScreenShake.Instance.ShakeCamera(2.0f, 0.20f);
                    audioSource.PlayOneShot(landInMaze, 1);
                    hasHitGround = true;
                }
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
        if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
        shootAmunition();
    }
#endif

    public void shootAmunition(){
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
 
 #if UNITY_IOS || UNITY_ANDROID
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

    private void resetPlayerHealth(float healthPoints){
        health = healthPoints;
        healthBarControl.SetHealthBarValue((0.01f * healthPoints));
    }

    public void resusciatePlayer(){
        resetPlayerHealth(60.0f);
        this.gameObject.SetActive(true);
    }

    private void dyingSequence(){
        audioSource.PlayOneShot(dyingSound, vol);
        StartCoroutine(destroyPlayerGameObject(this.gameObject));
    }

    public void killPlayerOnTimeOut(){
        health = 0;
        healthBarControl.SetHealthBarValue(0);
        dyingSequence();
    }

    void playerOutOfBounds()
    {
        // If player is falling.
        if (transform.position.y < -5)
        {
            gameManager.GameOver(false);
        }

        if (hasHitGround && isColliding) {
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
