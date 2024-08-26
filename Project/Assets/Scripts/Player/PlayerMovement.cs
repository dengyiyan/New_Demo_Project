using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float defaultSpeed = 4f; 
    [SerializeField] private float slowSpeed = 2f; // Add this line
    private float currentSpeed;

    private Vector3 movement;
    private Rigidbody2D myRigidbody;
    private Animator animator;
    private bool canMove = true;


    //private SpriteRenderer bodyRenderer;
    //private SpriteRenderer armsRenderer;
    //private SpriteRenderer hairRenderer;
    //private SpriteRenderer pantsRenderer;

    private bool shouldFlip = false;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();

        //GameObject player = GameObject.Find("Player");
        //bodyRenderer = player.transform.Find("Body").GetComponent<SpriteRenderer>();

        //armsRenderer = player.transform.Find("Arms").GetComponent<SpriteRenderer>();
        //hairRenderer = player.transform.Find("Hair").GetComponent<SpriteRenderer>();
        //pantsRenderer = player.transform.Find("Pants").GetComponent<SpriteRenderer>();

        SetFacingDirection(Direction.Front);
        SetDefaultSpeed();
    }

    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        EventHandler.MoveToPosition += OnMoveToPosition;
        EventHandler.PlayerFaceEvent += SetFacingDirection;

        EventHandler.EnablePlayerMovementEvent += EnableMovement;
        EventHandler.DisablePlayerMovementEvent += DisableMovement;
        EventHandler.SetDefaultSpeedEvent += SetDefaultSpeed;
        EventHandler.SetSlowSpeedEvent += SetSlowSpeed;
    }
    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        EventHandler.MoveToPosition -= OnMoveToPosition;
        EventHandler.PlayerFaceEvent -= SetFacingDirection;

        EventHandler.EnablePlayerMovementEvent -= EnableMovement;
        EventHandler.DisablePlayerMovementEvent -= DisableMovement;
        EventHandler.SetDefaultSpeedEvent -= SetDefaultSpeed;
        EventHandler.SetSlowSpeedEvent -= SetSlowSpeed;
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            movement = Vector3.zero;
            UpdateMovement();
            UpdateAnimationAndMove();
        }
    }

    public void SetSlowSpeed()
    {
        currentSpeed = slowSpeed;
    }

    public void SetDefaultSpeed()
    {
        currentSpeed = defaultSpeed;
    }

    private void UpdateMovement() 
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    private void UpdateAnimationAndMove()
    {
        if (movement.x != 0 || movement.y != 0)
        {
            setAnimatorOnDirection(movement);

            shouldFlip = movement.x < 0;

            //flipOnX(shouldFlip);

            //animator.SetTrigger("Move");
            animator.SetBool("isWalking", true);
            // myRigidbody.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
            MoveCharacter();
        }
        else
        {
            //animator.SetTrigger("Idle");
            animator.SetBool("isWalking", false);
            // myRigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
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
        myRigidbody.MovePosition(transform.position + movement * currentSpeed * Time.deltaTime);
    }

    private void EnableMovement()
    {
        canMove = true;
    }

    private void DisableMovement()
    {
        canMove = false; 
        //animator.SetBool("isWalking", false);
    }

    private bool isPlayer()
    {
        return gameObject.tag == "Player";
    }

    private void OnMoveToPosition(Vector3 targetPosition)
    {
        transform.position = targetPosition;
    }

    private void OnAfterSceneLoadEvent()
    {
        canMove = true;
        EventHandler.CallDecreaseDisableEvent();
    }

    private void OnBeforeSceneUnloadEvent()
    {
        canMove = false;
        EventHandler.CallIncreaseDisableEvent();
        animator.SetBool("isWalking", false);
    }

    private void SetFacingDirection(Direction direction)
    {
        if (animator != null)
        {
            Debug.Log($"setting player facing {direction}");
            switch (direction)
            {
                case Direction.Front:
                    animator.SetFloat("moveX", 0);
                    animator.SetFloat("moveY", -1);
                    //flipOnX(false);
                    break;
                case Direction.Back:
                    animator.SetFloat("moveX", 0);
                    animator.SetFloat("moveY", 1);
                    //flipOnX(false);
                    break;
                case Direction.Left:
                    animator.SetFloat("moveX", -1);
                    animator.SetFloat("moveY", 0);
                    //flipOnX(true);
                    break;
                case Direction.Right:
                    animator.SetFloat("moveX", 1);
                    animator.SetFloat("moveY", 0);
                    //flipOnX(false);
                    break;
                default:
                    //flipOnX(false);
                    break;
            }
        }
    }
}
