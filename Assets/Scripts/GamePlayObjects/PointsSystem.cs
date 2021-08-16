using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class PointsSystem
{

    public static float points = 0;
    public static void updatePointSystem(float killPoints, GameObject pointText){
        points += killPoints;
        pointText.GetComponent<TextMeshProUGUI>().text = ("00" + points);
    }
}
