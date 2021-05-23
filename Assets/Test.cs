using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        HealthBarControl.SetHealthBarValue(1);
    }

    void updateHealthBar(float healthPoints)
    {
        HealthBarControl.SetHealthBarValue(HealthBarControl.GetHealthBarValue() - (0.01f * healthPoints));
    }
}
