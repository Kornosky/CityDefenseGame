using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class UnitArea : MenuElement
{
    [SerializeField] Animator animator;
    [SerializeField] TMP_Text description;

   public void Open(UnitScriptableObject info)
    {
        Visibility(true);
        var unit = info.prefab.GetComponent<Unit>();
        var clip = unit.GetAnimationClip("Idle", unit.GetComponentInChildren<Animator>());
        SetCurrentAnimation(animator, clip);
        description.text = info.description;
    }

    public override void Visibility(bool isVisible)
    {
        base.Visibility(isVisible);
        //Not effected by the canvas group
        animator.gameObject.SetActive(isVisible);
    }
    /// <summary>
    /// Sub in real animations for stubs
    /// </summary>
    /// <param name="animator">Reference to animator</param>
    public void SetCurrentAnimation(Animator animator, AnimationClip animClip)
    {
        AnimatorOverrideController myCurrentOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);

        RuntimeAnimatorController myOriginalController = myCurrentOverrideController.runtimeAnimatorController;

        // Know issue: Disconnect the orignal controller first otherwise when you will delete this override it will send a callback to the animator
        // to reset the SM
        myCurrentOverrideController.runtimeAnimatorController = null;

        AnimatorOverrideController myNewOverrideController = new AnimatorOverrideController();
        myNewOverrideController.runtimeAnimatorController = myOriginalController;

        myNewOverrideController["Idle"] = animClip;
        animator.runtimeAnimatorController = myNewOverrideController;

        Object.Destroy(myCurrentOverrideController);
    }
}
