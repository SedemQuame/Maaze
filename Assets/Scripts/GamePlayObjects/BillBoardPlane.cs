using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoardPlane : MonoBehaviour
{
    private Camera theCamera;
    // Start is called before the first frame update
    void Start()
    {
        theCamera = Camera.main;
    }

    // Update is called once per framepublic Camera theCamera;
    void Update()
    {
        transform.LookAt(2 * transform.position - theCamera.transform.position);
    }
}
