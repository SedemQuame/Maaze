using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class ObjectiveManager : MonoBehaviour
{
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

        // instructions
        instructionInfoBox.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Instructions";
        instructionInfoBox.transform.GetChild(1).transform.GetChild(0).gameObject.GetComponent<Text>().text = instructionOutput;       

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
            string itemName = (((image.ToString()).Split('/')[3].Split('.')[0]).Replace("_", " "));
            itemPanel.text = itemName;
        }

        // Special Rule Info Box
        if(level.specialRule.text != ""){
            GameObject specialRuleInfoBox = Instantiate(infoBoxPrefabMedium);
            specialRuleInfoBox.transform.SetParent(objectiveManagerBody.transform, false);
            specialRuleInfoBox.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Special Rules";
            specialRuleInfoBox.transform.GetChild(1).transform.GetChild(0).gameObject.GetComponent<Text>().text =  level.specialRule.text; 
        }
        
    }

    private Texture2D createTextureUsingFileName(string image){
        var rawData = System.IO.File.ReadAllBytes(image);
        Texture2D imageTexture = new Texture2D(2, 2);
        imageTexture.LoadImage(rawData);
        return imageTexture;
    }
}
