using UnityEngine;
using UnityEngine.UI;

public class DisplayingHeader : MonoBehaviour
{
    private Text header;

    private readonly string prefix = "Displaying: ";
    // Start is called before the first frame update
    void Start()
    {
        header = GetComponent<Text>();
        header.text = prefix + "None";

    }

    private void OnEnable()
    {
        EventHandler.SetDisplayingExpressionEvent += SetHeader;
    }

    private void OnDisable()
    {
        EventHandler.SetDisplayingExpressionEvent -= SetHeader;
    }

    private void SetHeader(string str, ImageType type)
    {
        if (header)
            header.text = $"{str}{type}";
    }

}
