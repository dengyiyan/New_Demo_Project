using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvestObject : MonoBehaviour
{
    [SerializeField] private string relatedBool;
    [SerializeField] private string InversObjectID;
    public Conversation conversation;
    [SerializeField] private bool canPick = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void PickUp()
    {
        if (canPick)
        {
            GameStateManager.SetBool(relatedBool, true);
            GameStateManager.PickUp(InversObjectID);
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("Object cannot be picked up!");
        }
    }

    public void StartConversation()
    {
        EventHandler.CallStartConversationEvent(conversation);
    }
}
