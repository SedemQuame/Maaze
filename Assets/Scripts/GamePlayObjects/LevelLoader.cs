using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    // ===============PUBLIC VARIABLES===============
    public Animator transition;
    
    // ===============PRIVATE VARIABLES===============
    private Scene scene;
    private int waitTime,  nextSceneIndex;

    void Start()
    {
        scene = SceneManager.GetActiveScene();
        waitTime = 1;
        nextSceneIndex = scene.buildIndex + 1;
    }

    void Update()
    {
        if(scene.buildIndex != 1)
        {
            StartCoroutine(ChangeSplashScreen(nextSceneIndex));
        }
    }

    public IEnumerator ChangeSplashScreen(int sceneIndex)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(sceneIndex);
    }
}
