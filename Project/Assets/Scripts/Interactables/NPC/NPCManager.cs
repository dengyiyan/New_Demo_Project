using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCManager : MonoBehaviour
{

    private Dictionary<string, GameObject> npcs = new Dictionary<string, GameObject>();

    private void Start()
    {
        RegisterNPCs();
    }
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += RegisterNPCs;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= RegisterNPCs;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
    }

    private void OnBeforeSceneUnloadEvent()
    {
        npcs.Clear();
    }

    private void RegisterNPCs()
    {
        AnimationMovement[] npcList = FindObjectsOfType<AnimationMovement>();
        // Debug.Log(npcList.Length);
        if (npcList.Length > 0) {
            foreach (var npc in npcList)
            {
                npcs[npc.name] = npc.gameObject;
                //Debug.Log($"Get npc: {npc.name}");
            }
        }

        EventHandler.CallRegisterNPCEvent(npcs);
    }
}
