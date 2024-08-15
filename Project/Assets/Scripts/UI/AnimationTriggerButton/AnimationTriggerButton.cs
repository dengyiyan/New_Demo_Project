using UnityEngine;
using UnityEngine.UI;

public class AnimationTriggerButton : MonoBehaviour
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

        //gameObject.GetComponent<Button>().onClick.AddListener();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && animationManager != null)
        {
            animationManager.PlaySequence(sequence);
        }
    }
}
