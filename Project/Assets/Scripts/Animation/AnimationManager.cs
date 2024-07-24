//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Playables;
//using UnityEngine.SceneManagement;

//public class AnimationManager : MonoBehaviour
//{
//    [SerializeField] private List<AnimationSequence> sequences;
//    private Dictionary<string, GameObject> npcs;
//    // [SerializeField] private PlayerMovement playerMovement;
//    // [SerializeField] private UIController uiController;
//    // [SerializeField] private DialogueManager dialogueManager;
//    [SerializeField] private AnimationSequence startingSequence;

//    public bool isAnimationPlaying;
//    private int animationCounter = 0;
//    private List<AnimationStep> activeSteps = new List<AnimationStep>();
//    private GameObject player;
//    private Animator animator;
//    [SerializeField] private PlayableDirector playableDirector;

//    // private SceneStateHandler sceneStateHandler;

//    private void Start()
//    {
//        npcs = new Dictionary<string, GameObject>();
//        // playerMovement = FindObjectOfType<PlayerMovement>();
//        // uiController = FindObjectOfType<UIController>();
//        // dialogueManager = FindObjectOfType<DialogueManager>();
//        // sceneStateHandler = FindObjectOfType<SceneStateHandler>();

//        player = GameObject.FindGameObjectWithTag("Player");
//        animator = player.GetComponent<Animator>();

//        playableDirector = FindObjectOfType<PlayableDirector>();

//        foreach (var npc in FindObjectsOfType<AnimationMovement>())
//        {
//            npcs[npc.name] = npc.gameObject;
//        }

//        if (startingSequence != null)
//        {
//            StartCoroutine(PlaySequenceCoroutine(startingSequence));
//        }
//    }

//    private void Update()
//    {
//        // Debug.Log(activeSteps.Count);
//    }

//    private IEnumerator HandleSequences()
//    {
//        foreach (var seq in sequences)
//        {
//            while (GameStateManager.IsSequencePlayed(seq.sequenceName) || !ShouldPlaySequence(seq.conditions))
//            {
//                if (GameStateManager.IsSequencePlayed(seq.sequenceName))
//                {
//                    Debug.Log($"Sequence {seq.sequenceName} has already been played.");
//                    break; // Skip to the next sequence
//                }

//                if (!ShouldPlaySequence(seq.conditions))
//                {
//                    Debug.Log($"Conditions for sequence {seq.sequenceName} are not fulfilled. Pausing until conditions are met.");
//                }

//                yield return null; // Wait for the next frame and check again
//            }

//            if (!GameStateManager.IsSequencePlayed(seq.sequenceName) && ShouldPlaySequence(seq.conditions))
//            {
//                yield return StartCoroutine(PlaySequenceCoroutine(seq)); // Ensure sequence is fully completed
//            }
//        }

//        //uiController.OnAnimationEnd();
//        //if (playerMovement != null)
//        //{
//        //    Debug.Log("enable");
//        //    playerMovement.EnableMovement();
//        //}
//    }

//    public void PlaySequence(AnimationSequence sequence)
//    {
//        animator.SetBool("isWalking", false);
//        if (!GameStateManager.IsSequencePlayed(sequence.sequenceName))
//        {
//            StartCoroutine(PlaySequenceCoroutine(sequence));
//        }
//        else
//        {
//            HandlePlayerMovement();
//            Debug.Log($"Sequence {sequence.sequenceName} already played.");
//        }
//    }

//    private bool ShouldPlaySequence(SequenceConditions conditions)
//    {
//        foreach (var condition in conditions.conditions)
//        {
//            if (!CheckCondition(condition))
//            {
//                return false;
//            }
//        }

//        return true;
//    }

//    private bool CheckCondition(SequenceCondition condition)
//    {
//        switch (condition.conditionType)
//        {
//            case SequenceCondition.ConditionType.None:
//                return true;
//            case SequenceCondition.ConditionType.SequencePlayed:
//                return GameStateManager.IsSequencePlayed(condition.parameter);
//            case SequenceCondition.ConditionType.EventTriggered:
//                // Implement event trigger condition logic here
//                return false;
//            case SequenceCondition.ConditionType.ConversationEnded:
//                return GameStateManager.IsConversationCompleted(condition.parameter);
//            default:
//                return false;
//        }
//    }

//    private void HandleSceneChange(AnimationStep step)
//    {
//        EventHandler.CallTransitionEvent(step.nextScene, step.spawnID);
//    }

//    private IEnumerator PlaySequenceCoroutine(AnimationSequence sequence)
//    {
//        Debug.Log($"Playing sequence {sequence.sequenceName}");
//        GameStateManager.MarkSequencePlayed(sequence.sequenceName);

