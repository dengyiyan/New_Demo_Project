using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitAspectRatio : MonoBehaviour
{
    private float aspectRatio;
    private RectTransform rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();

        if (GameStateManager.aspectRatio != -1)
        {
            FitPhotoAspectRatio();
        }
    }

    private void OnEnable()
    {
        EventHandler.LoadPhotoEvent += FitPhotoAspectRatio;
        EventHandler.ImageSavedEvent += FitPhotoAspectRatio;
    }

    private void OnDisable()
    {
        EventHandler.LoadPhotoEvent -= FitPhotoAspectRatio;
        EventHandler.ImageSavedEvent -= FitPhotoAspectRatio;
    }

    private void FitPhotoAspectRatio()
    {
        aspectRatio = GameStateManager.aspectRatio;
         

        if (aspectRatio > 1) // Wider than tall
        {
            if (rectTransform)
                rectTransform.sizeDelta = new Vector2(200, 200 / aspectRatio);
        }
        else // Taller than wide or square
        {
            if (rectTransform)
                rectTransform.sizeDelta = new Vector2(200 * aspectRatio, 200);
        }
    }
}
