using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : Entity
{
    private Structure currentBuildJob;
    private WorkerTrigger currentTrigger;
    private bool isOnTheJob;
    protected override void Start()
    {
        base.Start();
       
        PlayerManager.Instance.structureRequestedAction.AddListener(StartJob);
        PlayerManager.Instance.structureCancelledAction.AddListener(EndJob);

        var job = PlayerManager.Instance.CheckForStructures();
        if (job)
            StartJob(job);
        else
            CheckForResources();
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
        while (!currentTrigger.Build(Time.deltaTime))
        {
            yield return new WaitForEndOfFrame();
        }
        CheckForResources();
    }
    public void CancelBuilding()
    {
        StopAllCoroutines();
    }
    public void EnteredTrigger(WorkerTrigger source)
    {
        this.currentTrigger = source;
        StartBuilding();
    }
    public void ExitSource(WorkerTrigger source)
    {
        this.currentTrigger = null;
        if(currentTrigger == source) //left the previous trigger
            StopBuilding();
    }
    public void StartBuilding()
    {
        Build();
    }
    public void StopBuilding()
    {
        if (currentBuildJob == null) //messes with the coroutine otherwise if colliders are together
            return;
        CancelBuilding();
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
        //NOt me!
        if (job != currentBuildJob)
            return;
        isOnTheJob = false;
        currentBuildJob = null;
        CheckForResources();
        //Check for another job
        var jobs = PlayerManager.Instance.CheckForStructures();
        if (jobs)
            StartJob(jobs);
    }
    //Check for resources to start mining if available
    bool CheckForResources()
    {
        KeyValuePair<Collider2D, float> closest = new KeyValuePair<Collider2D, float>(null, Mathf.Infinity);
        float distance = Mathf.Infinity;
        foreach(var coll in Physics2D.OverlapCircleAll(transform.position, info.range))
        {
            if (coll.GetComponent<WorkerTrigger>() == null)
                continue;
            distance = Vector2.Distance(coll.transform.position, transform.position);
            if (distance < closest.Value )
            {
                closest = new KeyValuePair<Collider2D, float>(coll, distance);
            }
        }

        if (closest.Key != null)
        {
            ChangeTarget(closest.Key.transform);

            return true;
        }
        else
        {
            ChangeTarget(homeBase);
            return false;
        }
    }
    private void StartBuilding(Collision2D target)
    {
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
}
