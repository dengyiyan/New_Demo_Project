using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using WebSocketSharp;
using System;
using Newtonsoft.Json;

public class ImageAPICall : MonoBehaviour
{
    private string serverAddress = "127.0.0.1:8188";
    private string clientId = System.Guid.NewGuid().ToString();
    private WebSocket ws;
    private string promptText;
    //public Button button;
    private List<Button> buttons;
    private string fixedPrompt = "solo, upper body, c0ne, pixel art, simple background";
    private string current_node = "";
    private Dictionary<string, List<byte[]>> output_images = new Dictionary<string, List<byte[]>>();

    public UnityEngine.UI.RawImage imageDisplay;
    private GenerateButton b = null;

    private byte[] imageData;
    private readonly Queue<Action> mainThreadActions = new Queue<Action>();
    void Start()
    {
        // Load and modify the JSON prompt
        TextAsset jsonFile = Resources.Load<TextAsset>("test_workflow");
        promptText = jsonFile.text;

        //button.onClick.AddListener(OnButtonClick);
        buttons = new List<Button>(FindObjectsOfType<Button>());
    }

    void Update()
    {
        // Execute all actions queued for the main thread
        while (mainThreadActions.Count > 0)
        {
            var action = mainThreadActions.Dequeue();
            action.Invoke();
        }
    }

    private void OnEnable()
    {
        EventHandler.SetGenerateButtonEvent += OnSetGenerateButtonEvent;
        EventHandler.SetServerRunningEvent += OnSetServerRunningEvent;
        EventHandler.SetServerStopEvent += OnSetServerStopEvent;
    }

    private void OnDisable()
    {
        EventHandler.SetGenerateButtonEvent -= OnSetGenerateButtonEvent;
        EventHandler.SetServerRunningEvent -= OnSetServerRunningEvent;
        EventHandler.SetServerStopEvent -= OnSetServerStopEvent;
    }

    void OnSetGenerateButtonEvent(GenerateButton inputButton)
    {
        b = inputButton;
        OnButtonClick();
    }

    public void OnButtonClick()
    {

        if (string.IsNullOrEmpty(promptText))
        {
            Debug.LogError("promptText is empty or null.");
            return;
        }

        try
        {
            JObject prompt = JObject.Parse(promptText);
            prompt["1"]["inputs"]["positive"] = fixedPrompt + b.promptText;
            if (!string.IsNullOrEmpty(GameStateManager.GetPhotoPath()))
                prompt["6"]["inputs"]["image"] = GameStateManager.GetPhotoPath();
            int randomSeed = UnityEngine.Random.Range(0, int.MaxValue);
            prompt["3"]["inputs"]["seed"] = randomSeed;
            promptText = prompt.ToString();

            EventHandler.CallSetServerRunningEvent();


            StartCoroutine(QueuePrompt(promptText));
        }
        catch (JsonReaderException ex)
        {
            Debug.LogError("JSON parsing error: " + ex.Message);
        }
    }

