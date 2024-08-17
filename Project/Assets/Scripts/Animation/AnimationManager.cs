using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;


public class AnimationManager : MonoBehaviour
{
    //[SerializeField] private List<AnimationSequence> sequences;
    private Dictionary<string, GameObject> npcs = new Dictionary<string, GameObject>();
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private AnimationSequence startingSequence;

    public bool isAnimationPlaying;
    private int animationCounter = 0;
    private List<AnimationStep> activeSteps = new List<AnimationStep>();
    private GameObject player;
    private Animator animator;

    [SerializeField] private PlayableDirector playableDirector;
    [SerializeField] private List<PlayableAsset> playables = new List<PlayableAsset>();
    private int playableIndex = 0;


    private void OnEnable()
    {
        EventHandler.RegisterNPCEvent += OnRegisterNPCEvent;
        //EventHandler.RegisterPlayableDirectorEvent += SetPlayableDirector;
        //EventHandler.UnregisterPlayableDirectorEvent += UnregisterPlayableDirector;
    }

    private void OnDisable()
    {
        EventHandler.RegisterNPCEvent -= OnRegisterNPCEvent;
        //EventHandler.RegisterPlayableDirectorEvent -= SetPlayableDirector;
        //EventHandler.UnregisterPlayableDirectorEvent -= UnregisterPlayableDirector;
    }

    private void Start()
    {
        //npcs = new Dictionary<string, GameObject>();

        player = GameObject.FindGameObjectWithTag("Player");
        if (player)
            animator = player.GetComponent<Animator>();

        // npcs["Player"] = player;
        //Debug.Log($"NPCs:{npcs["Player"]}");

        // playableDirector = FindObjectOfType<PlayableDirector>();
        dialogueManager = FindObjectOfType<DialogueManager>();

        if (startingSequence != null)
        {
            PlaySequence(startingSequence);
        }
    }


    private void Update()
    {
        // Debug.Log(activeSteps.Count);
    }

    //private void SetPlayableDirector(PlayableDirector playable)
    //{
    //    playableDirector = playable;
    //}

    //private void UnregisterPlayableDirector()
    //{
    //    playableDirector = null;
    //}


    public void SetStartingSequence(AnimationSequence newSequence)
    {
        startingSequence = newSequence;
    }

    private void OnRegisterNPCEvent(Dictionary<string, GameObject> in_npcs)
    {
        npcs = in_npcs;
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
        EventHandler.CallIncreaseDisableEvent();
        EventHandler.CallDisableCursorEvent();
        //Debug.Log("Disable cursor in Animation Manager!");
        if (animator)
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
            //Debug.Log($"Sequence {sequence.sequenceName} already played.");
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
        EventHandler.CallTransitionEvent(step.nextScene, step.spawnID, step.newStartingSequence);
        EventHandler.CallPlayerFaceEvent(step.direction);
    }
    private IEnumerator PlaySequenceCoroutine(AnimationSequence sequence)
    {

        // EventHandler.CallDisableCursorEvent();
        // Debug.Log("Disable cursor in Animation Manager!");

        //Debug.Log($"Playing sequence {sequence.sequenceName}");
        GameStateManager.MarkSequencePlayed(sequence.sequenceName);


        // EventHandler.CallDisableCursorEvent();
        foreach (var step in sequence.steps)
        {
            //Debug.Log($"Trying to get {step.npcName}");
            if (npcs.TryGetValue(step.npcName, out GameObject npc))
            {
                //Debug.Log(step.animationType);
                AnimationMovement movementScript = npc.GetComponent<AnimationMovement>();

                if (movementScript == null)
                {
                    //Debug.LogWarning("movement missing");
                }
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
                    EventHandler.CallOnSetTimelinePlaying(true);
                    Debug.Log($"Playing index {playableIndex}");
                    playableDirector.playableAsset = playables[playableIndex];
                    playableDirector.Play();
                    playableIndex++;

                    yield return new WaitUntil(() => playableDirector.state != PlayState.Playing);

                    EventHandler.CallOnSetTimelinePlaying(false);
                    //EventHandler.CallFadeInEvent();
                    //yield return StartCoroutine(HandleMovementStep(movementScript, step));
                    //if (!string.IsNullOrEmpty(step.nextScene))
                    //{
                    //    HandleSceneChange(step);
                    //}
                    //EventHandler.CallFadeOutEvent();
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
            else
            {
                //Debug.LogWarning("Not found!");
            }
        }



        // uiController.OnAnimationEnd();
        //if (!dialogueManager.GetIsShowing())
        // {
        EventHandler.CallDecreaseDisableEvent();
        EventHandler.CallEnableCursorEvent();
        //Debug.Log("Enable cursor in Animation Manager!");
        // }
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
            //Debug.Log($"Adding step: {step}");
            activeSteps.Add(step);
            IncrementAnimationCounter();
        }

        if (step.points.Count > 0)
        {
            yield return StartCoroutine(npc.MoveAlongPoints());
        }


        npc.SetWalking(false);

        if (!step.allowPlayerMove)
        {
            //Debug.Log($"Removing step: {step}");
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
            EventHandler.CallIncreaseDisableEvent();
        }
        else
        {
            EventHandler.CallDecreaseDisableEvent();
        }
    }

    public List<AnimationStep> GetActiveSteps()
    {
        return activeSteps;
    }

}
