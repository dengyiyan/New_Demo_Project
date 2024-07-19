using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadManger : MonoBehaviour
{
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoad;
    }

    private void OnDestroy()
    {
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoad;
    }

    private void OnAfterSceneLoad()
    {
        GameStateManager.DestroyPickedItems();
    }
}
