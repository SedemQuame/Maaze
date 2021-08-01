using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class ObjectiveManager : MonoBehaviour
{
    // public TMP_Text worldInfoText;
    // public TMP_Text instructionText;
    public GameObject objectiveManagerBody;
    public GameObject jsonReader;
    private JSONReader reader;
    private LevelList levelList;
    private Levels level;
    private WorldInfo worldInfo;
    private Instructions instructions;
    private SpecialRule specialRule;
    private GameObject infoBoxPrefabSmall;
    private GameObject infoBoxPrefabMedium;

    public void populateObjectiveMenu(){
        infoBoxPrefabSmall = Resources.Load("UI/InfoBox_Small") as GameObject;
        infoBoxPrefabMedium = Resources.Load("UI/InfoBox_Medium") as GameObject;

        reader = jsonReader.GetComponent<JSONReader>();
        levelList = reader.levelList;
        level = levelList.levels[(LevelDifficulty.levelDifficulty - 1)];

        // ====================================================
        // World Info Box
        GameObject worldInfoBox = Instantiate(infoBoxPrefabSmall);
        worldInfoBox.transform.SetParent(objectiveManagerBody.transform, false);
        // set the text and image.
        worldInfoBox.transform.GetChild(0).gameObject.GetComponent<Text>().text = "World Information";
        worldInfoBox.transform.GetChild(1).transform.GetChild(0).gameObject.GetComponent<Text>().text = level.worldInfo.text; 

        // Instruction Info Box
        GameObject instructionInfoBox = Instantiate(infoBoxPrefabMedium);
        instructionInfoBox.transform.SetParent(objectiveManagerBody.transform, false);
        
        string instructionOutput = "";
        if(level.instructions.common.Length > 0){
            instructionOutput = level.instructions.common[0].text;
        }else{
                // Check if we are running on a pc, web or unity editor
             #if UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBGL
                instructionOutput = level.instructions.laptop[0].text;
                // Check if we are running on a mobile device 
            #elif UNITY_IOS || UNITY_ANDROID
                instructionOutput = level.instructions.mobile[0].text;
            #endif
        }

        instructionInfoBox.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Instructions";
        instructionInfoBox.transform.GetChild(1).transform.GetChild(0).gameObject.GetComponent<Text>().text = instructionOutput; 

        // Special Rule Info Box
        if(level.specialRule.text != ""){
            GameObject specialRuleInfoBox = Instantiate(infoBoxPrefabMedium);
            specialRuleInfoBox.transform.SetParent(objectiveManagerBody.transform, false);
            specialRuleInfoBox.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Special Rules";
            specialRuleInfoBox.transform.GetChild(1).transform.GetChild(0).gameObject.GetComponent<Text>().text =  level.specialRule.text; 
        }
        
    }
}
