using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private CinemachineTargetGroup targetGroup;
    [SerializeField] private float defaultRadius = 5f;
    [SerializeField] private float defaultOthersRadius = 2f;
    [SerializeField] private float defaultWeight = 1f;
    [SerializeField] private float othersWeight = 0.4f;
    [SerializeField] private float focusWeight = 2f;
    [SerializeField] private float playSpeed = 0.05f;
    public GameObject dialoguePanel;
    public Text nameText;
    public Text dialogueText;
    public Button choiceButtonPrefab;
    public GameObject choicePanel;

    private List<Button> buttons = new List<Button>();
    private Queue<ConversationDialogue> dialogues;
    private bool isShowing = false;
    private bool isShowingMessage = false;
    private bool isChoiceActivated = false;
    private bool isTyping = false;
    private string typingSentence;
    private bool isAnimationPlaying = false;
    private DialogueChoice[] typingChoices;

    //public Sprite cursorSprite;
    public PlayerMovement movement;
    private GameObject player;
    private Animator animator;

    private ConversationDialogue[] currentDialogues;
    private int currentDialogueIndex;

    private string currentConversationID;


    private Dictionary<string, Animator> animators = new Dictionary<string, Animator>();
    private Dictionary<string, Animator> new_animators = new Dictionary<string, Animator>();


    void Awake()
    {
        if (virtualCamera == null)
        {
            virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        }

        if (targetGroup == null)
        {
            targetGroup = FindObjectOfType<CinemachineTargetGroup>();
        }

        dialogues = new Queue<ConversationDialogue>();
        //animationManager = FindObjectOfType<AnimationManager>();
        dialoguePanel.SetActive(false);
        choicePanel.SetActive(false);

        player = GameObject.FindGameObjectWithTag("Player");
        animator = player.GetComponent<Animator>();


        foreach (var ani in FindObjectsOfType<Animator>())
        {
            animators[ani.gameObject.name] = ani;
        }

        animators["Player"] = animator;

        AddEventTrigger(dialoguePanel, EventTriggerType.PointerClick, OnDialoguePanelClick);
    }

    private void Start()
    {
        OnRegisterAnimatorEvent();
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        EventHandler.StartConversationEvent += OnStartConversation;
        EventHandler.EndConversationEvent += EndConversation;
        EventHandler.ShowMessageEvent += ShowMessage;
        EventHandler.HideMessageEvent += HideMessage;

        EventHandler.EnableNewConversationEvent += EnableNewConversation;
        EventHandler.DisableNewConversationEvent += DisableNewConversation;

        //EventHandler.RegisterAnimatorEvent += OnRegisterAnimatorEvent;
    }

    private void OnDestroy()
    {
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        EventHandler.StartConversationEvent -= OnStartConversation;
        EventHandler.EndConversationEvent -= EndConversation;
        EventHandler.ShowMessageEvent -= ShowMessage;
        EventHandler.HideMessageEvent -= HideMessage;

        EventHandler.EnableNewConversationEvent -= EnableNewConversation;
        EventHandler.DisableNewConversationEvent -= DisableNewConversation;
        //EventHandler.RegisterAnimatorEvent -= OnRegisterAnimatorEvent;
    }


    private void OnAfterSceneLoadEvent()
    {
        OnRegisterAnimatorEvent();
        
    }

    private void EnableNewConversation()
    {
        isShowing = true;
    }

    private void DisableNewConversation()
    {
        isShowing = false;
    }

    private void OnRegisterAnimatorEvent()
    {
        foreach (var ani in FindObjectsOfType<Animator>())
        {
            animators[ani.gameObject.name] = ani;
            Debug.Log(ani.gameObject.name);
        }

        animators["Player"] = animator;
    }

    private string getTrueSpeakerName(string speakerName)
    {
        string name = "";
        if (speakerName == "Player")
        {
            name = GameStateManager.PlayerName;
        }
        else if (speakerName == "Merchant")
        {
            if (GameStateManager.GetBool("MerchantShowHisName"))
            {
                name = Settings.MerchantName;
            }
            else
            {
                name = Settings.MerchantCode;
            }
        }
        else
        {
            name = speakerName;
        }

        return name;
    }

    public void ShowMessage(string speakerName, string message)
    {
        isShowing = true;
        isShowingMessage = true;
        
        string name = getTrueSpeakerName(speakerName);

        dialoguePanel.SetActive(true);
        nameText.text = name;
        StopAllCoroutines();
        StartCoroutine(TypeSentence(ReplaceWithNames(message), null));

        EventHandler.CallDisablePlayerMovementEvent();
        EventHandler.CallDisableCursorEvent();
        Debug.Log("Disable cursor in Dialogue Manager!");

        animator.SetBool("isWalking", false);

        FocusCameraOnSpeaker(speakerName);
    }

    public void HideMessage()
    {
        isShowing = false;
        isShowingMessage = false;
        dialoguePanel.SetActive(false);
        EventHandler.CallEnablePlayerMovementEvent();
        EventHandler.CallEnableCursorEvent();
        Debug.Log("Enable cursor in Dialogue Manager!");

        ResetCameraFocus();
    }

    private void OnStartConversation(Conversation conversation)
    {
        if (isShowing)
            return;
        Debug.Log("Conversation Started!");
        isShowing = true;
        if (dialogues == null)
        {
            dialogues = new Queue<ConversationDialogue>();
        }
        dialogues.Clear();
        currentDialogues = conversation.conversationDialogues;
        currentDialogueIndex = 0;
        currentConversationID = conversation.ID;

        dialoguePanel.SetActive(true);

        EventHandler.CallDisablePlayerMovementEvent();
        EventHandler.CallDisableCursorEvent();
        Debug.Log("Disable cursor in Dialogue Manager!");

        animator.SetBool("isWalking", false);
        DisplayCurrentDialogue();
    }

    public void EndConversation()
    {
        isShowing = false;
        dialoguePanel.SetActive(false);
        choicePanel.SetActive(false);


        GameStateManager.MarkConversationCompleted(currentConversationID);

        EventHandler.CallEnablePlayerMovementEvent();
        EventHandler.CallEnableCursorEvent();
        Debug.Log("Enable cursor in Dialogue Manager!");

        ResetCameraFocus();
    }

    public void DisplayCurrentDialogue()
    {
        if (isChoiceActivated)
        {
            return;
        }

        if (currentDialogueIndex >= currentDialogues.Length)
        {
            EndConversation();
            return;
        }

        ConversationDialogue dialogue = currentDialogues[currentDialogueIndex];

        if (!string.IsNullOrEmpty(dialogue.sceneTo))
        {
            EventHandler.CallTransitionEvent(dialogue.sceneTo, dialogue.spawnID);
            EventHandler.CallPlayerFaceEvent(dialogue.direction);
        }

        string speakerName = getTrueSpeakerName(dialogue.speakerName);
        Debug.Log($"MerchantShowHisName:{GameStateManager.GetBool("MerchantShowHisName")}");
        // string speakerName = dialogue.speakerName == "Player" ? GameStateManager.PlayerName : dialogue.speakerName;

        if (!string.IsNullOrEmpty(dialogue.divergence.parameter.parameterName) && GameStateManager.IsConditionMet(dialogue.divergence.parameter))
        {
            currentDialogueIndex = dialogue.divergence.nextIndex;
            DisplayCurrentDialogue();
            return;
        }
        else if (!string.IsNullOrEmpty(dialogue.divergence.parameter.parameterName) && !GameStateManager.IsConditionMet(dialogue.divergence.parameter))
        {
            currentDialogueIndex = dialogue.divergence.elseIndex;
            DisplayCurrentDialogue();
            return;
        }

        // Check the required conditions
        if (dialogue.requirements != null && dialogue.requirements.Count > 0 && !GameStateManager.AreConditionsMet(dialogue.requirements))
        {
            currentDialogueIndex = dialogue.nextIndex;
            DisplayCurrentDialogue();
            return;
        }

        ApplyDialogueActions(dialogue.actions);

        StartCoroutine(DisplayDialogueAfterAnimations(dialogue));
        //TriggerAnimations(dialogue.triggers);

        //if (!string.IsNullOrEmpty(dialogue.text))
        //{
        //    nameText.text = speakerName;
        //    dialoguePanel.SetActive(true);
        //    StopAllCoroutines();

        //    typingChoices = dialogue.choices;
        //    StartCoroutine(TypeSentence(ReplaceWithNames(dialogue.text), () => {
        //        if (typingChoices != null && typingChoices.Length > 0)
        //        {
        //            DisplayChoices(typingChoices);
        //        }
        //    }));

        //    // StartCoroutine(TypeSentence(ReplaceWithNames(dialogue.text)));

        //    FocusCameraOnSpeaker(dialogue.speakerName);

        //    //if (dialogue.choices != null && dialogue.choices.Length > 0 && !isTyping)
        //    //{
        //    //    DisplayChoices(dialogue.choices);
        //    //}

        //    currentDialogueIndex = dialogue.nextIndex;
        //}
        //else
        //{
        //    StartCoroutine(WaitForAnimations(dialogue.triggers));
        //    // currentDialogueIndex = dialogue.nextIndex;
        //    // DisplayCurrentDialogue();
        //}
    }

    private void ApplyDialogueActions(List<DialogueAction> actions)
    {
        if (actions == null) return;

        foreach (var action in actions)
        {
            GameStateManager.SetBool(action.parameterName, action.value);
        }
    }

    private IEnumerator TriggerAnimations(List<AnimationTrigger> triggers)
    {
        foreach (AnimationTrigger trigger in triggers)
        {
            if (animators.TryGetValue(trigger.characterName, out Animator animator))
            {
                animator.SetTrigger(trigger.triggerName);
                AnimationMovement movement = animator.gameObject.GetComponent<AnimationMovement>();

                if (movement != null)
                {
                    yield return StartCoroutine(movement.WaitForAnimation(trigger.triggerName, AnimationType.Emote));
                }
                else
                {
                    Debug.LogWarning($"AnimationMovement component not found on {trigger.characterName}.");
                }
            }
            else
            {
                Debug.LogWarning($"Animator with name {trigger.characterName} not found.");
            }
        }
    }

    private IEnumerator DisplayDialogueAfterAnimations(ConversationDialogue dialogue)
    {
        isAnimationPlaying = true;
        yield return StartCoroutine(TriggerAnimations(dialogue.triggers));
        isAnimationPlaying = false;

        if (!string.IsNullOrEmpty(dialogue.text))
        {
            nameText.text = getTrueSpeakerName(dialogue.speakerName);
            dialoguePanel.SetActive(true);
            StopAllCoroutines();

            typingChoices = dialogue.choices;
            StartCoroutine(TypeSentence(ReplaceWithNames(dialogue.text), () => {
                if (typingChoices != null && typingChoices.Length > 0)
                {
                    DisplayChoices(typingChoices);
                }
            }));

            FocusCameraOnSpeaker(dialogue.speakerName);

            currentDialogueIndex = dialogue.nextIndex;
        }
        else
        {
            currentDialogueIndex = dialogue.nextIndex;
            DisplayCurrentDialogue();
        }
    }

    private IEnumerator WaitForAnimations(List<AnimationTrigger> triggers)
    {
        isAnimationPlaying = true;
        foreach (AnimationTrigger trigger in triggers)
        {
            if (animators.TryGetValue(trigger.characterName, out Animator animator))
            {
                AnimationMovement movement = animator.gameObject.GetComponent<AnimationMovement>();
                if (movement != null)
                {
                    yield return StartCoroutine(movement.WaitForAnimation(trigger.triggerName, AnimationType.Emote));
                }
            }
        }

        isAnimationPlaying = false;

        currentDialogueIndex = currentDialogues[currentDialogueIndex].nextIndex;
        DisplayCurrentDialogue();
    }

    private string ReplaceWithNames(string str)
    {
        string newString = str.Replace("[Player]", GameStateManager.PlayerName);
        newString = newString.Replace("[Merchant]", Settings.MerchantName);
        return newString;
    }

    private void DisplayChoices(DialogueChoice[] choices)
    {
        isChoiceActivated = true;
        choicePanel.SetActive(true);

        foreach (Button button in buttons)
        {
            Destroy(button.gameObject);
        }
        buttons.Clear();

        foreach (DialogueChoice choice in choices)
        {
            Button choiceButton = Instantiate(choiceButtonPrefab, choicePanel.transform);
            choiceButton.GetComponentInChildren<Text>().text = choice.choiceText;
            choiceButton.onClick.AddListener(() => OnChoiceSelected(choice));
            buttons.Add(choiceButton);
        }
    }

    private void OnChoiceSelected(DialogueChoice choice)
    {
        if (choice.triggerPickUp)
        {
            choice.investObject.PickUp();
        }
        choicePanel.SetActive(false);
        isChoiceActivated = false;
        currentDialogueIndex = choice.nextIndex;
        DisplayCurrentDialogue();
    }

    public bool GetIsShowing()
    {
        return isShowing;
    }

    private void OnDialoguePanelClick(BaseEventData data)
    {
        if (isAnimationPlaying)
        {
            return;
        }

        if (isShowingMessage)
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = typingSentence;
                isTyping = false;
            }
            else
            {
                EventHandler.CallHideMessageEvent();
            }
        }
        else
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = typingSentence;
                isTyping = false;

                if (typingChoices != null && typingChoices.Length > 0)
                {
                    DisplayChoices(typingChoices);
                }
            }
            else
            {
                DisplayCurrentDialogue();
            }
        }
    }


    private void AddEventTrigger(GameObject obj, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = obj.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }

    IEnumerator TypeSentence(string sentence, Action onComplete)
    {
        isTyping = true;
        typingSentence = sentence;
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(playSpeed);
        }

        isTyping = false;
        onComplete?.Invoke(); // Call the callback when typing is complete
    }

    private void FocusCameraOnSpeaker(string speakerName)
    {
        if (animators.TryGetValue(speakerName, out Animator animator))
        {
            Debug.Log($"Got the animator for {speakerName}");
            Transform speakerTransform = animator.transform;

            if (targetGroup != null)
            {
                bool playerInGroup = false;
                bool speakerInGroup = false;

                // Ensure player is in the group
                for (int i = 0; i < targetGroup.m_Targets.Length; i++)
                {
                    if (targetGroup.m_Targets[i].target == speakerTransform)
                    {
                        speakerInGroup = true;
                        targetGroup.m_Targets[i].weight = focusWeight;
                    }
                    else
                    {
                        targetGroup.m_Targets[i].weight = othersWeight;
                    }
                    if (targetGroup.m_Targets[i].target == player.transform)
                    {
                        playerInGroup = true;
                        targetGroup.m_Targets[i].weight = defaultWeight;
                    }
                }

                if (!playerInGroup)
                {
                    targetGroup.AddMember(player.transform, defaultWeight, defaultRadius); // Add player with high weight
                }


                if (!speakerInGroup)
                {
                    targetGroup.AddMember(speakerTransform, focusWeight, defaultOthersRadius); // Add speaker with high weight
                }
            }
        }
        else
        {
            Debug.Log($"Did not get the animator for {speakerName}");
        }
    }

    private void ResetCameraFocus()
    {
        if (targetGroup != null)
        {
            // Create a new list to store the members to remove
            List<CinemachineTargetGroup.Target> membersToRemove = new List<CinemachineTargetGroup.Target>();

            // Identify all members except the player for removal
            for (int i = 0; i < targetGroup.m_Targets.Length; i++)
            {
                if (targetGroup.m_Targets[i].target != player.transform)
                {
                    membersToRemove.Add(targetGroup.m_Targets[i]);
                }
                else
                {
                    targetGroup.m_Targets[i].radius = defaultRadius;
                    targetGroup.m_Targets[i].weight = defaultWeight;
                }
            }

            // Remove non-player members from the target group
            foreach (var member in membersToRemove)
            {
                targetGroup.RemoveMember(member.target);
            }
        }
    }
}
