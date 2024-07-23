using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Sprite defaultCursor, normalInteractCursor, investCursor, dialogueCursor;
    private Texture2D defaultCursorTexture, normalInteractCursorTexture, investCursorTexture, dialogueCursorTexture;
    private Texture2D invalidDefaultCursorTexture, invalidNormalInteractCursorTexture, invalidInvestCursorTexture, invalidDialogueCursorTexture;

    public Vector2 hotspot = Vector2.zero;

    private Texture2D currentTexture;
    // private float currentTransparency;

    // private Sprite previousSprite;
    // private float previousTransparency;

    private bool cursorEnable = true;

    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        EventHandler.CursorChangeEvent += OnCursorChange;

        EventHandler.EnableCursorEvent += SetCursorEnable;
        EventHandler.DisableCursorEvent += SetCursorDisable;
    }

    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        EventHandler.CursorChangeEvent -= OnCursorChange;

        EventHandler.EnableCursorEvent -= SetCursorEnable;
        EventHandler.DisableCursorEvent -= SetCursorDisable;
    }

    private void Awake()
    {
        PrepareTexture();

        SetCursor(defaultCursorTexture);
    }

    private void Start()
    {
        PrepareTexture();

        SetCursor(defaultCursorTexture);
    }

    private void PrepareTexture()
    {
        defaultCursorTexture = ConvertSpriteToTexture2D(defaultCursor);
        normalInteractCursorTexture = ConvertSpriteToTexture2D(normalInteractCursor);
        investCursorTexture = ConvertSpriteToTexture2D(investCursor);
        dialogueCursorTexture = ConvertSpriteToTexture2D(dialogueCursor);

        invalidDefaultCursorTexture = new Texture2D(defaultCursorTexture.width, defaultCursorTexture.height, defaultCursorTexture.format, false);
        invalidNormalInteractCursorTexture = new Texture2D(normalInteractCursorTexture.width, normalInteractCursorTexture.height, normalInteractCursorTexture.format, false);
        invalidInvestCursorTexture = new Texture2D(investCursorTexture.width, investCursorTexture.height, investCursorTexture.format, false);
        invalidDialogueCursorTexture = new Texture2D(dialogueCursorTexture.width, dialogueCursorTexture.height, dialogueCursorTexture.format, false);

        Graphics.CopyTexture(defaultCursorTexture, invalidDefaultCursorTexture);
        Graphics.CopyTexture(normalInteractCursorTexture, invalidNormalInteractCursorTexture);
        Graphics.CopyTexture(investCursorTexture, invalidInvestCursorTexture);
        Graphics.CopyTexture(dialogueCursorTexture, invalidDialogueCursorTexture);

        AdjustCursorTransparency(invalidDialogueCursorTexture, Settings.invalidCursorTransparency);
        AdjustCursorTransparency(invalidDefaultCursorTexture, Settings.invalidCursorTransparency);
        AdjustCursorTransparency(invalidNormalInteractCursorTexture, Settings.invalidCursorTransparency);
        AdjustCursorTransparency(invalidInvestCursorTexture, Settings.invalidCursorTransparency);

    }
    //private void Update()
    //{
    //    // Debug.Log(cursorEnable);
    //    if (!cursorEnable)
    //    {
    //        SetCursor(defaultCursor, Settings.validCursorTransparency);
    //    }
    //    else if (cursorEnable && (currentSprite != previousSprite || currentTransparency != previousTransparency))
    //    {
    //        Debug.Log(currentSprite);
    //        //Debug.Log(currentTransparency);
    //        SetCursor(currentSprite, currentTransparency);
    //        previousSprite = currentSprite;
    //        previousTransparency = currentTransparency;
    //    }

    //}

    public static Texture2D ConvertSpriteToTexture2D(Sprite sprite)
    {
        Texture2D texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height, TextureFormat.RGBA32, false);
        Color[] pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                  (int)sprite.textureRect.y,
                                                  (int)sprite.textureRect.width,
                                                  (int)sprite.textureRect.height);
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }

    private void AdjustCursorTransparency(Texture2D texture, float alpha)
    {
        Color[] pixels = texture.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i].a *= alpha;
        }
        texture.SetPixels(pixels);
        texture.Apply();
    }

    private void SetCursor(Texture2D cursor)
    {
        Debug.Log($"Setting cursor to texture {cursor}");
        if (cursor != null)
        {
            // exture2D cursorTexture = new Texture2D(cursor.width, cursor.height, cursor.format, false);
            // Graphics.CopyTexture(cursor, cursorTexture);
            // AdjustCursorTransparency(cursorTexture, transparency);
            Cursor.SetCursor(cursor, hotspot, CursorMode.Auto);
        }
    }

    private void OnBeforeSceneUnloadEvent()
    {
        SetCursor(defaultCursorTexture);
        SetCursorDisable();
    }

    private void SetCursorDisable()
    {
        SetCursor(defaultCursorTexture);
        cursorEnable = false;
    }

    private void OnAfterSceneLoadEvent()
    {
        SetCursor(defaultCursorTexture);
        SetCursorEnable();
    }

    private void SetCursorEnable()
    {
        SetCursor(defaultCursorTexture);
        // Update the cursor state immediately after enabling it
        // SetCursor(currentSprite, currentTransparency);
        cursorEnable = true;
    }


    private void OnCursorChange(InteractionType type, bool isValid)
    {
        if (cursorEnable)
        {
            currentTexture = type switch
            {
                InteractionType.NPC => isValid ? dialogueCursorTexture : invalidDialogueCursorTexture,
                InteractionType.Invest => isValid ? investCursorTexture : invalidInvestCursorTexture,
                InteractionType.Door => isValid ? normalInteractCursorTexture : invalidNormalInteractCursorTexture,
                InteractionType.Normal => isValid ? normalInteractCursorTexture : invalidNormalInteractCursorTexture,
                _ => isValid ? defaultCursorTexture : invalidDefaultCursorTexture
            };

            // currentTransparency = transparency;

            Debug.Log($"Cursor change triggered with type {type} and valid {isValid}!");
            SetCursor(currentTexture);
        }
    }
}
