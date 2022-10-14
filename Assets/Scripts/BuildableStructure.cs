using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(UnitStructure))]
public class BuildableStructure : WorkerInteractable
{
    private List<Worker> workers = new List<Worker>();
    protected bool isActive;
    protected Rigidbody2D rb;
    protected UnitStructure unitStructure;
    
    protected override void Awake()
    {
        base.Awake();

        rb ??= GetComponentInChildren<Rigidbody2D>();
        unitStructure ??= GetComponentInChildren<UnitStructure>();

    }
   
    public override void OnWorkerCompleteInteract()
    {
        base.OnWorkerCompleteInteract();
        PlayerManager.Instance.RemoveStructureFromQueue(unitStructure); //race case with next operation

        foreach (var worker in workers)
        {
            worker.EndJob(unitStructure);
        }
        unitStructure.BuildCompleted();
    }
    public override void OnWorkerBeginInteract(Worker worker)
    {
        base.OnWorkerBeginInteract(worker);
        workers.Add(worker);
    }
    
    public override void OnWorkerFailInteract()
    {
        Debug.LogWarning("Not implemented");
    }

    protected override void Interact()
    {
        Debug.LogWarning("Not implemented");
    }




}
