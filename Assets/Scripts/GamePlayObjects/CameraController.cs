using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Makes the camera follow the player's movements.
/// </summary>
public class CameraController : MonoBehaviour
{
    // reference the player gameObject
    public GameObject player;
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        offset = new Vector3(0, 30, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (player.gameObject != null)
        {
            transform.position = player.GetComponent<Rigidbody>().position + offset;
        }
    }
}
