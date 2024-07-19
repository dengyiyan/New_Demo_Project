using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] private InteractionType interactionType;
    [SerializeField] private float interactionRange;
    [SerializeField] private float invalidTransparency = Settings.invalidCursorTransparency;

    private GameObject player;
    private Transform playerTransform;
    private Collider2D interactableCollider;
    private SpriteRenderer[] spriteRenderers;
    private Door door;
    private NPC npc;
    private InvestObject invest;

    private static InteractableObject currentHoveredObject = null;

    private void Awake()
    {
        door = GetComponent<Door>();
        npc = GetComponent<NPC>();
        invest = GetComponent<InvestObject>();

        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player object with tag 'Player' not found in the scene.");
        }
        interactableCollider = GetComponent<Collider2D>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }
    // Start is called before the first frame update
    //void Start()
    //{
        
    //}

    // Update is called once per frame
    void Update()
    {
        bool isCursorOver = false;
        float distance = 0;
        if (GetComponentsInChildren<SpriteRenderer>().Length > 0)
        {
            isCursorOver = RaycastToSprite();
            distance = GetClosestDistanceSprite();
        }
        else if (GetComponentsInChildren<BoxCollider2D>().Length > 0)
        {
            isCursorOver = RaycastToPlayer();
            distance = GetClosestDistanceCollider();
        }

        bool isValid = distance < interactionRange;
        float transparency = isValid ? Settings.validCursorTransparency : invalidTransparency;
        if (isCursorOver)
        {
            currentHoveredObject = this;
            EventHandler.CallCursorChange(interactionType, transparency);
        }
        else if (currentHoveredObject == this)
        {
            currentHoveredObject = null;
            EventHandler.CallCursorChange(InteractionType.None, Settings.validCursorTransparency);
        }

        if (distance <= interactionRange && isCursorOver && Input.GetMouseButtonDown(0))
        {
            Interact();
            //EventHandler.CallCursorChange(InteractionType.None, Settings.validCursorTransparency);
        }
    }

    private bool RaycastToPlayer()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                return true;
            }
        }

        return false;
    }

    private bool RaycastToSprite()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        foreach (SpriteRenderer spriteRenderer in GetComponentsInChildren<SpriteRenderer>())
        {
            Bounds bounds = spriteRenderer.bounds;
            Vector3 min = bounds.min;
            Vector3 max = bounds.max;

            if (mousePosition.x >= min.x && mousePosition.x <= max.x && mousePosition.y >= min.y && mousePosition.y <= max.y)
            {
                return true;
            }
        }
        return false;
    }

    private float GetClosestDistanceSprite()
    {
        if (spriteRenderers.Length == 0)
        {
            Debug.LogError("No SpriteRenderer components found in the children.");
            return float.MaxValue;
        }

        float minDistance = float.MaxValue;
        Vector3 playerPosition = playerTransform.position;

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            Bounds bounds = spriteRenderer.bounds;
            Vector3 closestPoint = bounds.ClosestPoint(playerPosition);
            float distance = Vector2.Distance(closestPoint, playerPosition);

            if (distance < minDistance)
            {
                minDistance = distance;
            }
        }

        return minDistance;
    }

    private float GetClosestDistanceCollider()
    {
        Vector3 closestPoint = interactableCollider.ClosestPoint(playerTransform.position);
        float distance = Vector2.Distance(closestPoint, playerTransform.position);
        return distance;
    }

    private void Interact()
    {
        switch (interactionType) 
        {
            case InteractionType.Door:
                DoorInteraction();
                break;
            case InteractionType.NPC:
                NPCInteraction();
                break;
            case InteractionType.Invest:
                InvestInteraction();
                break;
            default:
                break;
        }
    }

    private void DoorInteraction()
    {
        if (door)
        {
            door.Interact();
        }
        //EventHandler.CallCursorChange(InteractionType.None, Settings.validCursorTransparency);

    }
    private void NPCInteraction()
    {
        if (npc)
        {
            npc.StartConversation();
        }
        else
        {
            Debug.Log("No npc script found");
        }
    }
    private void InvestInteraction()
    {
        if (invest)
        {
            invest.StartConversation();
        }
        else
        {
            Debug.Log("No invest script found");
        }
    }
}
