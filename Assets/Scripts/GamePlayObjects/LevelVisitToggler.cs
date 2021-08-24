using UnityEngine;
using UnityEngine.UI;

// get list of all locked icons, get value of the parent's text message.
// if the int casting is greater than the current level, hide the locked icon.
// make the button interactive
public class LevelVisitToggler : MonoBehaviour
{
    public Sprite clearedLevel, presentLevel;
    private GameObject[] gameLevels;

    // Start is called before the first frame update
    void Start()
    {
        gameLevels = GameObject.FindGameObjectsWithTag("Level Number");
    }

    void FixedUpdate(){
        foreach (GameObject lockedLevel in gameLevels)
        {
            // get the name of the button.
            int buttonLevel = int.Parse(lockedLevel.transform.name);
            if(PlayerPrefs.HasKey("maxLevelReached")){
                LevelDifficulty.maxLevelReached = PlayerPrefs.GetInt("maxLevelReached");
            }

            if (buttonLevel <= LevelDifficulty.maxLevelReached){
                // make button interactable
                lockedLevel.GetComponent<Button>().interactable = true;
                
                // get locked icon
                lockedLevel.transform.GetChild(1).gameObject.SetActive(false);

                // change source image for GetComponent Image
                if (buttonLevel == LevelDifficulty.maxLevelReached)
                {
                    lockedLevel.GetComponent<Image>().sprite = presentLevel;
                }else
                {                    
                    lockedLevel.GetComponent<Image>().sprite = clearedLevel;
                }
            }

        }
    }

}