//        foreach (var step in sequence.steps)
//        {
//            if (npcs.TryGetValue(step.npcName, out GameObject npc))
//            {
//                Debug.Log(step.animationType);
//                if (step.animationType == AnimationType.Movement)
//                {
//                    yield return StartCoroutine(HandleMovementStep(npc, step));
//                }
//                else if (step.animationType == AnimationType.Conversation)
//                {
//                    yield return StartCoroutine(HandleConversationStep(npc, step));
//                }
//                else if (step.animationType == AnimationType.SceneChange)
//                {
//                    HandleSceneChange(step);
//                }
//                else if (step.animationType == AnimationType.CutSceneTrigger)
//                {
//                    playableDirector.Play();
//                    yield return StartCoroutine(HandleMovementStep(npc, step));
//                    if (step.nextScene != null)
//                    {
//                        HandleSceneChange(step);
//                    }
//                }
//                else if (step.animationType == AnimationType.None)
//                {
//                }
//                else
//                {
//                    yield return StartCoroutine(HandleAnimationStep(npc, step));
//                }

//                foreach (var boolParam in step.boolParameters)
//                {
//                    EventHandler.CallSetBoolParameter(npc, boolParam.parameterName, boolParam.value);
//                }

//                if (step.waitForCompletion)
//                {
//                    yield return new WaitUntil(() => npc.GetComponent<NPCMovement>().HasCompletedPath || (step.conversation != null && !dialogueManager.GetIsShowing()));
//                }
//            }
//        }

//        if (playerMovement != null && !dialogueManager.GetIsShowing())
//        {
//            playerMovement.EnableMovement();
//        }
//    }

//    private IEnumerator HandleAnimationStep(GameObject npc, AnimationStep step)
//    {
//        EventHandler.CallPlayAnimation(npc, step.trigger, step.animationType);

//        if (!step.allowPlayerMove)
//        {
//            activeSteps.Add(step);
//            IncrementAnimationCounter();
//        }

//        yield return StartCoroutine(npc.GetComponent<NPCMovement>().WaitForAnimation(step.trigger, step.animationType));

//        if (step.conversation != null && step.conversation.conversationDialogues.Length != 0)
//        {
//            EventHandler.CallStartConversation(npc, step.conversation);

//            if (step.waitForConversationEnd)
//            {
//                yield return new WaitUntil(() => !dialogueManager.GetIsShowing());
//            }
//        }

//        if (!step.allowPlayerMove)
//        {
//            activeSteps.Remove(step);
//            DecrementAnimationCounter();
//        }
//    }

//    private IEnumerator HandleMovementStep(GameObject npc, AnimationStep step)
//    {
//        EventHandler.CallMoveCharacter(npc, step.points);
//        npc.GetComponent<NPCMovement>().SetWalking(true);

//        if (!step.allowPlayerMove)
//        {
//            activeSteps.Add(step);
//            IncrementAnimationCounter();
//        }

//        yield return new WaitUntil(() => npc.GetComponent<NPCMovement>().HasCompletedPath);

//        npc.GetComponent<NPCMovement>().SetWalking(false);

//        if (!step.allowPlayerMove)
//        {
//            activeSteps.Remove(step);
//            DecrementAnimationCounter();
//        }
//    }

//    private IEnumerator HandleConversationStep(GameObject npc, AnimationStep step)
//    {
//        if (step.conversation != null && step.conversation.conversationDialogues.Length != 0)
//        {
//            EventHandler.CallStartConversation(npc, step.conversation);

//            if (step.waitForConversationEnd)
//            {
//                yield return new WaitUntil(() => !dialogueManager.GetIsShowing());
//            }
//        }
//    }

//    private void IncrementAnimationCounter()
//    {
//        animationCounter++;
//        HandlePlayerMovement();
//    }

//    private void DecrementAnimationCounter()
//    {
//        animationCounter--;
//        HandlePlayerMovement();
//    }

//    private void HandlePlayerMovement()
//    {
//        if (playerMovement != null)
//        {
//            if (activeSteps.Count > 0 || dialogueManager.GetIsShowing())
//            {
//                EventHandler.CallDisablePlayerMovement();
//            }
//            else
//            {
//                EventHandler.CallEnablePlayerMovement();
//            }
//        }
//    }

//    public List<AnimationStep> GetActiveSteps()
//    {
//        return activeSteps;
//    }
//}

using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;


public class AnimationManager : MonoBehaviour
{
    [SerializeField] private List<AnimationSequence> sequences;
    private Dictionary<string, GameObject> npcs;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private AnimationSequence startingSequence;

