using UnityEngine.UI;
using UnityEngine;

public class DisableButton : MonoBehaviour
{
    private GameObject canvas;
    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.FindGameObjectWithTag("ButtonCanvas");

        canvas.SetActive(false);
    }

    private void OnDestroy()
    {
        canvas.SetActive(true);
    }
}
