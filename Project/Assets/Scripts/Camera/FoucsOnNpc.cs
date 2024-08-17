using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoucsOnNpc : MonoBehaviour
{
    [SerializeField] private string npcName = "Ian";
    [SerializeField] private GameObject player;

    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private CinemachineTargetGroup targetGroup;
    [SerializeField] private Transform npcTransform;
    private Dictionary<string, GameObject> npcs = new Dictionary<string, GameObject>();

    private void OnEnable()
    {
        EventHandler.RegisterNPCEvent += OnRegisterNPCEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnload;
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoad;
    }

    private void OnDisable()
    {
        EventHandler.RegisterNPCEvent -= OnRegisterNPCEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnload;
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoad;
    }

    private void Start()
    {
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        targetGroup = FindObjectOfType<CinemachineTargetGroup>();

        if (virtualCamera == null || targetGroup == null)
        {
            Debug.LogError("Virtual Camera or Target Group not found in the scene.");
            return;
        }

        player = GameObject.FindGameObjectWithTag("Player");

        if (npcs.TryGetValue(npcName, out GameObject npc))
        {
            npcTransform = npc.transform;
        }
        else
        {
            Debug.LogError($"{npcName} not found in the scene.");
        }

        FocusOnNPC();
    }

    private void FocusOnNPC()
    {
        if (npcTransform != null && targetGroup != null)
        {
            // Disable the player
            if (player != null)
            {
                player.SetActive(false);
            }

            // Add Ian to the target group and remove the player
            targetGroup.m_Targets = new CinemachineTargetGroup.Target[]
            {
                new CinemachineTargetGroup.Target { target = npcTransform, weight = 1f, radius = 6f }
            };
        }

        EventHandler.CallSetMainFocusEvent(npcTransform);
    }

    private void OnBeforeSceneUnload()
    {
        // Re-enable the player and focus back on the player when the scene is unloading
        if (player != null)
        {
            player.SetActive(true);
        }

        if (targetGroup != null)
        {
            targetGroup.m_Targets = new CinemachineTargetGroup.Target[]
            {
                new CinemachineTargetGroup.Target { target = player.transform, weight = 1f, radius = 6f }
            };
        }
    }

    private void OnRegisterNPCEvent(Dictionary<string, GameObject> in_npcs)
    {
        npcs = in_npcs;
    }

    private void OnAfterSceneLoad()
    {
        // Re-focus on the NPC (Ian) in the new scene if needed
        FocusOnNPC();
    }
}