    public bool isAnimationPlaying;
    private int animationCounter = 0;
    private List<AnimationStep> activeSteps = new List<AnimationStep>();
    private GameObject player;
    private Animator animator;
    [SerializeField] private PlayableDirector playableDirector;


    private void Start()
    {
        npcs = new Dictionary<string, GameObject>();

        player = GameObject.FindGameObjectWithTag("Player");
        animator = player.GetComponent<Animator>();

        playableDirector = FindObjectOfType<PlayableDirector>();
        dialogueManager = FindObjectOfType<DialogueManager>();

        foreach (var npc in FindObjectsOfType<AnimationMovement>())
        {
            npcs[npc.name] = npc.gameObject;
        }

        if (startingSequence != null)
        {
            PlaySequence(startingSequence);
        }
        // StartCoroutine(HandleSequences());
    }

    private void Update()
    {
        // Debug.Log(activeSteps.Count);
    }

    //private IEnumerator HandleSequences()
    //{
    //    foreach (var seq in sequences)
    //    {
    //        while (GameStateManager.IsSequencePlayed(seq.sequenceName) || !ShouldPlaySequence(seq.conditions))
    //        {
    //            if (GameStateManager.IsSequencePlayed(seq.sequenceName))
    //            {
    //                Debug.Log($"Sequence {seq.sequenceName} has already been played.");
    //                break; // Skip to the next sequence
    //            }

    //            if (!ShouldPlaySequence(seq.conditions))
    //            {
    //                Debug.Log($"Conditions for sequence {seq.sequenceName} are not fulfilled. Pausing until conditions are met.");
    //            }

    //            yield return null; // Wait for the next frame and check again
    //        }

    //        if (!GameStateManager.IsSequencePlayed(seq.sequenceName) && ShouldPlaySequence(seq.conditions))
    //        {
    //            yield return StartCoroutine(PlaySequenceCoroutine(seq)); // Ensure sequence is fully completed
    //        }
    //    }

    //    //uiController.OnAnimationEnd();
    //    //if (playerMovement != null)
    //    //{
    //    //    Debug.Log("enable");
    //    //    playerMovement.EnableMovement();
    //    //}
    //}

    public void PlaySequence(AnimationSequence sequence)
    {
        animator.SetBool("isWalking", false);
        
        //var sequence = sequences.Find(seq => seq.sequenceName == sequenceName);
        //if (sequence != null && !GameStateManager.IsSequencePlayed(sequenceName))
        //{
        //    StartCoroutine(PlaySequenceCoroutine(sequence));
        //}
        if (!GameStateManager.IsSequencePlayed(sequence.sequenceName))
        {
            // GameStateManager.PrintHashSet(GameStateManager.getSeqencesPlayed());
            StartCoroutine(PlaySequenceCoroutine(sequence));
        }
        else
        {
            HandlePlayerMovement();
            Debug.Log($"Sequence {sequence.sequenceName} already played.");
        }

    }

    private bool ShouldPlaySequence(SequenceConditions conditions)
    {
        foreach (var condition in conditions.conditions)
        {
            if (!CheckCondition(condition))
            {
                return false;
            }
        }

        return true;
    }

    private bool CheckCondition(SequenceCondition condition)
    {
        switch (condition.conditionType)
        {
            case SequenceCondition.ConditionType.None:
                return true;
            case SequenceCondition.ConditionType.SequencePlayed:
                return GameStateManager.IsSequencePlayed(condition.parameter);
            case SequenceCondition.ConditionType.EventTriggered:

                //need implementation
                return false;
            case SequenceCondition.ConditionType.ConversationEnded:
                return GameStateManager.IsConversationCompleted(condition.parameter);
            default:
                return false;

        }
    }

