using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationMovement : MonoBehaviour
{
    private Queue<Vector3> points;
    [SerializeField] private float threshold = 0.1f;
    [SerializeField] private float defaultSpeed = 4f;
    private Vector3 movement;
    private Rigidbody2D myRigidbody;
    private Animator animator;
    // private bool canMove = true;
    public bool HasCompletedPath { get; private set; }


    //private SpriteRenderer bodyRenderer;
    //private SpriteRenderer armsRenderer;
    //private SpriteRenderer hairRenderer;
    //private SpriteRenderer pantsRenderer;

    //private bool shouldFlip = false;
    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogWarning("animator missing in movement start method!!!");
        }
        myRigidbody = GetComponent<Rigidbody2D>();

        //if (isPlayer())
        //{
        //    bodyRenderer = gameObject.transform.Find("Body").GetComponent<SpriteRenderer>();

        //    armsRenderer = gameObject.transform.Find("Arms").GetComponent<SpriteRenderer>();
        //    hairRenderer = gameObject.transform.Find("Hair").GetComponent<SpriteRenderer>();
        //    pantsRenderer = gameObject.transform.Find("Pants").GetComponent<SpriteRenderer>();
        //}

        // animator = GetComponent<Animator>();
        // myRigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    public void SetPoints(List<Vector3> newPoints)
    {
        points = new Queue<Vector3>(newPoints);
        HasCompletedPath = false;
    }

    public IEnumerator MoveAlongPoints()
    {
        // myRigidbody.bodyType = RigidbodyType2D.Dynamic;

        while (points.Count > 0)
        {
            Vector3 currentTarget = points.Dequeue();
            while (Vector3.Distance(transform.position, currentTarget) > threshold)
            {
                Vector3 direction = (currentTarget - transform.position).normalized;
                //movement = direction * defaultSpeed * Time.deltaTime;
                animator.SetFloat("moveX", direction.x);
                animator.SetFloat("moveY", direction.y);

                //shouldFlip = direction.x < 0;
                transform.position = Vector3.MoveTowards(transform.position, currentTarget, defaultSpeed * Time.deltaTime);
                //flipOnX(shouldFlip);
                // Debug.LogWarning($"Setting flip to {shouldFlip} in movement");

                //MoveCharacter();
                yield return null; // Wait for the next frame
            }
            transform.position = currentTarget;
        }

        if (gameObject.tag == "Player")
        {
            myRigidbody.bodyType = RigidbodyType2D.Dynamic;
        }
        else
        {
            myRigidbody.bodyType = RigidbodyType2D.Kinematic;
        }
        HasCompletedPath = true;
        myRigidbody.velocity = Vector2.zero;
    }

    public IEnumerator WaitForAnimation(string trigger, AnimationType type)
    {
        int layerIndex = 0;

        switch (type)
        {
            case AnimationType.Emote:
                layerIndex = GetLayerIndex(animator, "Emote"); break;
            case AnimationType.Wake:
                layerIndex = GetLayerIndex(animator, "Body"); break;
        }
        if (layerIndex == -1)
        {
            Debug.LogError($"Layer not found in animator.");
            yield break;
        }

        animator.SetTrigger(trigger);

        bool animationStarted = false;

        // Wait for the animation to start
        while (!animationStarted)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
            if (stateInfo.IsName(trigger))
            {
                animationStarted = true;
            }
            yield return null;
        }

        // Wait for the animation to end
        while (animationStarted)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
            if (!stateInfo.IsName(trigger))
            {
                animationStarted = false;
            }
            yield return null;
        }
    }

    public int GetLayerIndex(Animator animator, string layerName)
    {

        if (animator == null)
        {
            Debug.LogWarning("animator missing in movement");
        }
        for (int i = 0; i < animator.layerCount; i++)
        {
            if (animator.GetLayerName(i) == layerName)
            {
                return i;
            }
        }
        return -1; // Layer not found
    }

    public void SetWalking(bool isWalking)
    {
        SetBoolParameter("isWalking", isWalking);
    }

    public void SetAnimationTrigger(string trigger)
    {
        if (animator != null)
        {
            animator.SetTrigger(trigger);
        }
    }

    public void SetBoolParameter(string parameterName, bool value)
    {
        if (animator != null)
        {
            animator.SetBool(parameterName, value);
        }
    }

    //private void flipOnX(bool flag)
    //{
    //    if (isPlayer())
    //    {
    //        bodyRenderer.flipX = flag;
    //        armsRenderer.flipX = flag;
    //        hairRenderer.flipX = flag;
    //        pantsRenderer.flipX = flag;
    //    }
    //}

    private void MoveCharacter()
    {
        if (movement.x != 0 && movement.y != 0)
        {
            movement.x *= 0.6f;
            movement.y *= 0.6f;
        }
        myRigidbody.MovePosition(transform.position + movement * defaultSpeed * Time.deltaTime);
    }

    private bool isPlayer()
    {
        return gameObject.tag == "Player";
    }
}
