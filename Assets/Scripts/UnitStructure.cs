using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Needs to be built by worker
/// </summary
/// 

public abstract class UnitStructure : Unit
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
 
    public void Activate(bool isActive)
    {
        this.isActive = isActive;
    }
    public virtual void AddWorker(Worker worker)
    {
        workers.Add(worker);
    }

    public virtual bool IsAtWorkerLimit()
    {
        if (workers.Count >= info.workersRequired)
        {
            return true;
        }
        return false;
    }

    public virtual void BuildCompleted()
    {
        hpBar.gameObject.SetActive(true);
    }
}
