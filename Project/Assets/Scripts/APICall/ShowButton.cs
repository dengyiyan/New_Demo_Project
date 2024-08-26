using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowButton : MonoBehaviour
{
    public ImageType type;
    public RawImage imageDisplay;

    private Button button;
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        //imageDisplay = FindObjectOfType<RawImage>();
        if (button)
            CheckInteractable();

        button.onClick.AddListener(DisplayImage);
    }

    private void OnEnable()
    {
        EventHandler.ImageSavedEvent += CheckInteractable;
    }

    private void OnDisable()
    {
        EventHandler.ImageSavedEvent -= CheckInteractable;
    }

    private void CheckInteractable()
    {
        if (button != null)
        {
            if (GameStateManager.GetImage(type) == null)
            {
                button.interactable = false;
            }
            else
            {
                button.interactable = true;
            }

        }
    }

    private void DisplayImage()
    {
        imageDisplay.texture = GameStateManager.GetImage(type);
        CheckInteractable();
        EventHandler.CallSetDisplayingExpressionEvent(type);
    }
}
