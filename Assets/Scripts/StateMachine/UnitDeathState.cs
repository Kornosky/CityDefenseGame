using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDeathState : StateMachineBehaviour
{
    protected Unit unit;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        unit = animator.GetComponentInParent<Unit>();
        unit.RemoveInteraction();

        if (unit.isEnemy)
        {
            unit.DropPickup();
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Destroy(unit.gameObject);
    }
}
