using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowSpeed : MonoBehaviour
{
    private void OnEnable()
    {
        EventHandler.CallSetSlowSpeedEvent();
    }

    private void OnDisable()
    {
        EventHandler.CallSetDefaultSpeedEvent();
    }
}
