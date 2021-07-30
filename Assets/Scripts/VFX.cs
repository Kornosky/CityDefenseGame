using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Controls VFX prefab
/// </summary>
public class VFX : MonoBehaviour
{
    [SerializeField] bool isOneShot;
    [SerializeField] Animator animator;

    public void Start()
    {
        animator.StopPlayback();
    }
    
    //Initialize
    public void Init(AnimationClip clip, bool isOneShot = true)
    {
        //swap clip
        if (clip)
        {
            AnimatorOverrideController aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);
            var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            foreach (var a in aoc.animationClips)
                anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, clip));
            aoc.ApplyOverrides(anims);
            animator.runtimeAnimatorController = aoc;
        }

        StartCoroutine(DestroyTimer());
        animator.StartPlayback();
    }


    IEnumerator DestroyTimer()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
        Destroy(gameObject);
    }
}
