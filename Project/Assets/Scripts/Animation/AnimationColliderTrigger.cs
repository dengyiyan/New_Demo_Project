using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAnimationOnCollision : MonoBehaviour
{
    public AnimationSequence sequence; // The name of the animation sequence to trigger

    private AnimationManager animationManager;

    private void Start()
    {
        animationManager = FindObjectOfType<AnimationManager>();
        if (animationManager == null)
        {
            Debug.LogError("AnimationManager not found in the scene.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && animationManager != null)
        {
            animationManager.PlaySequence(sequence);
        }
    }
}
