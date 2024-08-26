using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetGameStateManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameStateManager.ResetGameState();
    }
}
