using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetSettingScript : MonoBehaviour
{

    public SO_BodyPart[] allParts;

    private void Awake()
    {
        foreach (var part in allParts)
        {
            part.resetColor();
        }


        EventHandler.CallUpdateColorEvent();
    }
}
