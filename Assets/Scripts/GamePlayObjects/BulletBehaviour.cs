using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{

    // ===============PUBLIC VARIABLES===============
    public float bulletSpeed;
    public float bulletDamage;
    // ===============PRIVATE VARIABLES===============
    private Vector3 shootDir;
    private float playerRotationAxis;

    public void Setup(Vector3 shootDir)
    {
        this.shootDir = shootDir;
        this.shootDir.y = 0;
    }

    void Update()
    {
        transform.position += shootDir * bulletSpeed * Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Ground":
            case "Wall":
            case "Enemy":
            case "NavMesh":
                Destroy(transform.gameObject);
                break;
            default:
                break;
        }
    }
}
