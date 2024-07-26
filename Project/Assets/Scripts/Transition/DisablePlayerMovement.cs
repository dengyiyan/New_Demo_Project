using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisablePlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EventHandler.CallDisablePlayerMovementEvent();
    }
}
