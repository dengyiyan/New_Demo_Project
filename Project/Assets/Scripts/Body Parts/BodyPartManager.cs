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


    private SpriteRenderer bodyRenderer;
    private SpriteRenderer armsRenderer;
    private SpriteRenderer hairRenderer;
    private SpriteRenderer pantsRenderer;


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

        bodyRenderer = gameObject.transform.Find("Body").GetComponent<SpriteRenderer>();

        armsRenderer = gameObject.transform.Find("Arms").GetComponent<SpriteRenderer>();
        hairRenderer = gameObject.transform.Find("Hair").GetComponent<SpriteRenderer>();
        pantsRenderer = gameObject.transform.Find("Pants").GetComponent<SpriteRenderer>();

        bodyRenderer.flipX = false;
        armsRenderer.flipX = false;
        hairRenderer.flipX = false;
        pantsRenderer.flipX = false;


    }

    private void OnEnable()
    {
        EventHandler.UpdateColorEvent += ApplyColor;
        EventHandler.UpdateBodyPartEvent += UpdateBodyPart;
    }
    private void OnDisable()
    {
        EventHandler.UpdateColorEvent -= ApplyColor;
        EventHandler.UpdateBodyPartEvent -= UpdateBodyPart;
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
            // Find the child object with the specified name
            Transform childTransform = transform.Find(type);

            if (childTransform != null)
            {
                // Get the GameObject of the found transform
                GameObject obj = childTransform.gameObject;

                // Get the SpriteRenderer component and apply the color
                SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = characterBody.characterBodyParts[partIndex].bodyPart.bodyPartColor;
                }
                else
                {
                    Debug.LogWarning($"SpriteRenderer not found on child object: {type}");
                }
            }
            else
            {
                Debug.LogWarning($"Child object not found: {type}");
            }
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
