using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Makes the camera follow the player's movements.
/// </summary>
public class CameraController : MonoBehaviour
{
    // ===============PUBLIC VARIABLES===============
    public GameObject player;
    // ===============PRIVATE VARIABLES===============
    private Vector3 offset;

    void Start()
    {
        player = GameObject.Find("Player");
        offset = new Vector3(0, 30, 0);
    }

    void Update()
    {
        if (player.gameObject != null)
        {
            transform.position = player.GetComponent<Rigidbody>().position + offset;
        }
    }
}
