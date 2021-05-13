using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalController : MonoBehaviour
{
    private GameObject firstFloor;
    // private Rigidbody rigidbody;
    private string firstCreatedCell;
    private Vector3 goalOffset = new Vector3(0, 3, 0);
    // Start is called before the first frame update
    void Start()
    {
        firstCreatedCell = "Floor " + 0 + "," + 0;
        firstFloor = GameObject.Find(firstCreatedCell);
        if(firstFloor){
            transform.position = firstFloor.transform.position - goalOffset;
        }
    }

    void Update(){
        gameObject.transform.Rotate(0, 2, 0);
    }
}