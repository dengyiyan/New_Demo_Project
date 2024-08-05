using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClick : MonoBehaviour
{
    private Button myButton;
    [SceneName] public string SceneTo;
    public string SpawnID;

    void Awake()
    {
        myButton = GetComponent<Button>();
        if (myButton != null)
        {
            myButton.onClick.AddListener(OnButtonClick);
        }
        else
        {
            Debug.LogError("Button component not found on this GameObject.");
        }
    }

    void OnButtonClick()
    {
        EventHandler.CallTransitionEvent(SceneTo, SpawnID);
    }
}
