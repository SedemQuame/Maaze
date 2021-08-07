using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDockController : MonoBehaviour
{
    // this should be in increments of 2 to 10.
    public int healthIncrementValue;
    public GameObject player;
    [SerializeField]
    private float healthIncrementRate = 0.1f;
    [SerializeField]
    private bool isStandingOnHealingPad = false, isCoroutineRunning = false, isPlayer = false;
    [SerializeField]
    private Collider collidingGameObject = null;

    void Start(){
        player = GameObject.Find("Player");
    }

    void Update(){
        if(collidingGameObject != null && isStandingOnHealingPad){
            if(!isCoroutineRunning){
                StartCoroutine("restoreHealth");
            }
        }else{
            StopCoroutine("restoreHealth");
            isCoroutineRunning = false;
        }
    }

    void OnTriggerEnter(Collider other){
        collidingGameObject = other;
        if(collidingGameObject.tag == "Player"){
            isPlayer = true;
        }
        isStandingOnHealingPad = true;
    }

    void OnTriggerExit(Collider other){
        collidingGameObject = other;
        if(collidingGameObject.tag == "Player"){
            isPlayer = false;
        }
        isStandingOnHealingPad = false;
    }

    IEnumerator restoreHealth(){
        isCoroutineRunning = true;
        while(isPlayer){
            player.GetComponentInParent<PlayerController>().updateHealthBar(-healthIncrementValue);
            yield return new WaitForSeconds(0.4f);
        }
    }
}
