using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// The SpawnManager is responsible for spawning all guard or enemy game objects in the scene.
/// </summary>
[RequireComponent(typeof(GameObject))]
[RequireComponent(typeof(Material))]
[RequireComponent(typeof(Renderer))]

public class SpawnManager : MonoBehaviour
{
// ===============PUBLIC VARIABLES===============
    public Material[] materials;
    public GameObject[] reward;
    public GameObject[] enemyArr;
    public GameObject healthDock, portal;
    public GameObject eventManager, objectiveStatusIndicator;
    public TMP_Text rewardCountText;
// ===============PRIVATE VARIABLES===============
    private GameObject infoBoxPrefabSmall;
    private GameObject spawnManagerFloor, eventManagerBody;
    private MazeLoader loader;
    private float spawnRate, initialSpawnRate = 30.0f;
    private int numberOfRewards;
    private string eventMessage;
    private bool spawnedPortal, isPortalSpawned, eventBoxedDisplayed = false;

    void Start()
    {
        loader = GameObject.Find("Maze Loader Holder").GetComponent<MazeLoader>();
        isPortalSpawned = false;
        if(LevelDifficulty.levelDifficulty > 2){ // do not spawn collectibles till level 3.
            StartCoroutine(spawnStart());   
            objectiveStatusIndicator.SetActive(true);
        }
        eventManagerBody = eventManager.transform.GetChild(0).gameObject;
    }

    void FixedUpdate()
    {
        if(LevelDifficulty.levelDifficulty == 1){
            spawnPortal();
        }else{
            if (numberOfRewards > GameObject.FindGameObjectsWithTag("Goal").Length)
            {
                reduceRewardCount();
                updateRewardCountText();
            }
            checkGameOverStatus();
        }
        showDisplayPanel();
    }

    void showDisplayPanel(){
        if (isPortalSpawned && !eventBoxedDisplayed)
        {            
            // show info box
            eventManager.SetActive(true);
            StartCoroutine(hideWorldInfoBox());

            // display the objective manager after X amount of time.
            displayEventMessage(eventMessage);
            eventBoxedDisplayed = true;
        }
    }

     public void displayEventMessage(string eventMessage){       
        // clear all children in eventManagerBody.
        foreach (Transform child in eventManagerBody.transform) {
            GameObject.Destroy(child.gameObject);
        }

        // Loading resource prefabs
        infoBoxPrefabSmall = Resources.Load("UI/InfoBox_Small") as GameObject;

        // Instruction Info Box
        GameObject instructionInfoBox = Instantiate(infoBoxPrefabSmall);
        instructionInfoBox.transform.SetParent(eventManagerBody.transform, false);

        // instructions
        instructionInfoBox.transform.GetChild(0).gameObject.GetComponent<Text>().text = "New Event";
        instructionInfoBox.transform.GetChild(1).transform.GetChild(0).gameObject.GetComponent<Text>().text = eventMessage;  
    }

    void reduceRewardCount()
    {
        numberOfRewards--;
    }

    void updateRewardCountText()
    {
        rewardCountText.text = "Pickups: " + numberOfRewards.ToString();
    }

    void spawnPortal(){
        if(!spawnedPortal){
            randomlySpawn(portal, -0.9f);
        }
        spawnedPortal = true;
    }

    void checkGameOverStatus()
    {
        if (numberOfRewards < 1)
        {   
            spawnPortal();
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
    /// Spawns the reward game object, in a random cell.
    /// </summary>
    void randomlySpawn(GameObject gameObject, float ySpawnPoint)
    {
        string cell = "Floor " + Random.Range(0, (loader.getRowAndColumnNumber() - 1)) + "," + Random.Range(0, (loader.getRowAndColumnNumber() - 1));
        Vector3 floorPosition = GameObject.Find(cell).transform.position;
        Instantiate(gameObject, new Vector3(floorPosition.x, ySpawnPoint, floorPosition.z), transform.rotation);
        if(gameObject.CompareTag("Portal") && LevelDifficulty.levelDifficulty > 1){
            eventMessage = "Portal spawned at cell location: (" + cell.Split(' ')[1] + ").";
            isPortalSpawned = true;
        }
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
    /// Spawn an Enemy gameObject in a given position.
    /// </summary>
    void spawnEnemy(GameObject enemy, Vector3 position, Quaternion rotation)
    {
        Instantiate(enemy, position, rotation);
    }

    IEnumerator hideWorldInfoBox(){
        yield return new WaitForSeconds(6.0f);
        eventManager.SetActive(false);
    }

    IEnumerator spawnStart(){
        //calculate spawnRate using Exponential Decay.
        if(LevelDifficulty.levelDifficulty < 12){
            spawnRate = initialSpawnRate*(Mathf.Pow((1 - 0.08f), LevelDifficulty.levelDifficulty));
        }else{
            spawnRate = 10.0f;
        }
        spawnRewardsByLevelDifficulty();
        updateRewardCountText();

        //wait X amount of seconds before spawning enemies
        yield return new WaitForSeconds(4);

        if(LevelDifficulty.levelDifficulty > 3){
            spawnEnemiesByLevelDifficulty();
            //spawn health dock, if level difficulty > 6.
            if(LevelDifficulty.levelDifficulty > 5){
                randomlySpawn(healthDock, -0.9f);
            }
        }
    }

    /// <summary>
    /// A coroutine for timing the spawning of Enemy gameObjects.
    /// </summary>
    IEnumerator SpawnEnemy()
    {
        for (int i = 0; i < LevelDifficulty.levelDifficulty + 2; i++)
        {
            string spawnManagerCell = "Floor " + Random.Range(0, (loader.getRowAndColumnNumber() - 1)) + "," + Random.Range(0, (loader.getRowAndColumnNumber() - 1));
            spawnManagerFloor = GameObject.Find(spawnManagerCell);

            // get the renderer for the floor to spawn the enemy on.
            Renderer rend = spawnManagerFloor.GetComponent<Renderer>();
            rend.enabled = true;

            // toggle color for floor.
            StartCoroutine(ToggleColor(spawnManagerCell, Random.Range(0, enemyArr.Length), rend));

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
}
