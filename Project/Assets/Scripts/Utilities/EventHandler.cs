using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Playables;

public static class EventHandler
{
    public static event Action<string, string, AnimationSequence, Direction> TransitionEvent;
    public static void CallTransitionEvent(string sceneName, string spawnPointID, AnimationSequence sequence=null, Direction direction=Direction.None)
    {
        foreach (var convChecker in GameObject.FindObjectsOfType<ConversationChecker>())
        {
            convChecker.CheckTransitionStatus(sceneName, spawnPointID, sequence);
        }
        TransitionEvent?.Invoke(sceneName, spawnPointID, sequence, direction);
    }


    public static event Action<string> RegisterTipsEvent;
    public static void CallRegisterTipsEvent(string tips)
    {
        RegisterTipsEvent?.Invoke(tips);
    }


    public static event Action UnregisterTipsEvent;
    public static void CallUnregisterTipsEvent()
    {
        UnregisterTipsEvent?.Invoke();
    }

    public static event Action<string, ImageType> SetDisplayingExpressionEvent;
    public static void CallSetDisplayingExpressionEvent(string str, ImageType type)
    {
        SetDisplayingExpressionEvent?.Invoke(str, type);
    }

    public static event Action<GenerateButton> SetGenerateButtonEvent;
    public static void CallSetGenerateButtonEvent(GenerateButton b)
    {
        SetGenerateButtonEvent?.Invoke(b);
    }

    public static event Action ImageSavedEvent;
    public static void CallImageSavedEvent()
    {
        ImageSavedEvent?.Invoke();
    }

    public static event Action<string> SetServerRunningEvent;
    public static void CallSetServerRunningEvent(string ex = "")
    {
        SetServerRunningEvent?.Invoke(ex);
    }

    public static event Action<string> SetServerStopEvent;
    public static void CallSetServerStopEvent(string ex = "")
    {
        SetServerStopEvent?.Invoke(ex);
    }

    public static event Action LoadPhotoEvent;
    public static void CallLoadPhotoEvent()
    {
        LoadPhotoEvent?.Invoke();
    }

    public static event Action LoadPhotoFinishEvent;
    public static void CallLoadPhotoFinishEvent()
    {
        LoadPhotoFinishEvent?.Invoke();
    }

    public static event Action UpdateBodyPartEvent;
    public static void CallUpdateBodyPartEvent()
    {
        UpdateBodyPartEvent?.Invoke();
    }

    public static event Action UpdateColorEvent;
    public static void CallUpdateColorEvent()
    {
        UpdateColorEvent?.Invoke(); 
    }

    public static event Action IncreaseDisableEvent;
    public static void CallIncreaseDisableEvent()
    {
        IncreaseDisableEvent?.Invoke();
    }

    public static event Action DecreaseDisableEvent;
    public static void CallDecreaseDisableEvent()
    {
        DecreaseDisableEvent?.Invoke();
    }

    public static event Action<ImageType> ShowImageEvent;
    public static void CallShowImageEvent(ImageType type)
    {
        ShowImageEvent?.Invoke(type);
    }

    public static event Action HideImageEvent;
    public static void CallHideImageEvent()
    {
        HideImageEvent?.Invoke(); 
    }

    public static event Action<Direction> NPCFaceEvent;
    public static void CallNPCFaceEvent(Direction direction)
    {
        NPCFaceEvent?.Invoke(direction);
    }

    public static event Action<Direction> PlayerFaceEvent;
    public static void CallPlayerFaceEvent(Direction direction)
    {
        PlayerFaceEvent?.Invoke(direction);
    }

    public static event Action FadeInEvent;
    public static void CallFadeInEvent()
    {
        FadeInEvent?.Invoke();
    }

    public static event Action FadeOutEvent;
    public static void CallFadeOutEvent()
    {
        FadeOutEvent?.Invoke();
    }

    public static event Action DisableTransitionEvent;
    public static void CallDisableTransitionEvent()
    {
        DisableTransitionEvent?.Invoke();
    }

    public static event Action EnableTransitionEvent;
    public static void CallEnableTransitionEvent()
    {
        EnableTransitionEvent?.Invoke();
    }

    public static event Action BeforeSceneUnloadEvent;
    public static void CallBeforeSceneUnloadEvent()
    {
        //Debug.Log("Unload scene");
        BeforeSceneUnloadEvent?.Invoke();
    }

    public static event Action AfterFadeOutEvent;
    public static void CallAfterFadeOutEvent()
    {
        //Debug.Log("Unload scene");
        AfterFadeOutEvent?.Invoke();
    }

    public static event Action BeforeFadeInEvent;
    public static void CallBeforeFadeInEvent()
    {
        //Debug.Log("Unload scene");
        BeforeFadeInEvent?.Invoke();
    }

    public static event Action AfterSceneLoadEvent;
    public static void CallAfterSceneLoadEvent()
    {
        //Debug.Log("Load scene");
        AfterSceneLoadEvent?.Invoke();
    }

