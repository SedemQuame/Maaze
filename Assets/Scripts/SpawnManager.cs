using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject [] enemyArr;
    // Start is called before the first frame update
    void Start()
    {
        spawn(enemyArr[0], new Vector3(0,0,0));
        spawn(enemyArr[0], new Vector3(10,0,10));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void spawn(GameObject enemy, Vector3 position){
        Instantiate(enemy, position, transform.rotation);
    }
}
