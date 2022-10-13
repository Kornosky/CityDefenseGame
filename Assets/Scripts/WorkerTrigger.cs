using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WorkerTrigger : MonoBehaviour
{
    public UnityEvent<Worker> onTriggerEnter;
    public UnityEvent<Worker> onTriggerExit;
    public Action onComplete;
    public WorkerInteractable workerInteractable;

    private void Awake()
    {
        workerInteractable = GetComponentInParent<WorkerInteractable>();
    }
    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Worker");
    }
    public void Init(Action onComplete)
    {
        this.onComplete += onComplete;
    }
    public void RemoveInteraction()
    {
        GetComponent<Collider2D>().enabled = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Worker worker = collision.GetComponent<Worker>();
   
        if (worker && worker.CurrentGoal && worker.CurrentGoal.gameObject == workerInteractable.gameObject)
        {
            // If the interactable has already been completed, then ignore
            if(workerInteractable.IsComplete())
            {
                return;
            }
            workerInteractable.OnWorkerBeginInteract(worker);
            onTriggerExit.Invoke(worker);
            //worker.EnteredTrigger(this);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Worker worker = collision.gameObject.GetComponent<Worker>();
        if (worker)
        {
            workerInteractable.InterruptInteraction();
            onTriggerExit.Invoke(worker);
           //# worker.ExitSource(this);
        }
    }
}
