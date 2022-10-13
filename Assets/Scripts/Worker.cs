using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Worker : Entity
{
    private Component currentGoal;
    private WorkerTrigger currentTrigger;
    private bool isOnTheJob;

    public Component CurrentGoal { get => currentGoal; set => currentGoal = value; }

    protected override void Start()
    {
        base.Start();
       
        PlayerManager.Instance.structureRequestedAction.AddListener(StartJob);
        PlayerManager.Instance.structureCancelledAction.AddListener(EndJob);
        LevelManager.Instance.workers.Add(this);

        var job = PlayerManager.Instance.CheckForStructures();
        if (job)
            StartJob(job);
        else
            CheckForResources();
    }
    public UnitStructure GetBuildJob()
    {        
        return currentGoal as UnitStructure; //return null if current goal isnt a unitstructure
    }

    public void CancelBuilding()
    {
        StopAllCoroutines();
    }
    public void EnteredTrigger(WorkerTrigger source)
    {
        this.currentTrigger = source;
        //StartAction();
    }
    public void ExitSource(WorkerTrigger source)
    {
        this.currentTrigger = null;
        if(currentTrigger == source) //left the previous trigger
            StopAction();
    }

    public void StopAction()
    {
        if (currentGoal == null) //messes with the coroutine otherwise if colliders are together
            return;
        CancelBuilding();
    }
    private void StartJob(UnitStructure job)
    {
        job.AddWorker(this);
        isOnTheJob = true;
        currentGoal = job;
        ChangeTarget(job.transform);
    }
    public override void ScanForGoal()
    {
        if (isOnTheJob) //already on a job
            return;
        var jobs = PlayerManager.Instance.CheckForStructures();
        if (jobs && !isOnTheJob)  // If there is a job available and not already on one
            StartJob(jobs);
        else if (CheckForResources())        // Search for Resource

        { }
        else
            ChangeTarget(homeBase);
    }

    public void EndJob(UnitStructure job)
    {
        //NOt me!
        if (job != currentGoal)
            return;
        isOnTheJob = false;
        currentGoal = null;
        ScanForGoal();
    }
    //Check for resources to start mining if available
    bool CheckForResources()
    {
        
        if (LevelManager.Instance.observedResourceCollection.Count != 0)
        {
            foreach (var item in LevelManager.Instance.observedResourceCollection.OrderBy(o => Vector2.Distance(o.transform.position, transform.position)).ToList())
            {
                // Check if available then break if available
                currentGoal = item;
                ChangeTarget(item.transform);
                break;
            }             

            return true;
        }
        else
            return false;
        
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        PlayerManager.Instance.structureRequestedAction.RemoveListener(StartJob);
        PlayerManager.Instance.structureCancelledAction.RemoveListener(EndJob);
    }

    protected override void Action()
    {
       // throw new System.NotImplementedException();
    }

    public override void TryAction()
    {
        //throw new System.NotImplementedException();
    }
    public void Destroy()
    {
        LevelManager.Instance.workers.Remove(this);
    }
}
