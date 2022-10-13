using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : WorkerInteractable
{

    protected override void Start()
    {
        LevelManager.Instance.observedResourceCollection.Add(this);
        base.Start();
    }
    protected override void Interact()
    {
        PlayerManager.Instance.Money += 2;
    }

    public override void OnWorkerCompleteInteract()
    {
        Destroy(gameObject);
        foreach(Worker worker in interactingWorkers) // TODO would rather move this to an event
        {
            worker.ScanForGoal();
        }
    }

    public override void OnWorkerFailInteract()
    {
        throw new System.NotImplementedException();
    }

    private void OnDestroy()
    {
        LevelManager.Instance.observedResourceCollection.Remove(this);

    }

}
