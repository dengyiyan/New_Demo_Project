using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameStateManager
{
    public static string PlayerName { get; set; } = "Jack";

    private static Dictionary<string, bool> gameBools = new Dictionary<string, bool>
    {
        {"HasFlower", false},
        {"SisterReceiveFlower", false}
    };
    private static Dictionary<string, Vector3> scenePlayerPositions = new Dictionary<string, Vector3>();
    private static Dictionary<string, Dictionary<string, NPCState>> sceneNPCStates = new Dictionary<string, Dictionary<string, NPCState>>();
    // private static Dictionary<string, string> sceneLastSpawnPoints = new Dictionary<string, string>();
    private static HashSet<string> sequencePlayed = new HashSet<string>(); 
    private static HashSet<string> conversationsCompleted = new HashSet<string>(); // Track completed conversations
    private static HashSet<string> itemPicked = new HashSet<string>();

    // private const string DefaultSpawnPoint = "default";
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
        Debug.Log("destroy triggered!");
        foreach (var itemName in itemPicked)
        {
            var obj = GameObject.Find(itemName);
            if (obj != null)
            {
                UnityEngine.Object.Destroy(obj);

                Debug.Log("object destroyed!");
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

    public static Vector3 GetPlayerPosition(string sceneName)
    {
        if (scenePlayerPositions.ContainsKey(sceneName))
        {
            return scenePlayerPositions[sceneName];
        }
        return Vector3.zero; // Default position if not found
    }

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

    public static void SaveNPCState(string sceneName, string npcName, Vector3 position)
    {
        if (!sceneNPCStates.ContainsKey(sceneName))
        {
            sceneNPCStates[sceneName] = new Dictionary<string, NPCState>();
        }
        sceneNPCStates[sceneName][npcName] = new NPCState
        {
            npcName = npcName,
            position = position
        };
    }

    public static Dictionary<string, NPCState> GetNPCStates(string sceneName)
    {
        if (sceneNPCStates.ContainsKey(sceneName))
        {
            return sceneNPCStates[sceneName];
        }
        return new Dictionary<string, NPCState>(); // Return empty dictionary if not found
    }

    public static void MarkSequencePlayed(string sequenceName)
    {
        if (!sequencePlayed.Contains(sequenceName))
        {
            sequencePlayed.Add(sequenceName);
        }
    }

    public static bool IsSequencePlayed(string sequenceName)
    {
        // PrintHashSet(sequencePlayed);
        return sequencePlayed.Contains(sequenceName);
    }

    public static void PrintHashSet<T>(HashSet<T> hashSet)
    {
        foreach (T element in hashSet)
        {
            Debug.Log(element);
        }

        Debug.Log("Print finished");
    }

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


    public static void SaveToPlayerPrefs()
    {
        PlayerPrefs.SetString("PlayerPositions", JsonUtility.ToJson(new SerializableDictionary<string, Vector3>(scenePlayerPositions)));
        PlayerPrefs.SetString("NPCStates", JsonUtility.ToJson(new SerializableDictionary<string, Dictionary<string, NPCState>>(sceneNPCStates)));
        PlayerPrefs.SetString("SequencePlayed", JsonUtility.ToJson(sequencePlayed));
        // PlayerPrefs.SetString("LastSpawnPoints", JsonUtility.ToJson(new SerializableDictionary<string, string>(sceneLastSpawnPoints)));
        PlayerPrefs.SetString("PlayerName", PlayerName);
        PlayerPrefs.SetString("ConversationsCompleted", JsonUtility.ToJson(conversationsCompleted));
        PlayerPrefs.Save();
    }

    public static void LoadFromPlayerPrefs()
    {
        if (PlayerPrefs.HasKey("PlayerPositions"))
        {
            scenePlayerPositions = JsonUtility.FromJson<SerializableDictionary<string, Vector3>>(PlayerPrefs.GetString("PlayerPositions")).ToDictionary();
        }
        if (PlayerPrefs.HasKey("NPCStates"))
        {
            sceneNPCStates = JsonUtility.FromJson<SerializableDictionary<string, Dictionary<string, NPCState>>>(PlayerPrefs.GetString("NPCStates")).ToDictionary();
        }
        if (PlayerPrefs.HasKey("SequencePlayed"))
        {
            sequencePlayed = JsonUtility.FromJson<HashSet<string>>(PlayerPrefs.GetString("SequencePlayed"));
        }
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            PlayerName = PlayerPrefs.GetString("PlayerName");
        }
        if (PlayerPrefs.HasKey("ConversationsCompleted"))
        {
            conversationsCompleted = JsonUtility.FromJson<HashSet<string>>(PlayerPrefs.GetString("ConversationsCompleted"));
        }
        //if (PlayerPrefs.HasKey("LastSpawnPoints"))
        //{
        //    sceneLastSpawnPoints = JsonUtility.FromJson<SerializableDictionary<string, string>>(PlayerPrefs.GetString("LastSpawnPoints")).ToDictionary();
        //}
    }

    public static void ClearStates()
    {
        scenePlayerPositions.Clear();
        sceneNPCStates.Clear();
        sequencePlayed.Clear();
        // sceneLastSpawnPoints.Clear();
    }

    public static HashSet<string> getSeqencesPlayed()
    {
        return sequencePlayed;
    }
}
