using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class BodyPartManager : MonoBehaviour
{
    [SerializeField] private CharacterBody characterBody;

    [SerializeField] private string[] bodyPartTypes;
    [SerializeField] private string[] characterStates;
    [SerializeField] private string[] characterDirections;

    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip animationClip;
    [SerializeField] private AnimatorOverrideController animatorOverrideController;
    [SerializeField] private AnimationClipOverrides defaultAnimationClips;

    private Dictionary<string, AnimationClip> animationCache = new Dictionary<string, AnimationClip>();


    //public SO_BodyPart[] allParts;

    //private void Awake()
    //{
    //    foreach (var part in allParts)
    //    {
    //        part.resetColor();
    //    }
    //}
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;

        defaultAnimationClips = new AnimationClipOverrides(animatorOverrideController.overridesCount);
        animatorOverrideController.GetOverrides(defaultAnimationClips);

        UpdateBodyPart();
        ApplyColor();
    }

    public void UpdateBodyPart()
    {
        for (int partIndex = 0; partIndex < bodyPartTypes.Length; partIndex++)
        {
            string type = bodyPartTypes[partIndex];
            string id = characterBody.characterBodyParts[partIndex].bodyPart.bodyPartAnimationID.ToString();

            for (int stateIndex = 0; stateIndex < characterStates.Length; stateIndex++)
            {
                string state = characterStates[stateIndex];

                for (int directionIndex = 0; directionIndex < characterDirections.Length; directionIndex++)
                {
                    string direction = characterDirections[directionIndex];
                    string path = "New_Animation/" + type + "/" + type.ToLower() + "_" + id +  "/" + type.ToLower() + "_" + id 
                        + "_" + state.ToLower() + "_" + direction.ToLower();

                    //if (!animationCache.TryGetValue(path, out AnimationClip loadedClip))
                    //{
                    //    loadedClip = Resources.Load<AnimationClip>(path);
                    //    if (loadedClip != null)
                    //    {
                    //        animationCache[path] = loadedClip;
                    //    }
                    //    else
                    //    {
                    //        Debug.LogWarning("Animation not found: " + path);
                    //        continue;
                    //    }
                    //}

                    //defaultAnimationClips[type + "_" + 0 + "_" + state + "_" + direction] = loadedClip;

                    animationClip = Resources.Load<AnimationClip>(path);

                    // Override default animation
                    defaultAnimationClips[type.ToLower() + "_" + 0 + "_" + state.ToLower() + "_" + direction.ToLower()] = animationClip;
                }
            }
        }

        animatorOverrideController.ApplyOverrides(defaultAnimationClips);
    }


    private void ApplyColor()
    {
        for (int partIndex = 0; partIndex < bodyPartTypes.Length; partIndex++)
        {
            string type = bodyPartTypes[partIndex];
            GameObject obj = GameObject.Find(type);
            SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
            spriteRenderer.color = characterBody.characterBodyParts[partIndex].bodyPart.bodyPartColor;
        }
    }


    public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
    {
        public AnimationClipOverrides(int capacity) : base(capacity) { }

        public AnimationClip this[string name]
        {
            get { return this.Find(x => x.Key.name.Equals(name)).Value; }
            set
            {
                int index = this.FindIndex(x => x.Key.name.Equals(name));
                if (index != -1)
                    this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
            }
        }
    }
}
