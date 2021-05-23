using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject [] enemyArr;
    // Start is called before the first frame update
    void Start()
    {
        spawnEnemiesByLevelDifficulty();
    }

    void spawnEnemiesByLevelDifficulty(){
        int enemyType = Random.Range(0, 2);
        for (int i = 0; i < (LevelDifficulty.levelDifficulty + 1); i++)
        {
            if(LevelDifficulty.levelDifficulty == 1) return;
            spawn(enemyArr[enemyType], new Vector3(0,0,0));
        }
    }

    void spawn(GameObject enemy, Vector3 position){
        Instantiate(enemy, position, transform.rotation);
    }
}
