using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class EventHandler
{
    public static event Action<string, string> TransitionEvent;
    public static void CallTransitionEvent(string sceneName, string spawnPointID)
    {
        foreach (var convChecker in GameObject.FindObjectsOfType<ConversationChecker>())
        {
            convChecker.CheckTransitionStatus(sceneName, spawnPointID);
        }
        TransitionEvent?.Invoke(sceneName, spawnPointID);
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
        BeforeSceneUnloadEvent?.Invoke();
    }

    public static event Action AfterSceneLoadEvent;
    public static void CallAfterSceneLoadEvent()
    {
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
    //public static event Action<Vector3, ItemDetails> MouseClickedEvent;
    //public static void CallMouseClickedEvent(Vector3 pos, ItemDetails itemDetails)
    //{
    //    MouseClickedEvent?.Invoke(pos, itemDetails);
    //}


    public static event Action<int> StartNewGameEvent;
    public static void CallStartNewGameEvent(int index)
    {
        StartNewGameEvent?.Invoke(index);
    }

    public static event Action EndGameEvent;
    public static void CallEndGameEvent()
    {
        EndGameEvent?.Invoke();
    }
}
