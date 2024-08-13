using UnityEngine;

public class CharacterBodyResetter : MonoBehaviour
{
    [SerializeField] private CharacterBody characterBody;
    [SerializeField] private CharacterBody defaultCharacterBody;

    private void Start()
    {
        ResetCharacterBody();

        EventHandler.CallUpdateBodyPartEvent();
        EventHandler.CallUpdateColorEvent();
    }
    // Method to reset the CharacterBody ScriptableObject to default settings
    public void ResetCharacterBody()
    {
        if (characterBody == null)
        {
            Debug.LogError("CharacterBody is not assigned.");
            return;
        }

        if (defaultCharacterBody == null)
        {
            Debug.LogError("DefaultCharacterBody is not assigned.");
            return;
        }

        // Reset each body part to the corresponding default part
        for (int i = 0; i < characterBody.characterBodyParts.Length; i++)
        {
            if (i < defaultCharacterBody.characterBodyParts.Length)
            {
                characterBody.characterBodyParts[i].bodyPart = defaultCharacterBody.characterBodyParts[i].bodyPart;
            }
        }

        //Debug.Log("CharacterBody has been reset to default settings.");
    }
}