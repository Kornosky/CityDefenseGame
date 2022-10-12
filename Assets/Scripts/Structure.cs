using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Pixelplacement.TweenSystem;
/// <summary>
/// Needs to be built by worker
/// </summary
public abstract class Structure : Unit
{
    [Header("Structure Class")]
    [SerializeField] protected new StructureScriptableObject info;
    private bool isBuilt;
    private List<Worker> workers = new List<Worker>();
    public float buildProgress;
    protected bool isActive;
    protected Rigidbody2D rb;

    [SerializeField] WorkerTrigger workerTrigger;

    public override void Init(bool isEnemy, UnitScriptableObject info = null)
    {
        //TODO remove this nonesense
        base.Init(isEnemy, info);
        this.isEnemy = isEnemy;
       // ChangeLayer(isEnemy, "Placing");
    }
    protected override void Awake()
    {
        base.Awake();
        workerTrigger ??= GetComponentInChildren<WorkerTrigger>();
        rb ??= GetComponentInChildren<Rigidbody2D>();
        buildProgress = 0;
    }   
    protected override void Start()
    {
        base.Start();
        if(workerTrigger != null)
        {
            workerTrigger.Init(info, FinishedBuilding);
            workerTrigger.onTriggerEnter.AddListener(WorkerBuilding);
            workerTrigger.onTriggerExit.AddListener(WorkerStopped);
        }
    }
    void WorkerBuilding(Worker worker)
    {

    }
    void WorkerStopped(Worker worker)
    {

    }

    protected void Unbuilt()
    {
        spriteRenderer.color = new Color(0,0,0,.3f);
    }
   
    public void Activate(bool isActive)
    {
        this.isActive = isActive;
    }

    private void FinishedBuilding()
    {
        isBuilt = true;
        workerTrigger.gameObject.SetActive(false);
        spriteRenderer.color = Color.white;
        hpBar.gameObject.SetActive(true);
        ChangeLayer(isEnemy);
        PlayerManager.Instance.RemoveStructureFromQueue(this); //race case with next operation

        foreach (var worker in workers)
        {
            worker.EndJob(this);
        }

    }
    public virtual void AddWorker(Worker worker)
    {
        workers.Add(worker);
    }

    public virtual bool HasWorkerLimit()
    {
        if(info.workersRequired >= workers.Count)
        {
            return true;
        }
        return false;
    }
}
