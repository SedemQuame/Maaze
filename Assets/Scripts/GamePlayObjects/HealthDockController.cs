using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDockController : MonoBehaviour
{
    // this should be in increments of 2 to 10.
    public int healthIncrementValue;
    public float healthIncrementRate = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other){
        StartCoroutine(restoreHealth(other));
    }

    void OnTriggerExit(Collider other){
        StopCoroutine(restoreHealth(other));
    }

    IEnumerator restoreHealth(Collider other){
        // increment the health value.
        yield return new WaitForSeconds(healthIncrementRate);
        if(other.gameObject.tag == "Player" || other.gameObject.tag == "Nozzle"){
            // negate health point when passing to (-- => +).
            if(other.gameObject.tag == "Nozzle"){
                other.gameObject.GetComponentInParent<PlayerController>().updateHealthBar(-healthIncrementValue);
            }else{
                other.gameObject.GetComponent<PlayerController>().updateHealthBar(-healthIncrementValue);
            }
        }
        yield return null;
    }
}
