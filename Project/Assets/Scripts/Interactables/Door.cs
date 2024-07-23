using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    [SceneName]
    [SerializeField] private string sceneTo;
    [SerializeField] private string spawnID;
    public Conversation conversation;

    private DialogueManager dialogueManager;

    private void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    public void Interact()
    {
        StartCoroutine(HandleInteraction());
    }

    private IEnumerator HandleInteraction()
    {
        EventHandler.CallStartConversationEvent(conversation);
        yield return new WaitUntil(() => !dialogueManager.GetIsShowing());

        if (!string.IsNullOrEmpty(sceneTo))
        {
            EventHandler.CallTransitionEvent(sceneTo, spawnID);
        }
    }

}
