using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileBehaviour : MonoBehaviour
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
        // play sound everytime we collide with
        // 1. ground
        // 2. wall
        // set collision with ground to true
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
            case "Player":
                Debug.Log("Collided with the enemy");
                // Play destory particle system for bullet.
                Destroy(transform.gameObject);

                // Destroy enemy game object or reduce health.
                // Destroy(collision.gameObject);
                break;
            case "NavMesh":
                Debug.Log("Collided with the NavMesh");
                break;
            default:
                Debug.Log("Colliding with empty space.");
                break;
        }
    }
}
