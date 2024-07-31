using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public Conversation conversation;
    [SerializeField] private bool advanced;

    private Animator animator;
    private GameObject player;

    private SpriteRenderer bodyRenderer;
    private SpriteRenderer armsRenderer;
    private SpriteRenderer hairRenderer;
    private SpriteRenderer pantsRenderer;

    private void Start()
    {
        animator = GetComponentInParent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");


        bodyRenderer = gameObject.transform.Find("Body").GetComponent<SpriteRenderer>();
        armsRenderer = gameObject.transform.Find("Arms").GetComponent<SpriteRenderer>();
        hairRenderer = gameObject.transform.Find("Hair").GetComponent<SpriteRenderer>();
        pantsRenderer = gameObject.transform.Find("Pants").GetComponent<SpriteRenderer>();
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

            bool flag = direction.x < 0;

            if (bodyRenderer)
                bodyRenderer.flipX = flag;
            if (armsRenderer)
                armsRenderer.flipX = flag;
            if (hairRenderer)
                hairRenderer.flipX = flag;
            if (pantsRenderer)
                pantsRenderer.flipX = flag;

            animator.SetBool("isWalking", false);
        }
    }
}
