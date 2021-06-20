using UnityEngine;
using UnityEngine.InputSystem;


public class BulletBehaviour : MonoBehaviour
{
    public float bulletSpeed;
    public float bulletDamage;
    private Rigidbody rigidbody;
    private Vector3 shootDir;
    private float playerRotationAxis;

    public void Setup(Vector3 shootDir)
    {
        this.shootDir = shootDir;
        this.shootDir.y = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += shootDir * bulletSpeed * Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        // destroy bullet when it collides with other game objects
        // 1. ground
        // 2. wall
        // set collision with ground to true
        // 3. enemy
        switch (collision.gameObject.tag)
        {
            case "Ground":
                Debug.Log("Collided with the ground");
                Destroy(transform.gameObject);
                break;
            case "Wall":
                Debug.Log("Collided with the wall");
                Destroy(transform.gameObject);
                break;
            case "Enemy":
                Debug.Log("Collided with the enemy");
                // Play destory particle system for bullet.
                Destroy(transform.gameObject);
                break;
            case "NavMesh":
                Debug.Log("Collided with the NavMesh");
                Destroy(transform.gameObject);
                break;
            default:
                Debug.Log("Colliding with empty space.");
                break;
        }
    }
}
