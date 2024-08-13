

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[CustomPropertyDrawer(typeof(AnimationSequenceAttribute))]
public class AnimationSequenceDrawer : PropertyDrawer
{
    private List<AnimationSequence> animationSequences;
    private GUIContent[] sequenceNames;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (animationSequences == null || sequenceNames == null)
        {
            LoadAnimationSequences();
        }

        if (animationSequences.Count == 0)
        {
            EditorGUI.LabelField(position, label, new GUIContent("No Animation Sequences found"));
            return;
        }

        int selectedIndex = GetSelectedIndex(property);
        int newIndex = EditorGUI.Popup(position, label, selectedIndex, sequenceNames);

        if (newIndex != selectedIndex)
        {
            property.objectReferenceValue = animationSequences[newIndex];
        }
    }

    private void LoadAnimationSequences()
    {
        animationSequences = AssetDatabase.FindAssets("t:AnimationSequence")
            .Select(guid => AssetDatabase.LoadAssetAtPath<AnimationSequence>(AssetDatabase.GUIDToAssetPath(guid)))
            .ToList();

        sequenceNames = animationSequences.Select(seq => new GUIContent(seq.name)).ToArray();
    }

    private int GetSelectedIndex(SerializedProperty property)
    {
        var currentObject = property.objectReferenceValue as AnimationSequence;
        if (currentObject == null) return 0;

        return animationSequences.IndexOf(currentObject);
    }
}
#endif
