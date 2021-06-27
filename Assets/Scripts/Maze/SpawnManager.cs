using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// The SpawnManager is responsible for spawning all guard or enemy game objects in the scene.
/// </summary>

[RequireComponent(typeof(GameObject))]
[RequireComponent(typeof(Material))]
[RequireComponent(typeof(Renderer))]

public class SpawnManager : MonoBehaviour
{

    public Material[] materials;
    public GameObject[] reward;
    public GameObject[] enemyArr;
    public Vector3 goalOffset;
    private GameObject spawnManagerFloor;
    private GameManager gameManager;

    private Renderer rend;
    private string firstCreatedCell;
    private float spawnRate = 30.0f;
    // private float blinkRate = 0.8f;
    private MazeLoader loader;
    private int numberOfRewards;
    public TMP_Text rewardCountText;

    // Start is called before the first frame update
    void Start()
    {
        firstCreatedCell = "Floor " + 0 + "," + 0;
        goalOffset = new Vector3(0, -0.4f, 0);
        spawnRate /= LevelDifficulty.levelDifficulty;

        GameObject mazeLoader = GameObject.Find("Maze Loader Holder");
        loader = mazeLoader.GetComponent<MazeLoader>();

        gameManager = GameObject.Find("Manager").GetComponent<GameManager>();

        spawnRewardsByLevelDifficulty();
        updateRewardCountText();
        spawnEnemiesByLevelDifficulty();
    }

    void FixedUpdate()
    {
        if (numberOfRewards > GameObject.FindGameObjectsWithTag("Goal").Length)
        {
            reduceRewardCount();
            updateRewardCountText();
        }
        checkGameOverStatus();
    }

    public void reduceRewardCount()
    {
        // set initial value for rewards left.
        numberOfRewards -= 1;
    }

    void updateRewardCountText()
    {
        rewardCountText.text = "Pickups: " + numberOfRewards.ToString();
    }

    void checkGameOverStatus()
    {

        if (numberOfRewards < 1)
        {
            // game over player won.
            gameManager.GameOver(true);
        }
    }


    /// <summary>
    /// Spawns a number of rewards according to the level difficulty.
    /// </summary>
    void spawnRewardsByLevelDifficulty()
    {
        for (int i = 0; i < LevelDifficulty.levelDifficulty; i++)
        {
            spawnReward();
        }
        numberOfRewards = LevelDifficulty.levelDifficulty;
    }

    /// <summary>
    /// Spawns the reward game object, in the first created cell (genesis cell).
    /// </summary>
    void spawnReward()
    {
        // Make is such that reward can be spawned multiple times, and in various cells.
        string cell = "Floor " + Random.Range(0, (loader.getRowAndColumnNumber() - 1)) + "," + Random.Range(0, (loader.getRowAndColumnNumber() - 1));
        Instantiate(reward[Random.Range(0, 3)], new Vector3(GameObject.Find(cell).transform.position.x, -0.4f, GameObject.Find(cell).transform.position.z), transform.rotation);
    }

    /// <summary>
    /// Spawns the Enemy game objects, in a randomized patterns.
    /// Creates a blinking cell to alert the Player that an Enemy is about to be spawned in a particular cell.
    /// </summary>
    void spawnEnemiesByLevelDifficulty()
    {
        // todo, find a smarter way of spawning enemies such that the enemies are not all spawned at once?
        // tood, update the spawn manager to spawn a number of enemies at a given rate, every X amount of time.
        if (LevelDifficulty.levelDifficulty == 1) return;
        StartCoroutine("SpawnEnemy");
    }

    // void OnCollisionEnter(Collision collision)
    // {
    //     switch (collision.gameObject.tag)
    //     {
    //         case "Player":
    //             // Debug.Log("Collided with the player");
    //             reduceRewardCount();
    //             break;
    //     }
    // }

    /// <summary>
    /// A coroutine for timing the spawning of Enemy gameObjects.
    /// </summary>
    IEnumerator SpawnEnemy()
    {
        for (int i = 0; i < LevelDifficulty.levelDifficulty + 2; i++)
        {
            int enemyType = Random.Range(0, enemyArr.Length);

            string spawnManagerCell = "Floor " + Random.Range(0, (loader.getRowAndColumnNumber() - 1)) + "," + Random.Range(0, (loader.getRowAndColumnNumber() - 1));
            spawnManagerFloor = GameObject.Find(spawnManagerCell);

            rend = spawnManagerFloor.GetComponent<Renderer>();
            rend.enabled = true;

            ToggleColor();

            spawnEnemy(enemyArr[enemyType], spawnManagerFloor.transform.position + new Vector3(0, 2, 0), Quaternion.Euler(0.0f, 90.0f, 0.0f));
            yield return new WaitForSeconds(spawnRate);
        }
    }

    /// <summary>
    /// A coroutine that changes the color of a cell's ground between two materials to indicated spawning of an Enemy gameObject.
    /// </summary>
    void ToggleColor()
    {
        int blinkNumber = 10, i = 0;
        while (blinkNumber > i)
        {

            if (rend.sharedMaterial == materials[0])
            {
                rend.sharedMaterial = materials[1];
            }
            else
            {
                rend.sharedMaterial = materials[0];
            }
            i++;
        }

    }
    /// <summary>
    /// Spawns of an Enemy gameObject in a given position.
    /// </summary>
    void spawnEnemy(GameObject enemy, Vector3 position, Quaternion rotation)
    {
        Instantiate(enemy, position, rotation);
    }
}
