using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyItemDrop : MonoBehaviour
{
    [SerializeField] private string relatedBool;
    // Start is called before the first frame update
    void Start()
    {
        if (!GameStateManager.GetBool(relatedBool))
        {
            Object.Destroy(gameObject);
        }
    }
}
