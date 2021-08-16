using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoardPlane : MonoBehaviour
{
    private Camera theCamera;
    void Start()
    {
        theCamera = Camera.main;
    }

    void Update()
    {
        transform.LookAt(2 * transform.position - theCamera.transform.position);
    }
}