    private IEnumerator QueuePrompt(string prompt)
    {
        var promptData = $@"
    {{
        ""client_id"": ""{clientId}"",
        ""prompt"": {prompt}
    }}";

        byte[] postData = Encoding.UTF8.GetBytes(promptData);

        using (UnityWebRequest www = UnityWebRequest.Put($"http://{serverAddress}/prompt", postData))
        {
            www.method = UnityWebRequest.kHttpVerbPOST;
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error sending prompt: {www.error}");
                EventHandler.CallSetServerStopEvent($"Error sending prompt: {www.error}");
            }
            else
            {
                Debug.Log("Prompt successfully sent to server.");
                InitializeWebSocket(); // Initialize WebSocket only after HTTP success
            }
        }
    }

    private void InitializeWebSocket()
    {
        ws = new WebSocket($"ws://{serverAddress}/ws?clientId={clientId}");

        ws.OnMessage += (sender, e) =>
        {
            try
            {
                if (e.IsText)
                {
                    HandleTextMessage(e.Data);
                }
                else if (e.IsBinary)
                {
                    HandleBinaryMessage(e.RawData);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error processing WebSocket message: " + ex.Message);
                mainThreadActions.Enqueue(() => EventHandler.CallSetServerStopEvent($"Error processing WebSocket message: {ex.Message}"));
            }
        };

        ws.OnError += (sender, e) =>
        {
            Debug.LogError("WebSocket Error: " + e.Message);
            mainThreadActions.Enqueue(() => EventHandler.CallSetServerStopEvent($"WebSocket Error: {e.Message}"));
        };

        ws.OnClose += (sender, e) =>
        {
            Debug.Log("WebSocket closed with reason: " + e.Reason);
            mainThreadActions.Enqueue(() => EventHandler.CallSetServerStopEvent($"WebSocket closed with reason: {e.Reason}"));
        };

        ws.Connect();
    }

    private void HandleTextMessage(string message)
    {
        try
        {
            var data = JObject.Parse(message);
            Debug.Log(data);
            if (data["type"].ToString() == "executing")
            {
                var executingData = data["data"];
                string promptId = executingData["prompt_id"]?.ToString();
                if (promptId != null)
                {
                    if (executingData["node"] == null)
                    {
                        Debug.Log("Execution done.");

                        mainThreadActions.Enqueue(() => EventHandler.CallSetServerStopEvent("Execution done."));
                        return;
                    }
                    else
                    {
                        current_node = executingData["node"].ToString();
                        Debug.Log(current_node);
                    }
                }
            }
        }
        catch (JsonReaderException ex)
        {
            Debug.LogError("Error parsing JSON message: " + ex.Message);
            mainThreadActions.Enqueue(() => EventHandler.CallSetServerStopEvent($"Error parsing JSON message: {ex.Message}"));
        }
    }

    private void HandleBinaryMessage(byte[] rawData)
    {
        if (current_node == "save_image_websocket_node")
        {
            // Extract the image data (skipping the first 8 bytes, which may be metadata or a header)
            imageData = new byte[rawData.Length - 8];
            Array.Copy(rawData, 8, imageData, 0, imageData.Length);

            if (!output_images.ContainsKey(current_node))
            {
                output_images[current_node] = new List<byte[]>();
            }

            //output_images[current_node].Add(imageData);

            Debug.Log("Image data received and stored.");
            mainThreadActions.Enqueue(() => {
                DisplayImage(imageData);
                EventHandler.CallSetServerStopEvent();
                EventHandler.CallImageSavedEvent();
            });
        }
    }

    private void DisplayImage(byte[] imageData)
    {
        Texture2D texture = new Texture2D(2, 2); // Create a new texture
        if (texture.LoadImage(imageData)) // Load the image data into the texture
        {
            imageDisplay.texture = texture; // Set the texture to the RawImage component
            GameStateManager.SaveGeneratedImage(b.type, texture);
        }
        else
        {
            EventHandler.CallSetServerStopEvent("Failed to load image data into texture.");
            Debug.LogError("Failed to load image data into texture.");
        }
    }
    void OnDestroy()
    {
        CloseWebSocket();
    }

    void OnApplicationQuit()
    {
        CloseWebSocket();
    }

    private void CloseWebSocket()
    {
        if (ws != null)
        {
            if (ws.IsAlive)
            {
                ws.Close();
            }
            ws = null;
        }
    }

    private void SetButtonsInteractable(bool interactable)
    {
        foreach (var button in buttons)
        {
            button.interactable = interactable;
        }
    }

    private void OnSetServerRunningEvent(string ex="")
    {
        SetButtonsInteractable(false);
    }

    private void OnSetServerStopEvent(string ex = "")
    {
        SetButtonsInteractable(true);
    }

    public Dictionary<string, List<byte[]>> GetOutputImages()
    {
        return output_images;
    }
}