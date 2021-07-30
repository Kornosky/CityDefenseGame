using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CanvasGroup))]
public abstract class MenuElement : MonoBehaviour
{
    protected CanvasGroup canvasGroup;

    protected virtual void Awake()
    {
        LevelManager.Instance.OnGameStateChange += HandleGameStateChange;

        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        Visibility(false);
    }
    protected virtual void Start()
    {

    }
    void HandleGameStateChange(LevelManager.LevelState from, LevelManager.LevelState to)
    {
        switch (to)
        {
            case LevelManager.LevelState.Paused:
                // stop or pause couroutine etc
               /// StartCoroutine(coroutine);
                break;
        }
    }

    public virtual void Visibility(bool isVisible, bool willFade = false, float duration = .3f)
    {
        if(canvasGroup == null)
        {

            Debug.Log("Why is this null? " + canvasGroup + " == " + gameObject);
            canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        }
        //Nothing needs to be modified
        if (isVisible == canvasGroup.interactable)
            return;
        if(willFade)
        {
            if (isVisible)
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                Tween.CanvasGroupAlpha(GetComponent<CanvasGroup>(), 1, duration, 0f);
            }
            else
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                Tween.CanvasGroupAlpha(GetComponent<CanvasGroup>(), 0, duration, 0f);
            }
        }
        else
        {
            if (isVisible)
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                canvasGroup.alpha = 1;
            }
            else
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.alpha = 0;
            }
        }
      
    }
}
