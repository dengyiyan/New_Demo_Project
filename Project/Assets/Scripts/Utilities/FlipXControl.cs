using UnityEngine;

public class FlipXControl : StateMachineBehaviour
{
    public bool flipX;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Get all SpriteRenderer components in the GameObject and its children
        SpriteRenderer[] spriteRenderers = animator.GetComponents<SpriteRenderer>();

        // Loop through each SpriteRenderer and set the flipX property
        foreach (var spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = flipX;
            }
        }
    }
}