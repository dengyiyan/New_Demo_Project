
using UnityEngine;
using UnityEngine.UI;

public class Instruction : MonoBehaviour
{
    private Text instruction;

    private readonly string regularInstruction = "Click generate to generate your photo with different emotions," +
                " or click show to display the last generated photo of that expression!";
    private void Start()
    {
        instruction = GetComponent<Text>();
        if (GameStateManager.GetUploadedImage() == null)
            instruction.text = "First upload your photo then click generate. " +
                "You can access these generated images later in the game.";
        else
            instruction.text = regularInstruction;

    }

    private void OnEnable()
    {
        EventHandler.LoadPhotoEvent += OnPhotoUpload;
        EventHandler.SetServerRunningEvent += OnServerRunning;
        EventHandler.SetServerStopEvent += OnServerStop;
    }

    private void OnDisable()
    {
        EventHandler.LoadPhotoEvent -= OnPhotoUpload;
        EventHandler.SetServerRunningEvent -= OnServerRunning;
        EventHandler.SetServerStopEvent -= OnServerStop;
    }

    private void OnPhotoUpload()
    {
        if (instruction)
            instruction.text = regularInstruction;
    }

    private void OnServerRunning(string ex = "")
    {
        instruction.text = "Generating... Please be patient as this process might take a while";
    }

    private void OnServerStop(string ex = "")
    {
        if (string.IsNullOrEmpty(ex))
            instruction.text = regularInstruction;

        else
            instruction.text = ex;
    }


}
