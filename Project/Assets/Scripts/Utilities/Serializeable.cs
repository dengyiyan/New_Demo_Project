
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BooleanParameter
{
    public string parameterName;
    public bool value;
}

[Serializable]
public class DialogueAction
{
    public string parameterName; // The name of the parameter to modify
    public bool value; // The value to set
}

[System.Serializable]
public class SequenceCondition
{
    public enum ConditionType { None, SequencePlayed, EventTriggered, SceneEntryCount, ConversationEnded }
    public ConditionType conditionType = ConditionType.None;
    public string parameter;
    public int entryCount;
}

[System.Serializable]
public class SequenceConditions
{
    public List<SequenceCondition> conditions;
}

[System.Serializable]
public class AnimationStep
{
    public string npcName;
    public List<Vector3> points;
    public string trigger; // Use to trigger specific animations like emotes
    public bool waitForCompletion;
    public Conversation conversation;
    public bool waitForConversationEnd;
    public bool allowPlayerMove;
    public AnimationType animationType;
    public List<BooleanParameter> boolParameters;
    [SceneName] public string nextScene;
    public string spawnID;
    // public AnimationSequence nextSequence;
}

[System.Serializable]
public class AnimationTrigger
{
    // public Animator animator;
    public string characterName;
    public string triggerName;
}

[System.Serializable]
public class ConversationDialogue
{
    public string speakerName;

    [TextArea(3, 10)]
    public string text;
    public DialogueChoice[] choices;
    public int nextIndex = 0;  // In case no choices at all
    public List<AnimationTrigger> triggers;
    public List<BooleanParameter> requirements;
    public List<DialogueAction> actions;
}


[System.Serializable]
public class Conversation
{
    public ConversationDialogue[] conversationDialogues;
    public string ID;
}


[System.Serializable]
public class DialogueChoice
{
    public string choiceText;
    public int nextIndex;
}


[System.Serializable]
public class NPCState
{
    public string npcName;
    public Vector3 position;
}

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
{
    [SerializeField]
    private List<TKey> keys = new List<TKey>();

    [SerializeField]
    private List<TValue> values = new List<TValue>();

    private Dictionary<TKey, TValue> targetDictionary = new Dictionary<TKey, TValue>();

    public SerializableDictionary(Dictionary<TKey, TValue> targetDictionary)
    {
        this.targetDictionary = targetDictionary;
    }

    public SerializableDictionary()
    {
    }

    public Dictionary<TKey, TValue> ToDictionary()
    {
        return targetDictionary;
    }

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach (var kvp in targetDictionary)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        targetDictionary = new Dictionary<TKey, TValue>();

        for (int i = 0; i < keys.Count; i++)
        {
            targetDictionary[keys[i]] = values[i];
        }
    }

    public void Add(TKey key, TValue value)
    {
        targetDictionary[key] = value;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        return targetDictionary.TryGetValue(key, out value);
    }

}