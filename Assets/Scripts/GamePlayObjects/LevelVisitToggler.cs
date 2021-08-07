using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// get list of all locked icons, get value of the parent's text message.
// if the int casting is greater than the current level, hide the locked icon.
// make the button interactive
public class LevelVisitToggler : MonoBehaviour
{
    private GameObject[] lockedLevels;

    // Start is called before the first frame update
    void Start()
    {
        lockedLevels = GameObject.FindGameObjectsWithTag("Locked Icon");
    }

    void FixedUpdate(){
        foreach (GameObject lockedLevel in lockedLevels)
        {
            // get lockedLevel's parent panel element.
            var button = (lockedLevel.transform.parent).parent;

            // get the name of the button.
            int buttonLevel = int.Parse(button.name);

            if (buttonLevel <= LevelDifficulty.maxLevelReached){
                // make button interactable
                button.GetComponent<Button>().interactable = true;
                // disable the level button
                lockedLevel.SetActive(false);
            }
        }
    }

}
