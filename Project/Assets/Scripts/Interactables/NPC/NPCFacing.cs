using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NPCFacing : MonoBehaviour
{
    public Direction defaultFacingDirection; 
    private Animator animator;


    //private SpriteRenderer bodyRenderer;
    //private SpriteRenderer armsRenderer;
    //private SpriteRenderer hairRenderer;
    //private SpriteRenderer pantsRenderer;

    void Start()
    {
        animator = GetComponent<Animator>();
        //Debug.Log($"animator is {animator}");

        //if (gameObject.transform.Find("Body"))
        //{
        //    Debug.Log("Found body");
        //    bodyRenderer = gameObject.transform.Find("Body").GetComponent<SpriteRenderer>();

        //    armsRenderer = gameObject.transform.Find("Arms").GetComponent<SpriteRenderer>();
        //    hairRenderer = gameObject.transform.Find("Hair").GetComponent<SpriteRenderer>();
        //    pantsRenderer = gameObject.transform.Find("Pants").GetComponent<SpriteRenderer>();
        //}

        SetFacingDirection(defaultFacingDirection);
    }

    private void OnEnable()
    {
        SetFacingDirection(defaultFacingDirection);
    }
    //private void FixedUpdate()
    //{
    //    SetFacingDirection(defaultFacingDirection);
    //}

    public void SetFacingDirection(Direction direction)
    {
        if (animator != null)
        {
            switch (direction)
            {
                case Direction.Left:
                    animator.SetFloat("moveX", -1);
                    animator.SetFloat("moveY", 0);
                    break;
                case Direction.Front:
                    animator.SetFloat("moveX", 0);
                    animator.SetFloat("moveY", -1);
                    break;
                case Direction.Back:
                    animator.SetFloat("moveX", 0);
                    animator.SetFloat("moveY", 1);
                    break;
                case Direction.Right:
                    animator.SetFloat("moveX", 1);
                    animator.SetFloat("moveY", 0);
                    break;
                default:
                    break;
            }


            //switch (direction)
            //{
            //    case Direction.Left:
            //        Debug.Log("Facing left, flipping");
            //        if (bodyRenderer)
            //        {
            //            Debug.Log("Flipping body");
            //            bodyRenderer.flipX = true;
            //            Debug.LogWarning($"body flip is: {bodyRenderer.flipX}");

            //        }
            //        if (armsRenderer)
            //            armsRenderer.flipX = true;
            //        if (hairRenderer)
            //            hairRenderer.flipX = true;
            //        if (pantsRenderer)
            //            pantsRenderer.flipX = true;
            //        break;
            //    default:
            //        Debug.LogWarning($"Setting flip to false in npc facing");

            //        if (bodyRenderer)
            //            bodyRenderer.flipX = false;
            //        if (armsRenderer)
            //            armsRenderer.flipX = false;
            //        if (hairRenderer)
            //            hairRenderer.flipX = false;
            //        if (pantsRenderer)
            //            pantsRenderer.flipX = false;
            //        break;
            //}
        }
    }

    //private void SetFlipX(bool flag)
    //{
    //    if (bodyRenderer)
    //        bodyRenderer.flipX = flag;
    //    if (armsRenderer)
    //        armsRenderer.flipX = flag;
    //    if (hairRenderer)
    //        hairRenderer.flipX = flag;
    //    if (pantsRenderer)
    //        pantsRenderer.flipX = flag;
    //    Debug.LogWarning($"Set flipX to {flag} for all renderers.");
    //    Debug.LogWarning($"flipX for body is {bodyRenderer.flipX}");
    //}

    //public void SetPlayAnimation()
    //{

    //}
}



