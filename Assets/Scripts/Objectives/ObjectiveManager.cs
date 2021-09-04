using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class ObjectiveManager : MonoBehaviour
{
    // ===============PUBLIC VARIABLES===============
    public GameObject objectiveManagerBody, jsonReader;
    public RawImage rawImage;
    // ===============PRIVATE VARIABLES===============
    private JSONReader reader;
    private LevelList levelList;
    private Levels level;
    private WorldInfo worldInfo;
    private Instructions instructions;
    private SpecialRule specialRule;
    private GameObject infoBoxPrefabSmall, infoBoxPrefabMedium, worldItemsPanel, itemBox;

    void loadUIGameObjectResources(){
        infoBoxPrefabSmall = Resources.Load("UI/InfoBox_Small") as GameObject;
        infoBoxPrefabMedium = Resources.Load("UI/InfoBox_Medium") as GameObject;
        worldItemsPanel = Resources.Load("UI/World_Items_Panel") as GameObject;
        itemBox = Resources.Load("UI/ItemBox") as GameObject; 
    }

    void loadJsonDataForLevel(){
        reader = jsonReader.GetComponent<JSONReader>();
        levelList = reader.levelList;
        level = levelList.levels[(LevelDifficulty.levelDifficulty-1)];
    }

    public void populateObjectiveMenu(){   
        loadUIGameObjectResources();
        loadJsonDataForLevel();

        // ====================================================
        // World Info Box
        GameObject worldInfoBox = Instantiate(infoBoxPrefabSmall);
        worldInfoBox.transform.SetParent(objectiveManagerBody.transform, false);

        // set the text and image.
        worldInfoBox.transform.GetChild(0).gameObject.GetComponent<Text>().text = "World Information";
        // format worldInfo text depending on certain placeholders.
        worldInfoBox.transform.GetChild(1).transform.GetChild(0).gameObject.GetComponent<Text>().text = formatWorldInfoTextMessage(level.worldInfo.text); 

        // Instruction Info Box
        GameObject instructionInfoBox = Instantiate(infoBoxPrefabMedium);

        // World items Info Box
        GameObject worldPanel = Instantiate(worldItemsPanel);
        worldPanel.transform.SetParent(objectiveManagerBody.transform, false);
        
        string instructionOutput = "";
        var imageArr = new List<string>{};
        if((level.instructions.common != null) && (level.instructions.common.Length > 0)){
            instructionOutput = level.instructions.common[0].text;
            imageArr = (level.instructions.common[0].images).ToList();
        }else{
                // Check if we are running on a pc, web or unity editor
             #if UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBGL
                if((level.instructions.laptop != null) && (level.instructions.laptop.Length > 0)){
                    instructionOutput = level.instructions.laptop[0].text;
                    imageArr = (level.instructions.laptop[0].images).ToList();
                }
                // Check if we are running on a mobile device 
            #elif UNITY_IOS || UNITY_ANDROID
                if((level.instructions.mobile != null) && (level.instructions.mobile.Length > 0)){             
                    instructionOutput = level.instructions.mobile[0].text;
                    imageArr = (level.instructions.mobile[0].images).ToList();
                }
            #endif
        }

        if(instructionOutput != ""){
            // instructions
            instructionInfoBox.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Instructions";
            instructionInfoBox.transform.GetChild(1).transform.GetChild(0).gameObject.GetComponent<Text>().text = instructionOutput;
            instructionInfoBox.transform.SetParent(objectiveManagerBody.transform, false);
        }

        if(imageArr.Count() > 0)
        foreach (string image in imageArr)
        {
            GameObject itemBoxGameObject = Instantiate(itemBox);

            // append item 
            itemBoxGameObject.transform.SetParent(worldPanel.transform.GetChild(1).transform, false);

            // set item image
            RawImage itemImage = itemBoxGameObject.transform.GetChild(0).GetComponent<RawImage>();
            itemImage.texture = createTextureUsingFileName(image);

            // set item name
            Text itemPanel = itemBoxGameObject.transform.GetChild(1).GetChild(0).GetComponent<Text>();
            string itemName = (((image.ToString()).Split('/')[1].Split('.')[0]).Replace("_", " "));
            TextInfo myTI = new CultureInfo("en-US",false).TextInfo;
            itemPanel.text = myTI.ToTitleCase(itemName);
        }

        // Special Rule Info Box
        specialRuleBoxCreator(level.specialRule.text);
    }

    private void specialRuleBoxCreator(string levelSpecialRule){
        if(levelSpecialRule != ""){
            GameObject specialRuleInfoBox = Instantiate(infoBoxPrefabMedium);
            specialRuleInfoBox.transform.SetParent(objectiveManagerBody.transform, false);
            specialRuleInfoBox.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Special Rules";
            specialRuleInfoBox.transform.GetChild(1).transform.GetChild(0).gameObject.GetComponent<Text>().text =  levelSpecialRule; 
        }
    }

    private string formatWorldInfoTextMessage(string worldInfoMsg){
        if (!worldInfoMsg.Contains("expanded"))
        {
            return worldInfoMsg;
        }
        int mazeRowColumn =  GameObject.Find("Maze Loader Holder").GetComponent<MazeLoader>().getRowAndColumnNumber();
        string formattedMsg = string.Format(worldInfoMsg, mazeRowColumn, mazeRowColumn);
        return formattedMsg;
    }

    private Texture2D createTextureUsingFileName(string image){
        Texture2D imageTexture = Resources.Load(image.Split('.')[0]) as Texture2D;
        return imageTexture;
    }

    GameObject selectPrefabUsingTextSize(string text){
        int numberOfNewlines = text.Split('/').Length - 1;
        if (numberOfNewlines <= 1)
        {
            return infoBoxPrefabSmall;
        }

        if (numberOfNewlines >= 2)
        {
            return infoBoxPrefabMedium;
        }

        return null;
    }
}
