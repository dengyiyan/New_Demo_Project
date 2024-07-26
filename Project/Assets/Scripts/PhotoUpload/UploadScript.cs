using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;
using System.IO;
using UnityEngine.UI;

public class UploadScript : MonoBehaviour
{
    public Image image;
    public InputField nameInput;
    public Button submitButton;
    // Start is called before the first frame update
    void Start()
    {
        submitButton.onClick.AddListener(saveName);
        FileBrowser.SetFilters(false, new FileBrowser.Filter("Images", ".jpg", ".png"));

        //FileBrowser.AddQuickLink("Users", "C:\\Users", null);

        // !!! Uncomment any of the examples below to show the file browser !!!

        // Example 1: Show a save file dialog using callback approach
        // onSuccess event: not registered (which means this dialog is pretty useless)
        // onCancel event: not registered
        // Save file/folder: file, Allow multiple selection: false
        // Initial path: "C:\", Initial filename: "Screenshot.png"
        // Title: "Save As", Submit button text: "Save"
        // FileBrowser.ShowSaveDialog( null, null, FileBrowser.PickMode.Files, false, "C:\\", "Screenshot.png", "Save As", "Save" );

        // Example 2: Show a select folder dialog using callback approach
        // onSuccess event: print the selected folder's path
        // onCancel event: print "Canceled"
        // Load file/folder: folder, Allow multiple selection: false
        // Initial path: default (Documents), Initial filename: empty
        // Title: "Select Folder", Submit button text: "Select"
        // FileBrowser.ShowLoadDialog( ( paths ) => { Debug.Log( "Selected: " + paths[0] ); },
        //						   () => { Debug.Log( "Canceled" ); },
        //						   FileBrowser.PickMode.Folders, false, null, null, "Select Folder", "Select" );

        // Example 3: Show a select file dialog using coroutine approach
        // StartCoroutine(ShowLoadDialogCoroutine());
    }

    private void saveName()
    {
        if (!string.IsNullOrEmpty(nameInput.text))
        {
            GameStateManager.PlayerName = nameInput.text;
        }
        
    }

    public void onClick()
    {
        StartCoroutine(ShowLoadDialogCoroutine());
    }
    IEnumerator ShowLoadDialogCoroutine()
    {
        // Show a load file dialog and wait for a response from user
        // Load file/folder: file, Allow multiple selection: true
        // Initial path: default (Documents), Initial filename: empty
        // Title: "Load File", Submit button text: "Load"
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, null, null, "Select Files", "Load");

        // Dialog is closed
        // Print whether the user has selected some files or cancelled the operation (FileBrowser.Success)
        Debug.Log(FileBrowser.Success);

        if (FileBrowser.Success)
            OnFilesSelected(FileBrowser.Result); // FileBrowser.Result is null, if FileBrowser.Success is false
    }

    void OnFilesSelected(string[] filePaths)
    {
        // Print paths of the selected files
        for (int i = 0; i < filePaths.Length; i++)
            Debug.Log(filePaths[i]);

        // Get the file path of the first selected file
        string filePath = filePaths[0];

        // Read the bytes of the first file via FileBrowserHelpers
        // Contrary to File.ReadAllBytes, this function works on Android 10+, as well
        byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(filePath);

        DisplayImageFromBytes(bytes);

        // Or, copy the first file to persistentDataPath
        // string destinationPath = Path.Combine(Application.persistentDataPath, FileBrowserHelpers.GetFilename(filePath));
        // FileBrowserHelpers.CopyFile(filePath, destinationPath);
    }

    void DisplayImageFromBytes(byte[] imageBytes)
    {
        Texture2D texture = new Texture2D(2, 2);
        if (ImageConversion.LoadImage(texture, imageBytes))
        {
            Debug.Log("Loaded image successfully!");
            Sprite newSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            image.sprite = newSprite;
        }
        else
        {
            Debug.LogError("Failed to load image!");
        }
    }
}
