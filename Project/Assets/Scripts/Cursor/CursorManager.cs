using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Sprite defaultCursor, normalInteractCursor, investCursor, dialogueCursor;
    public Vector2 hotspot = Vector2.zero;

    private Sprite currentSprite;
    private float currentTransparency;

    private Sprite previousSprite;
    private float previousTransparency;

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

    private void Start()
    {
        previousSprite = defaultCursor;
        previousTransparency = 1f;
        SetCursor(defaultCursor, 1f);
    }

    private void Update()
    {
        if (cursorEnable && (currentSprite != previousSprite || currentTransparency != previousTransparency))
        {
            //Debug.Log(currentSprite);
            //Debug.Log(currentTransparency);
            SetCursor(currentSprite, currentTransparency);
            previousSprite = currentSprite;
            previousTransparency = currentTransparency;
        }
        else if (!cursorEnable)
        {
            SetCursor(defaultCursor, Settings.validCursorTransparency);
        }

    }

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

    private void SetCursor(Sprite cursorSprite, float transparency)
    {
        if (cursorSprite)
        {
            Texture2D cursorTexture = ConvertSpriteToTexture2D(cursorSprite);
            AdjustCursorTransparency(cursorTexture, transparency);
            Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
        }
    }

    private void OnBeforeSceneUnloadEvent()
    {
        SetCursorDisable();
    }

    private void SetCursorDisable()
    {
        SetCursor(defaultCursor, Settings.validCursorTransparency);
        cursorEnable = false;
    }

    private void OnAfterSceneLoadEvent()
    {
        SetCursorEnable();
    }

    private void SetCursorEnable()
    {
        SetCursor(defaultCursor, Settings.validCursorTransparency);
        cursorEnable = true;
    }


    private void OnCursorChange(InteractionType type, float transparency)
    {
        currentSprite = type switch
        {
            InteractionType.NPC => dialogueCursor,
            InteractionType.Invest => investCursor,
            InteractionType.Door => normalInteractCursor,
            InteractionType.Normal => normalInteractCursor,
            _ => defaultCursor
        };

        currentTransparency = transparency;
    }
}
