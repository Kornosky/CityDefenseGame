using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnitAnimator : MonoBehaviour
{
    private Unit unit;
    public UnityEvent actionEvent;
    public UnityEvent hitFinishedEvent;
    private void Awake()
    {
        //unit = GetComponentInParent<Unit
    }
    public void DestroyFromDeathAnim()
    {
        //Pretty shitty
        Destroy(GetComponentInParent<Unit>().gameObject);
    }
    public void Attack()
    {
        actionEvent.Invoke();
    }
    public void FinishedHit()
    {
        hitFinishedEvent.Invoke();
    }
}
