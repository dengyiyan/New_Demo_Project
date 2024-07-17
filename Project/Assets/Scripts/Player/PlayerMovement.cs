using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float defaultSpeed = 4f;
    private Vector3 movement;
    private Rigidbody2D myRigidbody;
    private Animator animator;
    private bool canMove = true;


    private SpriteRenderer bodyRenderer;
    private SpriteRenderer armsRenderer;
    private SpriteRenderer hairRenderer;
    private SpriteRenderer pantsRenderer;

    private bool shouldFlip = false;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();

        GameObject player = GameObject.Find("Player");
        bodyRenderer = player.transform.Find("Body").GetComponent<SpriteRenderer>();

        armsRenderer = player.transform.Find("Arms").GetComponent<SpriteRenderer>();
        hairRenderer = player.transform.Find("Hair").GetComponent<SpriteRenderer>();
        pantsRenderer = player.transform.Find("Pants").GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        EventHandler.MoveToPosition += OnMoveToPosition;

        EventHandler.EnablePlayerMovementEvent += EnableMovement;
        EventHandler.DisablePlayerMovementEvent += DisableMovement;
    }
    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        EventHandler.MoveToPosition -= OnMoveToPosition;

        EventHandler.EnablePlayerMovementEvent -= EnableMovement;
        EventHandler.DisablePlayerMovementEvent -= DisableMovement;
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

    private void UpdateMovement() 
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    private void UpdateAnimationAndMove()
    {
        if (movement.x != 0 || movement.y != 0)
        {
            animator.SetFloat("moveX", movement.x);
            animator.SetFloat("moveY", movement.y);

            shouldFlip = movement.x < 0;

            flipOnX(shouldFlip);

            //animator.SetTrigger("Move");
            animator.SetBool("isWalking", true);
            myRigidbody.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
            MoveCharacter();
        }
        else
        {
            //animator.SetTrigger("Idle");
            animator.SetBool("isWalking", false);
            myRigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        }
    }

    private void flipOnX(bool flag)
    {
        if (isPlayer())
        {
            bodyRenderer.flipX = flag;
            armsRenderer.flipX = flag;
            hairRenderer.flipX = flag;
            pantsRenderer.flipX = flag;
        }
    }

    private void MoveCharacter()
    {
        if (movement.x != 0 && movement.y != 0)
        {
            movement.x *= 0.6f;
            movement.y *= 0.6f;
        }
        myRigidbody.MovePosition(transform.position + movement * defaultSpeed * Time.deltaTime);
    }

    private void EnableMovement()
    {
        canMove = true;
    }

    private void DisableMovement()
    {
        canMove = false;
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
    }

    private void OnBeforeSceneUnloadEvent()
    {
        canMove = false;
        animator.SetBool("isWalking", false);
    }
}
