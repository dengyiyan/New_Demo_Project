using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SceneNameAttribute))]
public class SceneNameDrawer : PropertyDrawer
{
    private readonly string[] scenePathSplit = { "/", ".unity" };
    private Dictionary<string, int> sceneIndices = new Dictionary<string, int>();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (EditorBuildSettings.scenes.Length == 0)
        {
            EditorGUI.LabelField(position, label, new GUIContent("No scenes in build settings"));
            return;
        }

        if (!sceneIndices.ContainsKey(property.propertyPath))
        {
            GetSceneNameArray(property);
        }

        int sceneIndex = sceneIndices[property.propertyPath];
        int oldIndex = sceneIndex;
        sceneIndex = EditorGUI.Popup(position, label, sceneIndex, GetSceneNames());

        if (oldIndex != sceneIndex)
        {
            property.stringValue = sceneIndex == 0 ? string.Empty : GetSceneNames()[sceneIndex].text;
            sceneIndices[property.propertyPath] = sceneIndex;
        }
    }

    private void GetSceneNameArray(SerializedProperty property)
    {
        string currentSceneName = property.stringValue;
        var scenes = EditorBuildSettings.scenes;
        var sceneNames = GetSceneNames();

        int initialIndex = 0; // Default to "None"
        if (!string.IsNullOrEmpty(currentSceneName))
        {
            for (int i = 0; i < sceneNames.Length; i++)
            {
                if (sceneNames[i].text == currentSceneName)
                {
                    initialIndex = i;
                    break;
                }
            }
        }

        sceneIndices[property.propertyPath] = initialIndex;
    }

    private GUIContent[] GetSceneNames()
    {
        var scenes = EditorBuildSettings.scenes;
        GUIContent[] sceneNames = new GUIContent[scenes.Length + 1];
        sceneNames[0] = new GUIContent("None");

        for (int i = 0; i < scenes.Length; i++)
        {
            string path = scenes[i].path;
            var splitPath = path.Split(scenePathSplit, System.StringSplitOptions.RemoveEmptyEntries);

            string sceneName = splitPath.Length > 0 ? splitPath[splitPath.Length - 1] : "(Deleted Scene)";
            sceneNames[i + 1] = new GUIContent(sceneName);
        }

        return sceneNames;
    }
}
#endif

