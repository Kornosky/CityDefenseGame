using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHitState : StateMachineBehaviour
{
    protected Unit unit;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        unit = animator.GetComponentInParent<Unit>();
        unit.Invincibility(true);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        unit.FinishedHit();
        unit.Invincibility(false);
    }
}
