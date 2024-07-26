using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    private Animator animator;
    private Vector2[] directions;
    private int currentPosition = 0;


    private SpriteRenderer bodyRenderer;
    private SpriteRenderer armsRenderer;
    private SpriteRenderer hairRenderer;
    private SpriteRenderer pantsRenderer;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        GameObject player = this.gameObject;
        bodyRenderer = player.transform.Find("Body").GetComponent<SpriteRenderer>();

        armsRenderer = player.transform.Find("Arms").GetComponent<SpriteRenderer>();
        hairRenderer = player.transform.Find("Hair").GetComponent<SpriteRenderer>();
        pantsRenderer = player.transform.Find("Pants").GetComponent<SpriteRenderer>();

        CreateDirections();
    }

    private void CreateDirections()
    {
        directions = new Vector2[4];
        directions[0] = new Vector2(0, -1);        // Front
        directions[1] = new Vector2(1, 0);         // Right
        directions[2] = new Vector2(0, 1);         // Back
        directions[3] = new Vector2(-1, 0);        // Left
    }

    public void RotateRight()
    {
        if (currentPosition > 0)
        {
            currentPosition--;
        }
        else
        {
            currentPosition = directions.Length - 1;
        }

        UpdateDirection();
    }

    public void RotateLeft()
    {
        if (currentPosition < directions.Length - 1)
        {
            currentPosition++;
        }
        else
        {
            currentPosition = 0;
        }

        UpdateDirection();
    }

    private void UpdateDirection()
    {
        Vector2 direction = directions[currentPosition];

        animator.SetFloat("moveX", direction.x);
        animator.SetFloat("moveY", direction.y);

        bool shouldFlip = direction.x == -1;

        bodyRenderer.flipX = shouldFlip;
        armsRenderer.flipX = shouldFlip;
        hairRenderer.flipX = shouldFlip;
        pantsRenderer.flipX = shouldFlip;
    }
}
