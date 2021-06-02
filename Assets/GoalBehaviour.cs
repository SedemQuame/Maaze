using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class controlling the behaviour of the level's goal game object.
/// </summary>
public class GoalBehaviour : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // rotate the goal, about the y-axis per every frame.
        transform.Rotate(0, 2, 0);
    }
}
