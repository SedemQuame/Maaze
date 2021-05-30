using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    private string firstCreatedCell = "Floor " + 0 + "," + 0;
    private Vector3 goalOffset = new Vector3(0, 2, 0);
    public GameObject reward;
    public GameObject[] enemyArr;
    private GameObject spawnManagerFloor;

    public Material[] materials;
    private Renderer renderer;
    private float spawnRate = 30.0f;
    private float blinkRate = 0.8f;

    // Start is called before the first frame update
    void Start()
    {
        spawnReward();
        spawnEnemiesByLevelDifficulty();
        spawnRate /= (LevelDifficulty.levelDifficulty);
    }

    void spawnReward()
    {
        Instantiate(reward, (GameObject.Find(firstCreatedCell).transform.position + goalOffset), transform.rotation);
    }

    void spawnEnemiesByLevelDifficulty()
    {
        // todo, find a smarter way of spawning enemies such that the enemies are not all spawned at once?
        // tood, update the spawn manager to spawn a number of enemies at a given rate, every X amount of time.
        if (LevelDifficulty.levelDifficulty == 1) return;

        GameObject mazeLoader = GameObject.Find("Maze Loader Holder");
        MazeLoader loader = mazeLoader.GetComponent<MazeLoader>();

        string spawnManagerCell = "Floor " + 0 + "," + (loader.getRowAndColumnNumber() - 1);
        spawnManagerFloor = GameObject.Find(spawnManagerCell);

        renderer = spawnManagerFloor.GetComponent<Renderer>();
        renderer.enabled = true;
        StartCoroutine("SpawnEnemy");
    }

    IEnumerator SpawnEnemy()
    {
        for (int i = 0; i < LevelDifficulty.levelDifficulty + 2; i++)
        {
            int enemyType = Random.Range(0, enemyArr.Length);
            spawnEnemy(enemyArr[enemyType], spawnManagerFloor.transform.position + new Vector3(0, 2, 0), Quaternion.Euler(0.0f, 90.0f, 0.0f));
            StartCoroutine("ToggleColor");
            yield return new WaitForSeconds(blinkRate);
        }
    }

    IEnumerator ToggleColor()
    {
        if (renderer.sharedMaterial == materials[0])
        {
            renderer.sharedMaterial = materials[1];
        }
        else
        {
            renderer.sharedMaterial = materials[0];
        }
        yield return new WaitForSeconds(spawnRate);
    }

    void spawnEnemy(GameObject enemy, Vector3 position, Quaternion rotation)
    {
        Instantiate(enemy, position, rotation);
    }
}
