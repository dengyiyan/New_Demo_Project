using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateButton : MonoBehaviour
{
    public ImageType type;
    public string promptText;
    public int loraWeightIndex;
    //public string wildcardText;

    private Button button;
    void Start()
    {
        button = GetComponent<Button>();
        SetPrompt(type);

        GetComponent<Button>().onClick.AddListener(OnButtonClick);
        CheckInteractable();
    }

    private void OnEnable()
    {
        EventHandler.LoadPhotoFinishEvent += CheckInteractable;
    }

    private void OnDisable()
    {
        EventHandler.LoadPhotoFinishEvent -= CheckInteractable;
    }

    private void SetPrompt(ImageType t)
    {
        switch (t)
        {
            case ImageType.Happy:
                promptText = "happy expression, eyes open";
                loraWeightIndex = 1;
                break;
            case ImageType.Sad:
                promptText = "sad expression";
                loraWeightIndex = 5;
                break;
            case ImageType.Angry:
                promptText = "angry expression";
                loraWeightIndex = 2;
                //wildcardText = "large eyes, angry";
                break;
            case ImageType.Surprised:
                promptText = "shocked expression, eyes open wide, big mouth";
                loraWeightIndex = 4;
                break;
            case ImageType.Disgusted:
                promptText = "disgusted expression";
                loraWeightIndex = 6;
                break;
            case ImageType.Fearful:
                promptText = "scared expression";
                loraWeightIndex = 3;
                break;
            case ImageType.Neutral:
                promptText = "expressionless";
                loraWeightIndex = -1;
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