    public static event Action<Vector3> MoveToPosition;
    public static void CallMoveToPosition(Vector3 targetPosition)
    {
        MoveToPosition?.Invoke(targetPosition);
    }

    public static event Action<InteractionType, bool> CursorChangeEvent;
    public static void CallCursorChange(InteractionType type, bool isValid)
    {
        CursorChangeEvent?.Invoke(type, isValid);
    }

    public static event Action<string, string> ShowMessageEvent;
    public static void CallShowMessageEvent(string speakerName, string message)
    {
        ShowMessageEvent?.Invoke(speakerName, message);
    }

    public static event Action<Conversation> StartConversationEvent;
    public static void CallStartConversationEvent(Conversation conversation)
    {
        StartConversationEvent?.Invoke(conversation);
    }

    public static event Action EndConversationEvent;
    public static void CallEndConversationEvent()
    {
        EndConversationEvent?.Invoke();
    }

    public static event Action<Transform> SetMainFocusEvent;
    public static void CallSetMainFocusEvent(Transform focus)
    {
        SetMainFocusEvent?.Invoke(focus);
    }

    public static event Action HideMessageEvent;
    public static void CallHideMessageEvent()
    {
        HideMessageEvent?.Invoke();
    }

    public static event Action EnableNewConversationEvent;
    public static void CallEnableDialogueInteractionEvent()
    {
        EnableNewConversationEvent?.Invoke();
    }

    public static event Action DisableNewConversationEvent;
    public static void CallDisableDialogueInteractionEvent()
    {
        DisableNewConversationEvent?.Invoke();
    }

    public static event Action EnableCursorEvent;
    public static void CallEnableCursorEvent()
    {
        EnableCursorEvent?.Invoke();
    }

    public static event Action DisableCursorEvent;
    public static void CallDisableCursorEvent()
    {
        DisableCursorEvent?.Invoke();
    }

    public static event Action EnablePlayerMovementEvent;
    public static void CallEnablePlayerMovementEvent()
    {
        EnablePlayerMovementEvent?.Invoke();
    }

    public static event Action DisablePlayerMovementEvent;
    public static void CallDisablePlayerMovementEvent()
    {
        DisablePlayerMovementEvent?.Invoke();
    }

    public static event Action<PlayableDirector> RegisterPlayableDirectorEvent;

    public static void CallRegisterPlayableDirectorEvent(PlayableDirector playableDirector)
    {
        RegisterPlayableDirectorEvent?.Invoke(playableDirector);
    }

    public static event Action UnregisterPlayableDirectorEvent;

    public static void CallUnregisterPlayableDirectorEvent()
    {
        UnregisterPlayableDirectorEvent?.Invoke();
    }

    public static event Action SetDefaultSpeedEvent;
    public static void CallSetDefaultSpeedEvent()
    {
        SetDefaultSpeedEvent?.Invoke();
    }

    public static event Action SetSlowSpeedEvent;
    public static void CallSetSlowSpeedEvent()
    {
        SetSlowSpeedEvent?.Invoke();
    }

    public static event Action<Dictionary<string, Animator>> RegisterAnimatorEvent;
    public static void CallRegisterAnimatorEvent(Dictionary<string, Animator> animators)
    {
        RegisterAnimatorEvent?.Invoke(animators);
    }

    public static event Action<Dictionary<string, GameObject>> RegisterNPCEvent;
    public static void CallRegisterNPCEvent(Dictionary<string, GameObject> npcs)
    {
        RegisterNPCEvent?.Invoke(npcs);
    }

    public static event Action FollowerChangeEvent;
    public static void CallFollowerChangeEvent()
    {
        FollowerChangeEvent?.Invoke();
    }

    public static event Action OnStaticsUpdated;

    public static void RegisterToStaticsUpdate(Action callback)
    {
        OnStaticsUpdated += callback;
    }

    public static void UnregisterFromStaticsUpdate(Action callback)
    {
        OnStaticsUpdated -= callback;
    }

    public static void CallStaticsUpdate()
    {
        OnStaticsUpdated?.Invoke();
    }

    public static event Action<bool> SetTimelinePlayingEvent;
    public static void CallOnSetTimelinePlaying(bool playing)
    {
        SetTimelinePlayingEvent?.Invoke(playing);
    }

    //public static event Action<Vector3, ItemDetails> MouseClickedEvent;
    //public static void CallMouseClickedEvent(Vector3 pos, ItemDetails itemDetails)
    //{
    //    MouseClickedEvent?.Invoke(pos, itemDetails);
    //}


    //public static event Action<int> StartNewGameEvent;
    //public static void CallStartNewGameEvent(int index)
    //{
    //    StartNewGameEvent?.Invoke(index);
    //}

    //public static event Action EndGameEvent;
    //public static void CallEndGameEvent()
    //{
    //    EndGameEvent?.Invoke();
    //}
}
