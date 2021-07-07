using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActionCollider : MonoBehaviour
{
    [SerializeField] private List<Unit> unitsInTrigger = new List<Unit>();
    public UnityEvent actionEvent;
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<IDamageable>() != null)
        {
            unitsInTrigger.Add(collision.GetComponent<Unit>());
            actionEvent.Invoke();
        }
    }

    public Unit GetTarget()
    {
        if (unitsInTrigger.Count > 0)
            return unitsInTrigger[Random.Range(0, unitsInTrigger.Count)];
        else
            return null;
    }
    public Unit[] GetTargets()
    {
        if (unitsInTrigger.Count > 0)
            return unitsInTrigger.ToArray();
        else
            return null;
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<IDamageable>() != null)
        {
            unitsInTrigger.Remove(collision.GetComponent<Unit>());
        }
    }

}
