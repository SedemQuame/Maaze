using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONReader : MonoBehaviour
{
    public TextAsset textJSON;
    public LevelList levelList = new LevelList();

    // Start is called before the first frame update
    void Start()
    {
        levelList = JsonUtility.FromJson<LevelList>(textJSON.text);
    }
}
