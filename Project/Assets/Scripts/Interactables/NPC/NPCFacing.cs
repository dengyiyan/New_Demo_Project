using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NPCFacing : MonoBehaviour
{
    public Direction defaultFacingDirection; 
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
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
            }
        }
    }

    //public void SetPlayAnimation()
    //{

    //}
}


[System.Serializable]
public enum Direction
{
    Front,
    Back,
    Left,
    Right,
    // Add other animation types as needed
}
