using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Range))]
public class Archer : Entity
{
    private Range range;

    protected override void Awake()
    {
        base.Awake();
        range ??= GetComponentInChildren<Range>();
    }
    protected override void Start()
    {
        base.Start();
        if (isEnemy) ChangeTarget(PlayerManager.Instance.playerBase);
        else ChangeTarget(PlayerManager.Instance.enemyBase);
    }
    public override void TryAction()
    {
        if (isActing)
            return;
        isActing = true;
        Debug.Log("Attack!");
        stateMachine.SetTrigger("Attack");
    }
    protected override void Action()
    {
        Attack();
    }

    private void Attack() 
    {
        if (actionCollider.GetTarget() == null)
        {
            isActing = false;
            return;
        }
        var gObj = (MonoBehaviour) actionCollider.GetTarget();
        range.LaunchProjectile(gObj.transform.position);

        //Recoil
        //Still knockback and wait even if there was no target (empty swing)
        float recoilDirection = !isFacingRight ? -1f : 1f;
        movement.Recoil(recoilDirection, info.recoil);

        StopAllCoroutines();
        StartCoroutine(Wait());

    }
    protected override IEnumerator Wait()
    {
        //To wait, type this:

        //Stuff before waiting
        yield return new WaitForSeconds(info.actionCD);
        isActing = false;
        //Check to see if can attack again
        if (actionCollider.GetTarget() != null)
            TryAction();
    }

}
