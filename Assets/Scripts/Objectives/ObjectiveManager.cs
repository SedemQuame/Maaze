using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class ObjectiveManager : MonoBehaviour
{
    public TMP_Text worldInfoText;
    public TMP_Text instructionText;
    public GameObject jsonReader;
    private JSONReader reader;
    private LevelList levelList;
    private Levels level;
    private WorldInfo worldInfo;
    private Instructions instructions;
    private SpecialRule specialRule;

    public void Start(){
        // reader = jsonReader.GetComponent<JSONReader>();
        // levelList = reader.levelList;
        // level = levelList.levels[LevelDifficulty.levelDifficulty];
    }

    public void populateObjectiveMenu(){
        reader = jsonReader.GetComponent<JSONReader>();
        levelList = reader.levelList;
        level = levelList.levels[LevelDifficulty.levelDifficulty];

        // ====================================================
        worldInfoText.text = level.worldInfo.text;
        instructionText.text = level.instructions.common[0].text; //todo: iterate over the common array, create texts set information to it
        instructionText.text = level.instructions.laptop[0].text; //todo: iterate over the laptop array, create texts set information to it
    }
}
