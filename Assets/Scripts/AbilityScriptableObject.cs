using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
/// <summary>
/// An option for the unit during their turn
/// </summary>
public abstract class AbilityScriptableObject : ScriptableObject
{
    public string abilityName;
    public float cooldownPeriod;
    [TextArea]
    public string description;
    //For AI interpretation
    public int manaCost;
    [Header("Visuals")]
    public Sprite icon;
    public GameObject vfxPrefab;
    public AudioClip[] sfx;
    protected Unit unit;
    [HideInInspector] public UnityEvent onActionCompleted;
    [HideInInspector] public UnityEvent executeEffect;
    [HideInInspector] public UnityEvent onActionStarted;

    public bool Begin()
    {
        Init();
        executeEffect.AddListener(Effect);
        return true;
    }
    public void OnStarted()
    {
        onActionStarted.Invoke();
        //controller.CompletedAction(this);
       // CanvasManager.Instance.DimCanvas(false);

        OnFinished();
        onActionStarted.RemoveAllListeners();
     
    }
    public void OnCompleted()
    {
        //if (!isAI )
        //{
        //    GameManager.Instance.playerController.RemoveMana(manaCost, true);
        //}

        onActionCompleted.Invoke();
        onActionCompleted.RemoveAllListeners();
    }
    public void AddTargets(List<Unit> t)
    {
 
    }
    /// <summary>
    /// Unique initialization 
    /// </summary>
    public abstract void Init();
    /// <summary>
    /// Unique effect
    /// </summary>
    protected abstract void Effect();
    /// <summary>
    /// Unique finish
    /// </summary>
    protected abstract void OnFinished();
    
}
