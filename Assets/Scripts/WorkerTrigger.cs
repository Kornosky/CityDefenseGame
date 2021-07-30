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
    private float buildProgress;
    public UnitScriptableObject info;
    [SerializeField] BuildBar buildBar;
    Worker currentWorker;
    private void Awake()
    {
        buildBar ??= GetComponentInChildren<BuildBar>();
        buildProgress = 0;
    }
    private void Reset()
    {
        buildBar ??= transform.parent.gameObject.GetComponentInChildren<BuildBar>();
    }
    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Worker");

    }
    public void Init(UnitScriptableObject info, Action onComplete)
    {
        this.info = info;
        this.onComplete += onComplete;
        buildBar.Init(info);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Worker worker = collision.gameObject.GetComponent<Worker>();
        if (worker)
        {
            onTriggerExit.Invoke(worker);
            worker.EnteredTrigger(this);
        }
    }
    public bool Build(float amt)
    {
        buildProgress += amt;
        buildBar.UpdateValue(amt);
        if (buildProgress >= info.buildTime)
        {
            onComplete.Invoke();
            buildBar.gameObject.SetActive(false);
            return true;
        }
        return false;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Worker worker = collision.gameObject.GetComponent<Worker>();
        if (worker)
        {
            onTriggerExit.Invoke(worker);
            worker.ExitSource(this);
        }
    }
}
