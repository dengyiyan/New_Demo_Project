using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public Conversation conversation;
    [SerializeField] private bool advanced;

    private Animator animator;
    private GameObject player;

    private void Start()
    {
        animator = GetComponentInParent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void StartConversation()
    {
        FacePlayer();
        EventHandler.CallStartConversationEvent(conversation);
    }

    private void FacePlayer()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;

        if (advanced)
        {
            animator.SetFloat("moveX", direction.x);
            animator.SetFloat("moveY", direction.y);
            animator.SetBool("isWalking", false);
        }
    }
}
