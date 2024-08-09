using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementManger : MonoBehaviour
{
    public int constraintNumber = 1;

    private void OnEnable()
    {
        EventHandler.IncreaseDisableEvent += OnIncreaseDisableEvent;
        EventHandler.DecreaseDisableEvent += OnDecreaseDisableEvent;
    }

    private void OnDisable()
    {
        EventHandler.IncreaseDisableEvent -= OnIncreaseDisableEvent;
        EventHandler.DecreaseDisableEvent -= OnDecreaseDisableEvent;
    }

    private void Start()
    {
        OnNumberChange();
    }

    private void OnIncreaseDisableEvent()
    {
        constraintNumber += 1;
        OnNumberChange();
    }

    private void OnDecreaseDisableEvent()
    {
        constraintNumber -= 1;
        OnNumberChange();
    }



    private void OnNumberChange()
    {
        if (constraintNumber > 0)
        {
            EventHandler.CallDisablePlayerMovementEvent();
        }
        else
        {
            EventHandler.CallEnablePlayerMovementEvent();
        }
    }

}
