public enum InteractionType
{
    NPC, Invest, Door, Normal, None
}

public enum AnimationType
{
    Movement,
    Emote,
    Wake,
    Conversation,
    SceneChange,
    CutSceneTrigger,
    None,
    // Add other animation types as needed
}

[System.Serializable]
public enum Direction
{
    None,
    Front,
    Back,
    Left,
    Right,
    // Add other animation types as needed
}

public enum ImageType
{
    Happy,
    Angry,
    Fearful,
    Surprised,
    Sad,
    Disgusted,
    Neutral,
}
