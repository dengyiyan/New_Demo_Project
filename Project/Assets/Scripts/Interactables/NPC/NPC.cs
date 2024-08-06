using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public Conversation conversation;
    [SerializeField] private bool advanced;

    private Animator animator;
    private GameObject player;

    //private SpriteRenderer bodyRenderer;
    //private SpriteRenderer armsRenderer;
    //private SpriteRenderer hairRenderer;
    //private SpriteRenderer pantsRenderer;

    private void Start()
    {
        animator = GetComponentInParent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");

        //if (gameObject.transform.Find("Body"))
        //{
        //    bodyRenderer = gameObject.transform.Find("Body").GetComponent<SpriteRenderer>();
        //    armsRenderer = gameObject.transform.Find("Arms").GetComponent<SpriteRenderer>();
        //    hairRenderer = gameObject.transform.Find("Hair").GetComponent<SpriteRenderer>();
        //    pantsRenderer = gameObject.transform.Find("Pants").GetComponent<SpriteRenderer>();

        //}
    }

    //private void FixedUpdate()
    //{
    //    Debug.Log($"body flip is: {bodyRenderer.flipX}");
    //}

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
            // float x = 0; float y = 0;
            setAnimatorOnDirection(direction);
            //animator.SetFloat("moveX", direction.x);
            //animator.SetFloat("moveY", direction.y);

            //bool flag = false;

            //animator.SetBool("isWalking", false);

            //flipOnX(flag);
        }
    }

    private void setAnimatorOnDirection(Vector3 direction)
    {
        if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
        {
            if (direction.y < 0)
            {
                animator.SetFloat("moveX", 0);
                animator.SetFloat("moveY", -1);
            }
            else
            {
                animator.SetFloat("moveX", 0);
                animator.SetFloat("moveY", 1);
            }
        }
        else
        {
            if (direction.x < 0)
            {
                animator.SetFloat("moveX", -1);
                animator.SetFloat("moveY", 0);
            }
            else
            {
                animator.SetFloat("moveX", 1);
                animator.SetFloat("moveY", 0);
            }
        }
    }
    //private void flipOnX(bool flag)
    //{
    //    if (bodyRenderer)
    //        bodyRenderer.flipX = flag;
    //    if (armsRenderer)
    //        armsRenderer.flipX = flag;
    //    if (hairRenderer)
    //        hairRenderer.flipX = flag;
    //    if (pantsRenderer)
    //        pantsRenderer.flipX = flag;
    //    Debug.LogWarning($"Set flipX to {flag} in movement.");
    //}
}
