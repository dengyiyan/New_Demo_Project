using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateButton : MonoBehaviour
{
    public ImageType type;
    public string promptText;  // The prompt text associated with this button

    private Button button;
    void Start()
    {
        button = GetComponent<Button>();
        SetPrompt(type);
        // Ensure the button has an onClick listener
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
        CheckInteractable();
    }

    private void OnEnable()
    {
        EventHandler.LoadPhotoEvent += CheckInteractable;
    }

    private void OnDisable()
    {
        EventHandler.LoadPhotoEvent -= CheckInteractable;
    }

    private void SetPrompt(ImageType t)
    {
        switch (t)
        {
            case ImageType.Happy:
                promptText = ", happy face";
                break;
            case ImageType.Sad:
                promptText = ", sad face";
                break;
            case ImageType.Angry:
                promptText = ", angry face";
                break;
            case ImageType.Surprised:
                promptText = ", surprised face";
                break;
            case ImageType.Disgusted:
                promptText = ", disgusted face";
                break;
            case ImageType.Fearful:
                promptText = ", fearful face";
                break;
            case ImageType.Neutral:
                promptText = ", neutral face";
                break;
        }
    }

    private void CheckInteractable()
    {
        if (GameStateManager.GetUploadedImage() == null)
        {
            if (button)
                button.interactable = false;
        }
        else
        {
            if (button)
                button.interactable = true;
        }
    }

    private void OnButtonClick()
    {
        EventHandler.CallSetGenerateButtonEvent(this);
        EventHandler.CallSetDisplayingExpressionEvent(type);
    }
}
