using UnityEngine;

/// <summary>
/// A class controlling the behaviour of the level's goal game object.
/// </summary>
public class GoalBehaviour : MonoBehaviour
{
    public GameObject pickupEffect;

    // Update is called once per frame
    void Update()
    {
        // rotate the goal, about the y-axis per every frame.
        transform.Rotate(0, 2, 0);
    }

    void OnDestroy()
    {
        Pickup();
    }

    void Pickup()
    {
        Instantiate(pickupEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
