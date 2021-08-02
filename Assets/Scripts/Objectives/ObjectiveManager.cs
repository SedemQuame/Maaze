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
    public RawImage rawImage;
    private JSONReader reader;
    private LevelList levelList;
    private Levels level;
    private WorldInfo worldInfo;
    private Instructions instructions;
    private SpecialRule specialRule;
    private GameObject infoBoxPrefabSmall;
    private GameObject infoBoxPrefabMedium;
    private GameObject worldItemsPanel;
    private GameObject itemBox;

    public void populateObjectiveMenu(){
        // Loading resource prefabs
        infoBoxPrefabSmall = Resources.Load("UI/InfoBox_Small") as GameObject;
        infoBoxPrefabMedium = Resources.Load("UI/InfoBox_Medium") as GameObject;
        worldItemsPanel = Resources.Load("UI/World_Items_Panel") as GameObject;
        itemBox = Resources.Load("UI/ItemBox") as GameObject;        

        // loading json data.
        reader = jsonReader.GetComponent<JSONReader>();
        levelList = reader.levelList;
        Debug.Log(levelList.levels);
        level = levelList.levels[(LevelDifficulty.levelDifficulty-1)];


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

        // instructions
        instructionInfoBox.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Instructions";
        instructionInfoBox.transform.GetChild(1).transform.GetChild(0).gameObject.GetComponent<Text>().text = instructionOutput; 

        // world items
        GameObject worldPanel = Instantiate(worldItemsPanel);
        worldPanel.transform.SetParent(objectiveManagerBody.transform, false);

        string [] imageArr = null;
        #if UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBGL
            if (level.instructions.common.Length > 0)
            {
                imageArr = level.instructions.common[0].images;
            }else{
                imageArr = level.instructions.laptop[0].images;
            }
         #elif UNITY_IOS || UNITY_ANDROID
            imageArr = level.instructions.mobile[0].images;
        #endif

        

        foreach (string image in imageArr)
        {
            GameObject itemBoxGameObject = Instantiate(itemBox);
            // append item 
            itemBoxGameObject.transform.SetParent(worldPanel.transform.GetChild(1).transform, false);

            // set image and text
            Sprite sprite = Resources.Load<Sprite>(image);
            itemBoxGameObject.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
            Debug.Log(itemBoxGameObject.transform.GetChild(0).name);
        }

        // Special Rule Info Box
        if(level.specialRule.text != ""){
            GameObject specialRuleInfoBox = Instantiate(infoBoxPrefabMedium);
            specialRuleInfoBox.transform.SetParent(objectiveManagerBody.transform, false);
            specialRuleInfoBox.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Special Rules";
            specialRuleInfoBox.transform.GetChild(1).transform.GetChild(0).gameObject.GetComponent<Text>().text =  level.specialRule.text; 
        }
        
    }
}
