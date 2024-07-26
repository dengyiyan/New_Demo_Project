using UnityEngine;

public class SetRenderCamera : MonoBehaviour
{
    private string cameraTag = "MainCamera"; // Tag of the camera in the persistent scene

    void Start()
    {
        // Get the Canvas component attached to this GameObject
        Canvas canvas = GetComponent<Canvas>();

        if (canvas == null)
        {
            Debug.LogError("Canvas component not found!");
            return;
        }

        // Find the camera by tag
        Camera persistentCamera = GameObject.FindGameObjectWithTag(cameraTag)?.GetComponent<Camera>();

        if (persistentCamera != null)
        {
            // Set the render camera of the Canvas
            canvas.worldCamera = persistentCamera;
            Debug.Log("Render camera set for the Canvas.");
        }
        else
        {
            Debug.LogError("Persistent camera not found!");
        }
    }
}
