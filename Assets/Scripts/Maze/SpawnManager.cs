using System.Collections;
using System.Collections.Generic;
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
    public GameObject[] rewards;
    public GameObject[] enemyArr;
    public GameObject[] glitchedGameObjects;
    public GameObject healthDock, portal;
    public GameObject eventManager, objectiveStatusIndicator;
    public TMP_Text rewardCountText;
    public AudioClip beepSound, buzzerSound;
// ===============PRIVATE VARIABLES===============
    private GameObject infoBoxPrefabSmall;
    private GameObject spawnManagerFloor, eventManagerBody;
    private AudioSource audioSource;
    private MazeLoader loader;
    private float spawnRate, initialSpawnRate = 30.0f;
    private float volUp = 1.0f, volDown = 0.6f;
    private int numberOfRewards;
    private string eventMessage;
    private List<string> objectSpawnLocations = new List<string>();
    private bool spawnedPortal, isPortalSpawned, eventBoxedDisplayed = false, hasDisplayedObjectiveManager;

    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        loader = GameObject.Find("Maze Loader Holder").GetComponent<MazeLoader>();
        isPortalSpawned = false;
        if(LevelDifficulty.levelDifficulty >= 2){ // do not spawn collectibles till level 2.
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
                if(LevelDifficulty.levelDifficulty == 4 && !hasDisplayedObjectiveManager){
                    // spawn enemy when player, takes the first collectible reward.
                    StartCoroutine(SpawnEnemy(2));
                    GameObject.Find("Manager").GetComponent<GameManager>().showObjectivePanel();
                    hasDisplayedObjectiveManager = true;
                }
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
            spawnAtRandomLocation(portal, -0.9f);
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
        numberOfRewards = LevelDifficulty.levelDifficulty;
        if(LevelDifficulty.levelDifficulty == 2){
            foreach (GameObject reward in rewards)
            {
                spawnAtRandomLocation(reward);
            }
            return;
        }

        for (int i = 0; i < LevelDifficulty.levelDifficulty; i++)
        {
            spawnAtRandomLocation(rewards[Random.Range(0, 3)]);
        }
    }

    /// <summary>
    /// Spawns a number of rewards according to the level difficulty.
    /// </summary>
    void spawnGlitchedReward(int numberOfGlitchedObjectsToSpawn)
    {
        for (int i = 0; i < numberOfGlitchedObjectsToSpawn; i++)
        {
            spawnAtRandomLocation(glitchedGameObjects[Random.Range(0, 2)]);
        }
    }    

    /// <summary>
    /// Spawns the rewards game object, in a random cell.
    /// </summary>
    void spawnAtRandomLocation(GameObject gameObject, float ySpawnPoint = -0.4f)
    {
        string cell = generateUnvisitedSpawnPoint();
        Vector3 floorPosition = GameObject.Find(cell).transform.position;
        Instantiate(gameObject, new Vector3(floorPosition.x, ySpawnPoint, floorPosition.z), transform.rotation);
        objectSpawnLocations.Add(cell);
        if(gameObject.CompareTag("Portal") && LevelDifficulty.levelDifficulty > 1){
            eventMessage = "Portal spawned at cell location: (" + cell.Split(' ')[1] + ").";
            isPortalSpawned = true;
        }
    }

    private string generateUnvisitedSpawnPoint()
    {
        while (true)
        {
            string possibleSpawnPoint = "Floor " + Random.Range(0, (loader.getRowAndColumnNumber() - 1)) + "," + Random.Range(0, (loader.getRowAndColumnNumber() - 1));
            if (!objectSpawnLocations.Contains(possibleSpawnPoint))
            {
                return possibleSpawnPoint;
            }
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
        StartCoroutine(SpawnEnemy(LevelDifficulty.levelDifficulty + 2));
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
        // if(LevelDifficulty.levelDifficulty < 12){
        //     spawnRate = initialSpawnRate*(Mathf.Pow((1 - 0.08f), LevelDifficulty.levelDifficulty));
        // }else{
            spawnRate = 15.0f;
        // }

        spawnRewardsByLevelDifficulty();

        //add spawned glitched gameobjects
        if (LevelDifficulty.levelDifficulty >= 25 && LevelDifficulty.levelDifficulty < 33)
        {
            int numberOfGlitchedObjectsToSpawn = 2;
            spawnGlitchedReward(numberOfGlitchedObjectsToSpawn);
        }

        updateRewardCountText();

        //wait X amount of seconds before spawning enemies
        yield return new WaitForSeconds(4);

        if(LevelDifficulty.levelDifficulty > 3){
            spawnEnemiesByLevelDifficulty();

            //spawn health dock, if level difficulty > 6.
            if(LevelDifficulty.levelDifficulty > 5){
                spawnAtRandomLocation(healthDock, -0.9f);
            }
        }
    }

    /// <summary>
    /// A coroutine for timing the spawning of Enemy gameObjects.
    /// </summary>
    IEnumerator SpawnEnemy(int numberOfEnemiesToSpawn)
    {
        for (int i = 0; i < numberOfEnemiesToSpawn; i++)
        {
            // string spawnManagerCell = "Floor " + Random.Range(0, (loader.getRowAndColumnNumber() - 1)) + "," + Random.Range(0, (loader.getRowAndColumnNumber() - 1));
            string spawnManagerCell = generateUnvisitedSpawnPoint();
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
        // todo: play blinking buzzer audio.
        // todo: play the beep sound here.
        float vol = Random.Range(volDown, volUp);
        for (int blinkNumber = 9, i = 0; blinkNumber >= i; i++)
        {
            yield return new WaitForSeconds(2);

            if (rend.sharedMaterial == materials[0])
            {
                rend.sharedMaterial = materials[enemyType + 1];
                // audioSource.PlayOneShot(beepSound);
            }
            else
            {
                // change back to ground material
                rend.sharedMaterial = materials[0];
            }
        }
        rend.sharedMaterial = materials[0];

        // todo: play the buzzer sound  
        // audioSource.PlayOneShot(buzzerSound, vol);

        // todo: create a smoke prefab
        // todo: add audio for enemy instantiation.
        spawnEnemy(enemyArr[enemyType], spawnManagerFloor.transform.position + new Vector3(0, 2, 0), Quaternion.Euler(0.0f, 90.0f, 0.0f));
    }

    public IEnumerator spawnTeleportedEnemies(){
        yield return new WaitForSeconds(2.0f);
        for (int i = 0; i < LevelDifficulty.objectsToTeleport.Count; i++)
        {
            // string teleportationFloor = "Floor " + (loader.getRowAndColumnNumber()-1) + ", " + (loader.getRowAndColumnNumber()-1);
            string teleportationFloor =  generateUnvisitedSpawnPoint();
            GameObject teleportationFloorGameObj = GameObject.Find(teleportationFloor);
            Vector3 enemySpawnPoint = new Vector3(14, 10, 14);
            Instantiate(simulateTeleportedEnemies(LevelDifficulty.objectsToTeleport[i]), enemySpawnPoint, enemyArr[0].transform.rotation);
            yield return new WaitForSeconds(0.5f);
        }
        // empty list.
        LevelDifficulty.objectsToTeleport.RemoveAll(checkIfToRemove);
    }

    private GameObject simulateTeleportedEnemies(EnemyType enemyType){
        // returns a gameObject based on the enemyType received.
        return enemyType switch
        {
            EnemyType.blue => enemyArr[0],
            EnemyType.green => enemyArr[1],
            EnemyType.red => enemyArr[2],
            EnemyType.yellow => enemyArr[3],
            _ => null
        };
    }

    private bool checkIfToRemove(EnemyType enemyType){
        return ((int)enemyType < 5);
    }
}
