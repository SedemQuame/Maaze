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
    public GameObject healthDock;
    public GameObject portal;
    public Vector3 goalOffset;
    private GameObject spawnManagerFloor;
    private string firstCreatedCell;
    private float spawnRate;
    private MazeLoader loader;
    private int numberOfRewards;
    private bool spawnedPortal;
    public TMP_Text rewardCountText;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(spawnStart());
    }

    IEnumerator spawnStart(){
        firstCreatedCell = "Floor " + 0 + "," + 0;
        goalOffset = new Vector3(0, -0.4f, 0);
        
        float initialSpawnRate = 30.0f;
        //calculate spawnRate using the Exponential Delay Function.
        if(LevelDifficulty.levelDifficulty < 12){
            spawnRate = initialSpawnRate*(Mathf.Pow((1 - 0.08f), LevelDifficulty.levelDifficulty));
        }else{
            spawnRate = 10.0f;
        }

        GameObject mazeLoader = GameObject.Find("Maze Loader Holder");
        loader = mazeLoader.GetComponent<MazeLoader>();

        spawnRewardsByLevelDifficulty();
        updateRewardCountText();

        //wait X amount of seconds before spawning enemies
        yield return new WaitForSeconds(4);

        if(LevelDifficulty.levelDifficulty > 3){
             //todo: refactor this to delay code using coroutines.
            spawnEnemiesByLevelDifficulty();

            // only spawn health dock, if level difficulty is greater at level 6.
            if(LevelDifficulty.levelDifficulty > 5){
                randomlySpawn(healthDock, -0.9f);
            }
        }
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
            if(!spawnedPortal){
                // show portal.
                randomlySpawn(portal, -0.9f);
            }
            spawnedPortal = true;
            // StartCoroutine(showGameMenu());
        }
    }

    /// <summary>
    /// Spawns a number of rewards according to the level difficulty.
    /// </summary>
    void spawnRewardsByLevelDifficulty()
    {
        for (int i = 0; i < LevelDifficulty.levelDifficulty; i++)
        {
            randomlySpawn(reward[Random.Range(0, 3)], -0.4f);
        }
        numberOfRewards = LevelDifficulty.levelDifficulty;
    }

    /// <summary>
    /// Spawns the reward game object, in the first created cell (genesis cell).
    /// </summary>
    void randomlySpawn(GameObject gameObject, float ySpawnPoint)
    {
        // Make is such that reward can be spawned multiple times, and in various cells.
        string cell = "Floor " + Random.Range(0, (loader.getRowAndColumnNumber() - 1)) + "," + Random.Range(0, (loader.getRowAndColumnNumber() - 1));
        Instantiate(gameObject, new Vector3(GameObject.Find(cell).transform.position.x, ySpawnPoint, GameObject.Find(cell).transform.position.z), transform.rotation);
    }

    /// <summary>
    /// Spawns the Enemy game objects, in a randomized patterns.
    /// Creates a blinking cell to alert the Player that an Enemy is about to be spawned in a particular cell.
    /// </summary>
    void spawnEnemiesByLevelDifficulty()
    {
        // if player has not reached a certain level, don't spawn any enemy.
        if (LevelDifficulty.levelDifficulty <= 4) return;
        
        StartCoroutine(SpawnEnemy());
    }

    /// <summary>
    /// A coroutine for timing the spawning of Enemy gameObjects.
    /// </summary>
    IEnumerator SpawnEnemy()
    {
        // todo: spawn a given numner of enemies.   
        for (int i = 0; i < LevelDifficulty.levelDifficulty + 2; i++)
        {
            int enemyType = Random.Range(0, enemyArr.Length);

            string spawnManagerCell = "Floor " + Random.Range(0, (loader.getRowAndColumnNumber() - 1)) + "," + Random.Range(0, (loader.getRowAndColumnNumber() - 1));
            spawnManagerFloor = GameObject.Find(spawnManagerCell);

            // get the renderer for the floor to spawn the enemy on.
            Renderer rend = spawnManagerFloor.GetComponent<Renderer>();
            rend.enabled = true;

            // toggle color for floor.
            StartCoroutine(ToggleColor(spawnManagerCell, enemyType, rend));

            // delay when next enemy is spawned.
            yield return new WaitForSeconds(spawnRate);
        }
    }

    /// <summary>
    /// A coroutine that changes the color of a cell's ground between two materials to indicated spawning of an Enemy gameObject.
    /// </summary>
    IEnumerator ToggleColor(string spawnManagerCell, int enemyType, Renderer rend)
    {
        // todo: play bliking buzzer audio.
        int blinkNumber = 9, i = 0;
        while (blinkNumber >= i)
        {
            yield return new WaitForSeconds(1);
            if (rend.sharedMaterial == materials[0])
            {
                rend.sharedMaterial = materials[enemyType + 1];
            }
            else
            {
                // change back to ground material
                rend.sharedMaterial = materials[0];
            }
            i++;
        }
        rend.sharedMaterial = materials[0];

        // todo: create a smoke prefab
        // todo: add audio for enemy instantiation.
        spawnEnemy(enemyArr[enemyType], spawnManagerFloor.transform.position + new Vector3(0, 2, 0), Quaternion.Euler(0.0f, 90.0f, 0.0f));
    }
    
    /// <summary>
    /// Spawn an Enemy gameObject in a given position.
    /// </summary>
    void spawnEnemy(GameObject enemy, Vector3 position, Quaternion rotation)
    {
        Instantiate(enemy, position, rotation);
    }
}
