using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class controlling the behaviour of the level's goal game object.
/// </summary>
public class GoalBehaviour : MonoBehaviour
{
    public GameObject pickupEffect;
    public AudioClip pickUpSoundFx;
    // public AudioSource audioData {  get { return GetComponent<AudioSource>(); } }
    public AudioSource audioData;

    void Start(){
        // audioData = GetComponent<AudioSource>();
        audioData.enabled = true;
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
