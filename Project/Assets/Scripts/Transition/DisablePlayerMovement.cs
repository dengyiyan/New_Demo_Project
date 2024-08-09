using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisablePlayerMovement : MonoBehaviour
{

    //void Start()
    //{
    //    EventHandler.CallDisablePlayerMovementEvent();
    //}

    private void Start()
    {
        EventHandler.CallIncreaseDisableEvent();
    }

    private void OnDestroy()
    {
        EventHandler.CallDecreaseDisableEvent();
    }

}
