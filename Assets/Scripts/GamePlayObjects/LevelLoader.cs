using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    private Scene scene;
    private int waitTime;
    private int nextSceneIndex;
    public Animator transition;


    // Start is called before the first frame update
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
