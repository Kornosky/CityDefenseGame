using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerTrigger : MonoBehaviour
{
    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Worker");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Worker worker = collision.gameObject.GetComponent<Worker>();
        if (worker)
        {
            worker.StartBuilding();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Worker worker = collision.gameObject.GetComponent<Worker>();
        if (worker)
        {
            worker.StopBuilding();
        }
    }
}
