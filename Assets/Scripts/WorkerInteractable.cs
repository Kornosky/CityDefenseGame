using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Any object that is interactable by a worker inherits this class
/// </summary>
public abstract class WorkerInteractable : MonoBehaviour
{
    [SerializeField] int tick = 1;

    [SerializeField] protected ActionBar actionBar;

    [Header("Needs to be set")]
    [SerializeField] protected MonoBehaviour identifyingMonoBehavior;
    [SerializeField] WorkerTrigger workerTrigger;
    [SerializeField] protected List<Worker> interactingWorkers = new List<Worker>();

    protected virtual void Awake()
    {
        actionBar ??= GetComponentInChildren<ActionBar>();
    }

    protected virtual void Start()
    {
        actionBar.Init((identifyingMonoBehavior as Unit).Info);
    }
    public virtual void OnWorkerBeginInteract(Worker worker)
    {
        interactingWorkers.Add(worker);
        StartCoroutine("Interacting");
    }
    protected IEnumerator Interacting()
    {
        if (actionBar.IsComplete())
        {
            Debug.LogWarning(string.Format("WorkerInteractable [{0}] is being interacted with after completion.", gameObject.name));
        }
        while(!actionBar.IsComplete())
        {
            yield return new WaitForSeconds(tick);
            actionBar.UpdateValue(1); //TODO if workers get upgradable tick amts
            Interact();
        }
        OnWorkerCompleteInteract();
    }
    public bool IsComplete()
    {
        return actionBar.IsComplete();
    }
    protected abstract void Interact();
    public void InterruptInteraction()
    {
        StopCoroutine("Interacting");
    }
    public virtual void OnWorkerCompleteInteract()
    {
        interactingWorkers.Clear();
        actionBar.gameObject.SetActive(false);
        workerTrigger.RemoveInteraction();
    }
    public abstract void OnWorkerFailInteract(); //if interrupted early

    private void Reset()
    {
        actionBar ??= transform.parent.gameObject.GetComponentInChildren<ActionBar>();
    }


}