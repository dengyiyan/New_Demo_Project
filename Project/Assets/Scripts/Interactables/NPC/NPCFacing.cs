using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NPCFacing : MonoBehaviour
{
    public Direction defaultFacingDirection; 
    private Animator animator;


    private SpriteRenderer bodyRenderer;
    private SpriteRenderer armsRenderer;
    private SpriteRenderer hairRenderer;
    private SpriteRenderer pantsRenderer;

    void Start()
    {
        animator = GetComponent<Animator>();


        bodyRenderer = gameObject.transform.Find("Body").GetComponent<SpriteRenderer>();

        armsRenderer = gameObject.transform.Find("Arms").GetComponent<SpriteRenderer>();
        hairRenderer = gameObject.transform.Find("Hair").GetComponent<SpriteRenderer>();
        pantsRenderer = gameObject.transform.Find("Pants").GetComponent<SpriteRenderer>();

        SetFacingDirection(defaultFacingDirection);
    }

    public void SetFacingDirection(Direction direction)
    {
        if (animator != null)
        {
            switch (direction)
            {
                case Direction.Front:
                    animator.SetFloat("moveX", 0);
                    animator.SetFloat("moveY", -1);
                    break;
                case Direction.Back:
                    animator.SetFloat("moveX", 0);
                    animator.SetFloat("moveY", 1);
                    break;
                case Direction.Left:
                    animator.SetFloat("moveX", -1);
                    animator.SetFloat("moveY", 0);
                    break;
                case Direction.Right:
                    animator.SetFloat("moveX", 1);
                    animator.SetFloat("moveY", 0);
                    break;
                default:
                    break;
            }

            switch (direction)
            {
                case Direction.Left:

                    if (bodyRenderer)
                        bodyRenderer.flipX = true;
                    if (armsRenderer)
                        armsRenderer.flipX = true;
                    if (hairRenderer)
                        hairRenderer.flipX = true;
                    if (pantsRenderer)
                        pantsRenderer.flipX = true;
                    break;
                default:

                    if (bodyRenderer)
                        bodyRenderer.flipX = false;
                    if (armsRenderer)
                        armsRenderer.flipX = false;
                    if (hairRenderer)
                        hairRenderer.flipX = false;
                    if (pantsRenderer)
                        pantsRenderer.flipX = false;
                    break;
            }
        }
    }

    //public void SetPlayAnimation()
    //{

    //}
}



