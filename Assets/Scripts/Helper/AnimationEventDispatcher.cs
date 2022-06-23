using System;
using UnityEngine;

/// <summary>
/// Dynamimcally creates start/end animation events an animator (optionally assigned or dynamically found), which Monobehaviours may subscribe to
/// Modified from source - https://gamedev.stackexchange.com/questions/117423/unity-detect-animations-end
/// </summary>
public class AnimationEventDispatcher : MonoBehaviour
{
    public Action<string> OnAnimationStart;
    public Action<string> OnAnimationComplete;

    [SerializeField]
    private Animator animator;

    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (animator?.runtimeAnimatorController?.animationClips == null)
            return;

        for (int i = 0; i < animator.runtimeAnimatorController.animationClips.Length; i++)
        {
            AnimationClip clip = animator.runtimeAnimatorController.animationClips[i];

            AnimationEvent animationStartEvent = new AnimationEvent();
            animationStartEvent.time = 0;
            animationStartEvent.functionName = "AnimationStartHandler";
            animationStartEvent.stringParameter = clip.name;

            AnimationEvent animationEndEvent = new AnimationEvent();
            animationEndEvent.time = clip.length;
            animationEndEvent.functionName = "AnimationCompleteHandler";
            animationEndEvent.stringParameter = clip.name;

            clip.AddEvent(animationStartEvent);
            clip.AddEvent(animationEndEvent);
        }
    }

    public void AnimationStartHandler(string name)
    {
        OnAnimationStart?.Invoke(name);
    }

    public void AnimationCompleteHandler(string name)
    {
        OnAnimationComplete?.Invoke(name);
    }
}
