using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewAnimationSequence", menuName = "Animation Sequence", order = 1)]
public class AnimationSequence : ScriptableObject
{
    public string sequenceName;
    public List<AnimationStep> steps;
    // public SequenceConditions conditions = new SequenceConditions(); 
}
