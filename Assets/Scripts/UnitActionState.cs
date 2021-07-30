using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitActionState : StateMachineBehaviour
{
    protected Unit unit;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        unit = animator.GetComponentInParent<Unit>();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
