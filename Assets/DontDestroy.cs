using UnityEngine;

/// <summary>
/// The DontDestroy class ensures that only one instance of the music gameobject exists in a scene.
/// </summary>
public class DontDestroy : MonoBehaviour
{
    void Awake()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("music");
        if (objects.Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