    private void HandleSceneChange(AnimationStep step)
    {
        EventHandler.CallTransitionEvent(step.nextScene, step.spawnID);
    }
    private IEnumerator PlaySequenceCoroutine(AnimationSequence sequence)
    {
        EventHandler.CallDisableCursorEvent();
        Debug.Log("Disable cursor in Animation Manager!");


        Debug.Log($"Playing sequence {sequence.sequenceName}");
        GameStateManager.MarkSequencePlayed(sequence.sequenceName);


        // EventHandler.CallDisableCursorEvent();
        foreach (var step in sequence.steps)
        {
            if (npcs.TryGetValue(step.npcName, out GameObject npc))
            {
                Debug.Log(step.animationType);
                AnimationMovement movementScript = npc.GetComponent<AnimationMovement>(); 
                if (step.animationType == AnimationType.Movement)
                {
                    yield return StartCoroutine(HandleMovementStep(movementScript, step));
                }
                else if (step.animationType == AnimationType.Conversation)
                {
                    yield return StartCoroutine(HandleConversationStep(movementScript, step));
                }
                else if (step.animationType == AnimationType.SceneChange)
                {
                    HandleSceneChange(step);
                }
                else if (step.animationType == AnimationType.CutSceneTrigger)
                {
                    // playableDirector.Play();
                    EventHandler.CallFadeInEvent();
                    yield return StartCoroutine(HandleMovementStep(movementScript, step));
                    if (!string.IsNullOrEmpty(step.nextScene))
                    {
                        HandleSceneChange(step);
                    }
                    EventHandler.CallFadeOutEvent();
                }
                else if (step.animationType == AnimationType.None)
                {
                }
                else
                {
                    yield return StartCoroutine(HandleAnimationStep(movementScript, step));
                }

                foreach (var boolParam in step.boolParameters)
                {
                    movementScript.SetBoolParameter(boolParam.parameterName, boolParam.value);
                }

                if (step.waitForCompletion)
                {
                    yield return new WaitUntil(() => movementScript.HasCompletedPath || (step.conversation != null && !dialogueManager.GetIsShowing()));
                }
            }
        }



        // uiController.OnAnimationEnd();
        if (!dialogueManager.GetIsShowing())
        {
            EventHandler.CallEnablePlayerMovementEvent();
            EventHandler.CallEnableCursorEvent();
            Debug.Log("Enable cursor in Animation Manager!");
        }
    }

    private IEnumerator HandleAnimationStep(AnimationMovement npc, AnimationStep step)
    {
        npc.SetAnimationTrigger(step.trigger);
        // npc.SetAnimationTrigger(step.startTrigger, step.endTrigger, "Emote");

        if (!step.allowPlayerMove)
        {
            activeSteps.Add(step);
            IncrementAnimationCounter();
        }

        yield return StartCoroutine(npc.WaitForAnimation(step.trigger, step.animationType));

        if (step.conversation != null && step.conversation.conversationDialogues.Length != 0)
        {
            EventHandler.CallStartConversationEvent(step.conversation);

            if (step.waitForConversationEnd)
            {
                yield return new WaitUntil(() => !dialogueManager.GetIsShowing());
            }
        }

        if (!step.allowPlayerMove)
        {
            activeSteps.Remove(step);
            DecrementAnimationCounter();
        }
    }

    private IEnumerator HandleMovementStep(AnimationMovement npc, AnimationStep step)
    {
        npc.SetPoints(step.points);
        npc.SetWalking(true);

        if (!step.allowPlayerMove)
        {
            Debug.Log($"Adding step: {step}");
            activeSteps.Add(step);
            IncrementAnimationCounter();
        }

        yield return StartCoroutine(npc.MoveAlongPoints());


        npc.SetWalking(false);

        if (!step.allowPlayerMove)
        {
            Debug.Log($"Removing step: {step}");
            activeSteps.Remove(step);
            DecrementAnimationCounter();
        }
    }

    private IEnumerator HandleConversationStep(AnimationMovement npc, AnimationStep step)
    {
        if (step.conversation != null && step.conversation.conversationDialogues.Length != 0)
        {
            EventHandler.CallStartConversationEvent(step.conversation);

            if (step.waitForConversationEnd)
            {
                yield return new WaitUntil(() => !dialogueManager.GetIsShowing());
            }
        }
    }

    private IEnumerator WaitForNPCAndHandleConversation(AnimationMovement npc, AnimationStep step)
    {
        yield return StartCoroutine(npc.MoveAlongPoints());

        if (step.conversation.conversationDialogues.Length != 0)
        {
            EventHandler.CallStartConversationEvent(step.conversation);

            if (step.waitForConversationEnd)
            {
                yield return new WaitUntil(() => !dialogueManager.GetIsShowing());
            }
        }


        if (!step.allowPlayerMove)
        {
            activeSteps.Remove(step);
            DecrementAnimationCounter();
        }

    }

    private void IncrementAnimationCounter()
    {
        animationCounter++;
        HandlePlayerMovement();
        // HandleShow();

    }

    private void DecrementAnimationCounter()
    {
        animationCounter--;
        HandlePlayerMovement();
        // HandleHide();
    }

    private void HandlePlayerMovement()
    {
        if (activeSteps.Count > 0 || dialogueManager.GetIsShowing())
        {
            EventHandler.CallDisablePlayerMovementEvent();
        }
        else
        {
            EventHandler.CallEnablePlayerMovementEvent();
        }
    }

    public List<AnimationStep> GetActiveSteps()
    {
        return activeSteps;
    }

}
