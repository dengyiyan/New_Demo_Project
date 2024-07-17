using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationChecker : MonoBehaviour
{
    public List<NPC> npcs;

    public string needConversationName;
    public string needConversationText;

    //private void Awake()
    //{
    //    EventHandler.TransitionEvent += OnTransitionEvent;
    //}

    //private void OnDisable()
    //{
    //    EventHandler.TransitionEvent -= OnTransitionEvent;
    //}

    public bool canLeaveScene()
    {
        foreach (var npc in npcs)
        {
            if (!GameStateManager.IsConversationCompleted(npc.conversation.ID))
            {
                return false;
            }
        }

        return true;
    }

    public void CheckTransitionStatus(string SceneTo, string SpawnID)
    {
        if (!canLeaveScene())
        {
            EventHandler.CallDisableTransitionEvent();
            EventHandler.CallShowMessageEvent(needConversationName, needConversationText);
        }
        else
        {
            EventHandler.CallEnableTransitionEvent();
        }
    }
}
