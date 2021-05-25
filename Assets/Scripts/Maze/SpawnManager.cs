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
        // todo, find a smarter way of spawning enemies such that the enemies are not all spawned at once?
        int enemyType = Random.Range(0, 2);
        for (int i = 0; i < (LevelDifficulty.levelDifficulty + 1); i++)
        {
            if(LevelDifficulty.levelDifficulty == 1) return;
            // spawn objects in random cells.

            GameObject mazeLoader = GameObject.Find("Maze Loader Holder");
            MazeLoader loader = mazeLoader.GetComponent<MazeLoader>();

            string lastCreatedCell = "Floor " + (Random.Range(1, loader.getRowAndColumnNumber())) + "," + (Random.Range(0, loader.getRowAndColumnNumber()));

            Vector3 playerJumpOffset = new Vector3(0, 10, 0);
            spawn(enemyArr[enemyType], GameObject.Find(lastCreatedCell).transform.position + playerJumpOffset);
        }
    }

    void spawn(GameObject enemy, Vector3 position){
        Instantiate(enemy, position, transform.rotation);
    }
}
