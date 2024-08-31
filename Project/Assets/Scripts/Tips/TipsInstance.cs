using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipsInstance : MonoBehaviour
{
    [TextArea(3, 10)]
    public string tips;
    // Start is called before the first frame update
    void Start()
    {
        EventHandler.CallRegisterTipsEvent(tips);
    }

    private void OnDestroy()
    {
        EventHandler.CallUnregisterTipsEvent();
    }
}
