using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class controlling the behaviour of the level's goal game object.
/// </summary>
public class GoalBehaviour : MonoBehaviour
{
    private GameObject pointText;
    public GameObject pickupEffect;
    public float collectiblePoints;
    public AudioClip pickUpSoundFx; 
    public AudioSource audioData;

    void Start(){
        audioData.enabled = true;
        pointText = GameObject.Find("pointText");
    }

    void Update()
    {
        // rotate the goal, about the y-axis per every frame.
        transform.Rotate(0, 2, 0);
    }

    public void Pickup()
    { 
        PlayPickUpSound();
        Instantiate(pickupEffect, transform.position, transform.rotation);
        StartCoroutine(DestroyGoalObject());

        // update points system
        PointsSystem.updatePointSystem(collectiblePoints, pointText);
    }

    IEnumerator DestroyGoalObject(){
        yield return new WaitForSeconds(0.70f);
        Destroy(this.gameObject);
    }

    public void PlayPickUpSound(){
        // pickup sound
            float vol = Random.Range (.5f, 1.0f);
            audioData.PlayOneShot(pickUpSoundFx, vol);
    }
}
