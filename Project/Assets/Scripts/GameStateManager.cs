using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameStateManager
{
    public static string PlayerName { get; set; } = "Jack";
    public static int Followers = 78;
    //private static bool PlayerCanMove = false;

    private static Texture2D UploadedImage;

    private static Texture2D HappyImage;
    private static Texture2D SadImage;
    private static Texture2D FearfulImage;
    private static Texture2D AngryImage;
    private static Texture2D DisgustedImage;
    private static Texture2D SurprisedImage;
    private static Texture2D NeutralImage;

    private const string exercise2Flag = "Excercise2Completed";

    private static readonly List<string> replayableAnimations = new List<string> 
    { 
        "sat_exercise2",
        "SAT_embrace",
        "SAT_embrace_unhappy",
        "SAT_dance",
        "SAT_play",
        "SAT_play_after",
        "hide_and_seek_start",
        "sat_exercise8_after",
    };

    private static Dictionary<string, bool> gameBools = new Dictionary<string, bool>
    {
        {"HasFlower", false},
        {"SisterReceiveFlower", false},
        {"MerchantShowHisName", false },
        {"RecognizeChildhoodSelf", false},
        {"Exercise2bCompleted", false},
        {exercise2Flag, false},
    };

    private static Dictionary<string, HashSet<string>> gameCounters = new Dictionary<string, HashSet<string>>
    {
        {exercise2Flag, new HashSet<string>{ } }
    };


    private static Dictionary<string, Vector3> scenePlayerPositions = new Dictionary<string, Vector3>();
    private static Dictionary<string, Dictionary<string, NPCState>> sceneNPCStates = new Dictionary<string, Dictionary<string, NPCState>>();
    // private static Dictionary<string, string> sceneLastSpawnPoints = new Dictionary<string, string>();
    private static HashSet<string> sequencePlayed = new HashSet<string>(); 
    private static HashSet<string> conversationsCompleted = new HashSet<string>(); // Track completed conversations
    private static HashSet<string> itemPicked = new HashSet<string>();

    private static string photoPath;
    public static float aspectRatio = -1;

    // private const string DefaultSpawnPoint = "default";

    //public static bool GetPlayerCanMove()
    //{
    //    return PlayerCanMove; 
    //}

    //public static void SetPlayerCanMove()

    public static void SetPhotoPath(string path)
    {
        photoPath = path;
    }

    public static string GetPhotoPath()
    {
        return photoPath;
    }

    public static void AddCounter(string counterName, string itemName)
    {
        gameCounters[counterName].Add(itemName);


        // Conditions to check
        if (counterName == exercise2Flag && gameCounters[counterName].Count >= 4)
        {
            SetBool(counterName, true);
        }
    }


    public static void SaveImage(Texture2D image)
    {
        UploadedImage = image;
        //HappyImage = image;
        //SadImage = image;
    }

    public static Texture2D GetUploadedImage()
    {
        return UploadedImage;
    }


    public static void SaveGeneratedImage(ImageType type, Texture2D image)
    {
        switch (type)
        {
            case ImageType.Happy:
                HappyImage = image;
                break;
            case ImageType.Sad:
                SadImage = image;
                break;
            case ImageType.Angry:
                AngryImage = image;
                break;
            case ImageType.Surprised:
                SurprisedImage = image;
                break;
            case ImageType.Disgusted:
                DisgustedImage = image;
                break;
            case ImageType.Fearful:
                FearfulImage = image;
                break;
            case ImageType.Neutral:
                NeutralImage = image;
                break;
        }
    }


    public static Texture2D GetImage(ImageType type)
    {
        switch (type)
        {
            case ImageType.Happy:
                return HappyImage;
            case ImageType.Sad:
                return SadImage;
            case ImageType.Angry:
                return AngryImage;
            case ImageType.Surprised:
                return SurprisedImage;
            case ImageType.Disgusted:
                return DisgustedImage;
            case ImageType.Fearful:
                return FearfulImage;
            case ImageType.Neutral:
                return NeutralImage;
            default:
                throw new NotImplementedException();
        }

    }

    public static void SetBool(string key, bool value)
    {
        gameBools[key] = value;
    }

    public static void PickUp(string ItemID)
    {
        itemPicked.Add(ItemID);
    }

    public static bool IsItemPicked(string itemName)
    {
        return itemPicked.Contains(itemName);
    }

    public static void DestroyPickedItems()
    {
        //Debug.Log("destroy triggered!");
        foreach (var itemName in itemPicked)
        {
            var obj = GameObject.Find(itemName);
            if (obj != null)
            {
                UnityEngine.Object.Destroy(obj);

                //Debug.Log("object destroyed!");
            }
        }
    }

    public static bool GetBool(string key)
    {
        return gameBools.ContainsKey(key) && gameBools[key];
    }

    public static bool IsConditionMet(BooleanParameter condition)
    {
        if (!gameBools.TryGetValue(condition.parameterName, out bool value) || value != condition.value)
        {
            return false;
        }
        return true;
    }

    public static bool AreConditionsMet(List<BooleanParameter> conditions)
    {
        foreach (var condition in conditions)
        {
            if (!IsConditionMet(condition))
            {
                return false;
            }
        }
        return true;
    }



    public static void SavePlayerPosition(string sceneName, Vector3 position)
    {
        scenePlayerPositions[sceneName] = position;
    }

    //public static Vector3 GetPlayerPosition(string sceneName)
    //{
    //    if (scenePlayerPositions.ContainsKey(sceneName))
    //    {
    //        return scenePlayerPositions[sceneName];
    //    }
    //    return Vector3.zero; // Default position if not found
    //}

    //public static void SaveLastSpawnPoint(string sceneName, string spawnPointID)
    //{
    //    sceneLastSpawnPoints[sceneName] = spawnPointID;
    //}

    //public static string GetLastSpawnPoint(string sceneName)
    //{
    //    if (sceneLastSpawnPoints.ContainsKey(sceneName))
    //    {
    //        return sceneLastSpawnPoints[sceneName];
    //    }
    //    return DefaultSpawnPoint; 
    //}

    //public static void SaveNPCState(string sceneName, string npcName, Vector3 position)
    //{
    //    if (!sceneNPCStates.ContainsKey(sceneName))
    //    {
    //        sceneNPCStates[sceneName] = new Dictionary<string, NPCState>();
    //    }
    //    sceneNPCStates[sceneName][npcName] = new NPCState
    //    {
    //        npcName = npcName,
    //        position = position
    //    };
    //}

    //public static Dictionary<string, NPCState> GetNPCStates(string sceneName)
    //{
    //    if (sceneNPCStates.ContainsKey(sceneName))
    //    {
    //        return sceneNPCStates[sceneName];
    //    }
    //    return new Dictionary<string, NPCState>(); // Return empty dictionary if not found
    //}

    public static void MarkSequencePlayed(string sequenceName)
    {
        if (sequenceName.StartsWith("replay_"))
        {
            return;
        }
        if (!replayableAnimations.Contains(sequenceName) && !sequencePlayed.Contains(sequenceName))
        {
            sequencePlayed.Add(sequenceName);
        }
    }

    public static bool IsSequencePlayed(string sequenceName)
    {
        // PrintHashSet(sequencePlayed);
        return sequencePlayed.Contains(sequenceName);
    }

    //public static void PrintHashSet<T>(HashSet<T> hashSet)
    //{
    //    foreach (T element in hashSet)
    //    {
    //        Debug.Log(element);
    //    }

    //    Debug.Log("Print finished");
    //}

    public static void MarkConversationCompleted(string name)
    {
        if (!conversationsCompleted.Contains(name))
        {
            conversationsCompleted.Add(name);
        }
    }

    public static bool IsConversationCompleted(string name)
    {
        return conversationsCompleted.Contains(name);
    }

    public static void ResetGameState()
    {
        PlayerName = "Jack"; // Reset to default value
        Followers = 78; // Reset to default value

        UploadedImage = null;
        HappyImage = null;
        SadImage = null;

        // Clear and reset dictionaries and collections
        gameBools = new Dictionary<string, bool>
    {
        {"HasFlower", false},
        {"SisterReceiveFlower", false},
        {"MerchantShowHisName", false },
        {"RecognizeChildhoodSelf", false},
        {"Exercise2bCompleted", false},
        {exercise2Flag, false},
    };

        gameCounters = new Dictionary<string, HashSet<string>>
    {
        {exercise2Flag, new HashSet<string>{ } }
    };

        scenePlayerPositions.Clear();
        sceneNPCStates.Clear();
        sequencePlayed.Clear();
        conversationsCompleted.Clear();
        itemPicked.Clear();
    }


    //public static void SaveToPlayerPrefs()
    //{
    //    PlayerPrefs.SetString("PlayerPositions", JsonUtility.ToJson(new SerializableDictionary<string, Vector3>(scenePlayerPositions)));
    //    PlayerPrefs.SetString("NPCStates", JsonUtility.ToJson(new SerializableDictionary<string, Dictionary<string, NPCState>>(sceneNPCStates)));
    //    PlayerPrefs.SetString("SequencePlayed", JsonUtility.ToJson(sequencePlayed));
    //    // PlayerPrefs.SetString("LastSpawnPoints", JsonUtility.ToJson(new SerializableDictionary<string, string>(sceneLastSpawnPoints)));
    //    PlayerPrefs.SetString("PlayerName", PlayerName);
    //    PlayerPrefs.SetString("ConversationsCompleted", JsonUtility.ToJson(conversationsCompleted));
    //    PlayerPrefs.Save();
    //}

    //public static void LoadFromPlayerPrefs()
    //{
    //    if (PlayerPrefs.HasKey("PlayerPositions"))
    //    {
    //        scenePlayerPositions = JsonUtility.FromJson<SerializableDictionary<string, Vector3>>(PlayerPrefs.GetString("PlayerPositions")).ToDictionary();
    //    }
    //    if (PlayerPrefs.HasKey("NPCStates"))
    //    {
    //        sceneNPCStates = JsonUtility.FromJson<SerializableDictionary<string, Dictionary<string, NPCState>>>(PlayerPrefs.GetString("NPCStates")).ToDictionary();
    //    }
    //    if (PlayerPrefs.HasKey("SequencePlayed"))
    //    {
    //        sequencePlayed = JsonUtility.FromJson<HashSet<string>>(PlayerPrefs.GetString("SequencePlayed"));
    //    }
    //    if (PlayerPrefs.HasKey("PlayerName"))
    //    {
    //        PlayerName = PlayerPrefs.GetString("PlayerName");
    //    }
    //    if (PlayerPrefs.HasKey("ConversationsCompleted"))
    //    {
    //        conversationsCompleted = JsonUtility.FromJson<HashSet<string>>(PlayerPrefs.GetString("ConversationsCompleted"));
    //    }
    //    //if (PlayerPrefs.HasKey("LastSpawnPoints"))
    //    //{
    //    //    sceneLastSpawnPoints = JsonUtility.FromJson<SerializableDictionary<string, string>>(PlayerPrefs.GetString("LastSpawnPoints")).ToDictionary();
    //    //}
    //}

    //public static void ClearStates()
    //{
    //    scenePlayerPositions.Clear();
    //    sceneNPCStates.Clear();
    //    sequencePlayed.Clear();
    //    // sceneLastSpawnPoints.Clear();
    //}

    //public static HashSet<string> getSeqencesPlayed()
    //{
    //    return sequencePlayed;
    //}
}
