using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Resource : MonoBehaviour
{
    [SerializeField] UnitScriptableObject info;
    [SerializeField] WorkerTrigger trigger;
    private void Reset()
    {
        trigger ??= GetComponentInChildren<WorkerTrigger>();
    }
    private void Start()
    {
        trigger.Init(info, IsGathered);
    }
    public void IsGathered()
    {
        PlayerManager.Instance.Money += 5;
        Destroy(gameObject);
    }
}
