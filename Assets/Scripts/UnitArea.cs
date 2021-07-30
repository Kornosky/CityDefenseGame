using Pixelplacement;
using Pixelplacement.TweenSystem;

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
    [SerializeField] Transform descriptionBG;
    TweenBase tween = null;
   public void Open(UnitScriptableObject info)
    {
        Visibility(true);
        var unit = info.prefab.GetComponent<Unit>();
        var clip = unit.GetAnimationClip("Idle", unit.GetComponentInChildren<Animator>());
        SetCurrentAnimation(animator, clip);
        ChangeDescription(info.description);
    }

    public void ChangeDescription(string desc)
    {
        description.text = desc;
        float delay = 0;
        //if(tween != null)
        //{
        //    tween = Tween.AnchoredPosition(descriptionBG.GetComponent<RectTransform>(), new Vector2(0, -33), new Vector2(0, -134), .2f, 0, Tween.EaseInBack, Tween.LoopType.None, null, null);
        //    delay = .1f;
        //}
        tween = Tween.AnchoredPosition(descriptionBG.GetComponent<RectTransform>(), new Vector2(0, -134), new Vector2(0, -33), .1f, delay, Tween.EaseOutBack, Tween.LoopType.None, null, null);
        
    }
    public override void Visibility(bool isVisible, bool willFade = false, float duration = .3f)
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
