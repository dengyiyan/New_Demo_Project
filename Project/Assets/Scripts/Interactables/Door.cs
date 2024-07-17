using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    [SceneName]
    [SerializeField] private string sceneTo;
    [SerializeField] private string spawnID;

    public void Interact()
    {
        EventHandler.CallTransitionEvent(sceneTo, spawnID);
    }
}
