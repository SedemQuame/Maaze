using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public float bulletSpeed;
    public float bulletDamage;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0.05f * bulletSpeed, 0, 0);
    }
    void OnCollisionEnter(Collision collision)
    {
        // play sound everytime we collide with
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
