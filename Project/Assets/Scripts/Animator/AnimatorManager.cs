using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    private Dictionary<string, Animator> animators = new Dictionary<string, Animator>();

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
    }

    private void OnAfterSceneLoadEvent()
    {
        OnRegisterAnimatorEvent();

    }

    private void OnBeforeSceneUnloadEvent()
    {
        animators.Clear();

    }


    private void OnRegisterAnimatorEvent()
    {
        foreach (var ani in FindObjectsOfType<Animator>())
        {
            animators[ani.gameObject.name] = ani;
            //Debug.Log(ani.gameObject.name);
        }

        EventHandler.CallRegisterAnimatorEvent(animators);

        // animators["Player"] = animator;
    }


}
