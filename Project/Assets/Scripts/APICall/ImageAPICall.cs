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
using System.IO;

public class ImageAPICall : MonoBehaviour
{
    private readonly string serverAddress = "comfyui-final-hbpmushhem.cn-shanghai.fcapp.run";
    //private readonly string serverAddress = "127.0.0.1:8188";
    private readonly string clientId = System.Guid.NewGuid().ToString();
    private WebSocket ws;
    private string promptText;
    //public Button button;
    private List<Button> buttons;
    private List<GenerateButton> generateButtons;
    private string fixedFacePrompt = "solo, ";
    private string fixedPrompt = "solo, pixelart, highly detailed, 4k, masterpiece, looking at viewer, eye contact, simple background, ";
    private string current_node = "";
    private Dictionary<string, List<byte[]>> output_images = new Dictionary<string, List<byte[]>>();

    public UnityEngine.UI.RawImage imageDisplay;
    private bool isGenerating = false;
    private GenerateButton b = null;

    private byte[] imageData;
    private readonly Queue<Action> mainThreadActions = new Queue<Action>();

    private string imageName;
    void Start()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("workflow");
        promptText = jsonFile.text;

        //button.onClick.AddListener(OnButtonClick);
        buttons = new List<Button>(FindObjectsOfType<Button>());
        if (buttons.Count == 0)
        {
            Debug.LogError("No buttons found in the scene.");
        }

        //generateButtons = new List<GenerateButton>(FindObjectsOfType<GenerateButton>());
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
        EventHandler.LoadPhotoEvent += OnLoadPhotoEvent;
    }

    private void OnDisable()
    {
        EventHandler.SetGenerateButtonEvent -= OnSetGenerateButtonEvent;
        EventHandler.SetServerRunningEvent -= OnSetServerRunningEvent;
        EventHandler.SetServerStopEvent -= OnSetServerStopEvent;
        EventHandler.LoadPhotoEvent -= OnLoadPhotoEvent;
    }

    private void OnLoadPhotoEvent()
    {
        StartCoroutine(UploadImage());
    }

    private IEnumerator UploadImage()
    {
        string imagePath = GameStateManager.GetPhotoPath();
        if (!File.Exists(imagePath))
        {
            Debug.LogError("Image file does not exist.");
            yield break;
        }
        byte[] imageBytes = File.ReadAllBytes(imagePath);

        string fileName = Path.GetFileName(imagePath);

        // Create a new form and add the image as binary data
        WWWForm form = new WWWForm();
        form.AddBinaryData("image", imageBytes, fileName);
        //form.AddField("overwrite", "true");

        // Send the POST request to the server
        using (UnityWebRequest www = UnityWebRequest.Post($"https://{serverAddress}/upload/image", form))
        {
            SetButtonsInteractable(false);
            yield return www.SendWebRequest();
            SetButtonsInteractable(true);
            EventHandler.CallLoadPhotoFinishEvent();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string responseText = www.downloadHandler.text;

                //Debug.Log("Response Code: " + www.responseCode);
                //Debug.Log("Response: " + responseText);

                ProcessUploadResponse(responseText);
            }
            else
            {
                Debug.LogError("Error: " + www.error);
            }
        }

    }

    private void ProcessUploadResponse(string responseText)
    {
        try
        {
            // Parse the JSON response
            JObject response = JObject.Parse(responseText);

            imageName = response["name"]?.ToString();
            //string subfolder = response["subfolder"]?.ToString();
            //string imageType = response["type"]?.ToString();

            //Debug.Log($"Uploaded image name: {imageName}");
            //Debug.Log($"Uploaded image subfolder: {subfolder}");
            //Debug.Log($"Uploaded image type: {imageType}");
        }
        catch (JsonReaderException ex)
        {
            Debug.LogError("Error parsing server response: " + ex.Message);
        }
    }

    //public void OnGenerateAllButtonClick()
    //{
    //    StartCoroutine(GenerateAllImages());
    //}

    //private IEnumerator GenerateAllImages()
    //{
    //    foreach (var generateButton in generateButtons)
    //    {
    //        generateButton.GenerateImage(); 
    //        yield return new WaitUntil(() => CheckIfServerIsAvailable());
    //        yield return new WaitForSeconds(1f);
    //    }
    //}

    public bool CheckIfServerIsAvailable()
    {
        return ws == null || !ws.IsAlive;
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

            ProcessPrompt(prompt);

            promptText = prompt.ToString();

            EventHandler.CallSetServerRunningEvent();


            StartCoroutine(QueuePrompt(promptText));
        }
        catch (JsonReaderException ex)
        {
            Debug.LogError("JSON parsing error: " + ex.Message);
        }
    }


    private void ProcessPrompt(JObject prompt)
    {
        // Seed update
        //facedetailer
        int randomSeed = UnityEngine.Random.Range(0, int.MaxValue);
        prompt["71"]["inputs"]["seed"] = randomSeed;
        //prompt["71"]["inputs"]["wildcard"] = b.type.ToString();

        randomSeed = UnityEngine.Random.Range(0, int.MaxValue);
        prompt["88"]["inputs"]["seed"] = randomSeed;

        //prompt for facedetailer
        prompt["96"]["inputs"]["positive"] = fixedFacePrompt + b.promptText;

        //prompt for pixel art
        prompt["91"]["inputs"]["positive"] = fixedPrompt + b.promptText;

        //image
        prompt["80"]["inputs"]["image"] = imageName;

        ProcessLora(prompt);
    }

    private void ProcessLora(JObject prompt)
    {
        var pixelLoraLoader = prompt["82"]["inputs"];
        var realLoraLoader = prompt["111"]["inputs"];

        SetLoraWeights(pixelLoraLoader, 1, 2, 2);
        SetLoraWeights(realLoraLoader, 1.5f, 2, 1.5f);
    }

    private void SetLoraWeights(JToken token, float val, float val1, float val2)
    {
        for (int i = 1; i <= 6; i++)
        {
            string key = $"lora_wt_{i}";
            if (i != b.loraWeightIndex)
            {

                token[key] = 0;
            }
            else if (i == 3)
            {
                token[key] = val1;
            }
            else if (i == 4)
            {
                token[key] = val2;
            }
            else
            {
                token[key] = val;
            }
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

        using (UnityWebRequest www = UnityWebRequest.Put($"https://{serverAddress}/prompt", postData))
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
                InitializeWebSocket();
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
                mainThreadActions.Enqueue(() => EventHandler.CallSetServerStopEvent($"Error processing WebSocket message: {ex.Message}, try upload the photo again!"));
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
            //Debug.Log(data);
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
                        //Debug.Log(current_node);
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
                EventHandler.CallSetDisplayingExpressionEvent("Displaying: ", b.type);
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
        if (buttons == null)
        {
            return;
        }
        foreach (var button in buttons)
        {
            button.interactable = interactable;
        }
    }

    private void OnSetServerRunningEvent(string ex = "")
    {
        SetButtonsInteractable(false);
        isGenerating = true;
    }

    private void OnSetServerStopEvent(string ex = "")
    {
        SetButtonsInteractable(true);
        EventHandler.CallImageSavedEvent();
        isGenerating = false;
    }

    public Dictionary<string, List<byte[]>> GetOutputImages()
    {
        return output_images;
    }

    public bool GetIsGenerating()
    {
        return isGenerating;
    }
}