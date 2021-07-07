using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : Unit
{
    private Structure currentBuildJob;
    private bool isOnTheJob;
    protected override void Start()
    {
        base.Start();
       
        GameManager.Instance.structureRequestedAction.AddListener(StartJob);
        GameManager.Instance.structureCancelledAction.AddListener(EndJob);

        var job = GameManager.Instance.CheckForStructures();
        if (job)
            StartJob(job);
    }
    public Structure GetBuildJob()
    {
        return currentBuildJob;
    }
    public virtual void Build()
    {
        StartCoroutine("Building");
    }
    IEnumerator Building()
    {
        while (currentBuildJob && !currentBuildJob.Build(Time.deltaTime))
        {
            yield return new WaitForEndOfFrame();
        }
    }
    public void CancelBuilding()
    {
        StopAllCoroutines();
    }
    public void StartBuilding()
    {
        Build();
        Debug.Log("WOrker has begun building");
    }
    public void StopBuilding()
    {
        if (currentBuildJob == null) //messes with the coroutine otherwise if colliders are together
            return;
        CancelBuilding();
        Debug.Log("WOrker has stopped building");
    }
    private void StartJob(Structure job)
    {
        //Probably need to just grab the closest workers.....
        //if (job.HasWorkerLimit())
        //    return;
        if (isOnTheJob) //already on a job
            return;
        job.AddWorker(this);
        isOnTheJob = true;
        currentBuildJob = job;
        ChangeTarget(job.transform);
    }
    public void EndJob(Structure job)
    {
        Debug.Log("WOrker has ended job");

        //NOt me!
        if (job != currentBuildJob)
            return;
        isOnTheJob = false;
        currentBuildJob = null;
        ChangeTarget(homeBase);
        //Check for another job
        var jobs = GameManager.Instance.CheckForStructures();
        if (jobs)
            StartJob(jobs);
    }
    private void StartBuilding(Collision2D target)
    {
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameManager.Instance.structureRequestedAction.RemoveListener(StartJob);
        GameManager.Instance.structureCancelledAction.RemoveListener(EndJob);
    }

    protected override void Action()
    {
        throw new System.NotImplementedException();
    }

    public override void TryAction()
    {
        throw new System.NotImplementedException();
    }
}
