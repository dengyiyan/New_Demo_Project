using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BodyPartsSelection : MonoBehaviour
{
    [SerializeField] private CharacterBody characterBody;
    // Body Part Selections
    [SerializeField] private BodyPartSelection[] bodyPartSelections;

    //List<Color> skinColors = new List<Color>();
    // Start is called before the first frame update
    private void Start()
    {
        // setSkinColors();
        setHairColors();
        SetPantsColors();

        for (int i = 0; i < bodyPartSelections.Length; i++)
        {
            GetCurrentBodyParts(i);
            checkButtonEnabled(i);
        }
    }


    private void setSkinColors()
    {
        bodyPartSelections[0].colorOptions = new Color[]
        {
            new Color(1f, 1f, 1f),
            new Color(0.97f, 0.81f, 0.67f), // Light skin
            new Color(0.85f, 0.65f, 0.48f), // Light tan
            new Color(0.71f, 0.53f, 0.35f), // Moderate brown
            new Color(0.61f, 0.46f, 0.33f), // Darker brown
            new Color(0.50f, 0.36f, 0.25f), // Dark brown
            new Color(0.33f, 0.25f, 0.20f), // Very dark brown
        };
    }

    private void setHairColors()
    {
        bodyPartSelections[1].colorOptions = new Color[]
        {
            new Color(1.0f, 1.0f, 1.0f), // White
            new Color(0.0f, 0.0f, 0.0f), // Black
            new Color(0.34f, 0.16f, 0.14f), // Dark Brown
            new Color(0.5f, 0.25f, 0.2f), // Medium Brown
            new Color(0.68f, 0.31f, 0.25f), // Light Brown
            new Color(1.0f, 0.98f, 0.82f), // Blond
            new Color(0.82f, 0.7f, 0.55f), // Dark Blond
            new Color(0.55f, 0.15f, 0.14f), // Auburn
            new Color(0.20f, 1.0f, 0.20f), // Neon Green
            new Color(1.0f, 0.0f, 0.0f), // Red
            new Color(0.95f, 0.52f, 0.0f), // Ginger
            new Color(0.5f, 0.5f, 0.5f), // Gray
            new Color(0.75f, 0.75f, 0.75f), // Silver
            new Color(0.5f, 0.5f, 0.55f), // Salt and Pepper
            new Color(0.76f, 0.75f, 0.71f), // Ash Blond
            new Color(1.0f, 0.7f, 0.8f), // Soft Pink
            new Color(1.0f, 0.67f, 0.5f), // Strawberry Blond
            new Color(0.13f, 0.55f, 0.13f), // Dark Forest Green
            new Color(0.5f, 0.0f, 0.5f), // Purple
            new Color(1.0f, 0.2f, 0.6f) // Vibrant Pink
           };
    }

    private void SetPantsColors()
    {
        bodyPartSelections[3].colorOptions = new Color[]
    {
        new Color(0.0f, 0.0f, 0.0f), // Black
        new Color(0.15f, 0.15f, 0.15f), // Charcoal Grey
        new Color(0.25f, 0.25f, 0.25f), // Medium Grey
        new Color(0.5f, 0.5f, 0.5f), // Light Grey
        new Color(0.05f, 0.1f, 0.15f), // Navy Blue
        new Color(0.4f, 0.4f, 0.2f), // Khaki
        new Color(0.35f, 0.25f, 0.2f), // Dark Brown
        new Color(0.5f, 0.35f, 0.25f), // Medium Brown
        new Color(0.65f, 0.45f, 0.3f), // Light Brown
        new Color(0.3f, 0.6f, 0.2f), // Olive Green
        new Color(0.7f, 0.5f, 0.3f), // Beige
        new Color(0.3f, 0.1f, 0.4f), // Deep Purple
        new Color(0.6f, 0.2f, 0.2f), // Maroon
        new Color(0.75f, 0.4f, 0.15f), // Rust
        new Color(0.0f, 0.0f, 0.5f), // Royal Blue
        new Color(0.25f, 0.0f, 0.5f), // Deep Plum
        new Color(0.2f, 0.3f, 0.2f), // Forest Green
        new Color(0.85f, 0.65f, 0.45f) // Camel
    };
    }

    private void getColor(int index)
    {
        SO_BodyPart selectedBodyPart = characterBody.characterBodyParts[index].bodyPart;
        Color currentColor = selectedBodyPart.bodyPartColor;
        int currentColorIndex = System.Array.FindIndex(bodyPartSelections[index].colorOptions, color => color.Equals(currentColor));
        if (currentColorIndex == -1)
        {
            Debug.Log("Error! Color does not exist!");
        }
        else
        {
            bodyPartSelections[index].colorIndex = currentColorIndex;
            UpdateColor(index);
        }
    }

    private void UpdateColor(int index)
    {
        BodyPartSelection selection = bodyPartSelections[index];
        GameObject obj = GameObject.Find(selection.bodyPartName);
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        Color updateColor = selection.colorOptions[selection.colorIndex];
        selection.bodyPartOptions[selection.bodyPartCurrentIndex].bodyPartColor = updateColor;
        spriteRenderer.color = updateColor;
        selection.bodyPartColorTextComponent.text = "Color " + (selection.colorIndex + 1).ToString();

        EventHandler.CallUpdateColorEvent();
    }

    public void nextColor(int index)
    {
        int colorIndex = bodyPartSelections[index].colorIndex;
        if (bodyPartSelections[index].colorIndex < bodyPartSelections[index].colorOptions.Length - 1)
        {
            bodyPartSelections[index].colorIndex++;
        }
        else
        {
            bodyPartSelections[index].colorIndex = 0;
        }

        UpdateColor(index);
    }

    public void prevColor(int index)
    {
        int colorIndex = bodyPartSelections[index].colorIndex;
        if (bodyPartSelections[index].colorIndex > 0)
        {
            bodyPartSelections[index].colorIndex--;
        }
        else
        {
            bodyPartSelections[index].colorIndex = bodyPartSelections[index].colorOptions.Length - 1;
        }

        UpdateColor(index);
    }

    private void GetCurrentBodyParts(int index)
    {
        BodyPart selectedBodyPart = characterBody.characterBodyParts[index];
        int num = selectedBodyPart.bodyPart.bodyPartAnimationID;
        bodyPartSelections[index].bodyPartNameTextComponent.text = "Type " + (num + 1).ToString();
        bodyPartSelections[index].bodyPartCurrentIndex = num;
        getColor(index);
    }

    private void checkButtonEnabled(int index)
    {
        BodyPart selectedBodyPart = characterBody.characterBodyParts[index];

        if (bodyPartSelections[index].bodyPartOptions.Length < 2)
        {
            GameObject leftButtonObj = GameObject.Find(selectedBodyPart.bodyPartName + " Left Button");
            GameObject rightButtonObj = GameObject.Find(selectedBodyPart.bodyPartName + " Right Button");
            Button leftButton = leftButtonObj.GetComponent<Button>();
            Button rightButton = rightButtonObj.GetComponent<Button>();

            leftButton.enabled = false;
            rightButton.enabled = false;

            setGreyColor(leftButton);
            setGreyColor(rightButton);
        }

        if (bodyPartSelections[index].colorOptions.Length < 2)
        {
            GameObject prevColorButtonObj = GameObject.Find(selectedBodyPart.bodyPartName + " Previous Color");
            GameObject nextColorButtonObj = GameObject.Find(selectedBodyPart.bodyPartName + " Next Color");

            Button prevColorButton = prevColorButtonObj.GetComponent<Button>();
            Button nextColorButton = nextColorButtonObj.GetComponent<Button>();

            prevColorButton.enabled = false;
            nextColorButton.enabled = false;

            setGreyColor(prevColorButton);
            setGreyColor(nextColorButton);

        }
    }

    private void setGreyColor(Button button)
    {
        button.image.color = new Color(0.7f, 0.7f, 0.7f);
    }
    private void UpdateCurrentPart(int index)
    {
        BodyPartSelection selection = bodyPartSelections[index];
        SO_BodyPart newBodyPart = selection.bodyPartOptions[selection.bodyPartCurrentIndex];
        selection.bodyPartNameTextComponent.text = "Type " + (selection.bodyPartCurrentIndex + 1).ToString();
        characterBody.characterBodyParts[index].bodyPart = newBodyPart;

        EventHandler.CallUpdateBodyPartEvent();
    }

    private bool ValidateIndexValue(int index)
    {
        if (index > bodyPartSelections.Length || index < 0)
        {
            Debug.Log("Index value does not match existing body parts!");
            return false;
        }
        return true;
    }


    public void NextBodyPart(int index)
    {
        if (ValidateIndexValue(index))
        {
            BodyPartSelection currentBodyPart = bodyPartSelections[index];
            if (currentBodyPart.bodyPartCurrentIndex < currentBodyPart.bodyPartOptions.Length - 1)
            {
                currentBodyPart.bodyPartCurrentIndex++;
            }
            else
            {
                currentBodyPart.bodyPartCurrentIndex = 0;
            }

            UpdateCurrentPart(index);
        }
    }


    public void PrevBodyPart(int index) {  
        if (ValidateIndexValue(index))
        {
            BodyPartSelection currentBodyPart = bodyPartSelections[index];
            if (currentBodyPart.bodyPartCurrentIndex > 0)
            {
                currentBodyPart.bodyPartCurrentIndex--;
            }
            else
            {
                currentBodyPart.bodyPartCurrentIndex = currentBodyPart.bodyPartOptions.Length - 1;
            }

            UpdateCurrentPart(index);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}


[System.Serializable]
public class BodyPartSelection
{
    public string bodyPartName;
    public SO_BodyPart[] bodyPartOptions;
    public Color[] colorOptions = new Color[] { new Color(1f, 1f, 1f) };
    public Text bodyPartNameTextComponent;
    public Text bodyPartColorTextComponent;
    public int bodyPartCurrentIndex;
    public int colorIndex;
}
