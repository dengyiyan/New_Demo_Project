using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PhotoManager : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private GameObject photoPanel;
    [SerializeField] private ImageType defaultImageType;
    [SerializeField] private Button photoPanelButton;
    [SerializeField] private Button photoPanelCloseButton;
    private bool isShowing = false;
    private bool disabled = false;

    private void OnEnable()
    {
        EventHandler.ShowImageEvent += OnShowImageEvent;
        EventHandler.HideImageEvent += OnHideImageEvent;
    }

    private void OnDestroy()
    {
        EventHandler.ShowImageEvent -= OnShowImageEvent;
        EventHandler.HideImageEvent -= OnHideImageEvent;
    }

    private void Start()
    {
        PhotoPanelOpenSettings();
        PhotoPanelClosedSettings();
        photoPanelButton.onClick.AddListener(OnPhotoButtonClicked);
        photoPanelCloseButton.onClick.AddListener(OnCloseButtonClicked);
    }

    private void OnShowImageEvent(ImageType type)
    {
        if (!isShowing)
        {
            PhotoPanelOpenSettings();
        }
        LoadImage(GameStateManager.GetImage(type));
    }

    private void OnHideImageEvent()
    {
        PhotoPanelClosedSettings();
    }

    private void OnPhotoButtonClicked()
    {
        PhotoPanelOpenSettings();
        OnShowImageEvent(defaultImageType);
    }

    private void OnCloseButtonClicked()
    {
        PhotoPanelClosedSettings();
    }

    private void PhotoPanelClosedSettings()
    {
        isShowing = false;
        photoPanel.SetActive(false);
        photoPanelButton.gameObject.SetActive(true);
        EventHandler.CallEnableCursorEvent();
        // EventHandler.CallEnablePlayerMovementEvent();
    }

    private void PhotoPanelOpenSettings()
    {
        isShowing = true;
        photoPanel.SetActive(true);
        photoPanelButton.gameObject.SetActive(false);
        EventHandler.CallDisableCursorEvent();
        // EventHandler.CallDisablePlayerMovementEvent();
    }

    private void LoadImage(Texture2D texture)
    {
        if (texture)
        {
            Sprite newSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            image.sprite = newSprite;

            // Adjust the Image component's rectTransform to match the aspect ratio of the uploaded image
            float aspectRatio = (float)texture.width / texture.height;
            RectTransform rectTransform = image.GetComponent<RectTransform>();

            if (aspectRatio > 1) // Wider than tall
            {
                rectTransform.sizeDelta = new Vector2(200, 200 / aspectRatio);
            }
            else // Taller than wide or square
            {
                rectTransform.sizeDelta = new Vector2(200 * aspectRatio, 200);
            }

        }
        else
        {
            Debug.LogWarning("Image does not exist");
        }
    }
}
