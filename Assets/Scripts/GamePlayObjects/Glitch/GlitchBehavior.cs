using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlitchBehavior : MonoBehaviour
{
    private Vector3 currentPosition, nextPosition;
    private MazeLoader loader;
    private int teleportationTime;
    // Start is called before the first frame update
    void Start()
    {
        loader = GameObject.Find("Maze Loader Holder").GetComponent<MazeLoader>();
        // teleportationTime = Random.Range(10, 15);
        teleportationTime = 15;
        StartCoroutine(Teleport());
    }

    IEnumerator Teleport(){
        while (true)
        {
            yield return new WaitForSeconds(teleportationTime);
            string cell = "Floor " + Random.Range(0, (loader.getRowAndColumnNumber() - 1)) + "," + Random.Range(0, (loader.getRowAndColumnNumber() - 1));
            Vector3 newSpawnPoint = GameObject.Find(cell).transform.position;
            this.transform.position = newSpawnPoint;
        }
    }
}
