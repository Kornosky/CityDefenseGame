using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Unit
{
    protected override void Start()
    {
        base.Start();
        if (isEnemy) ChangeTarget(GameManager.Instance.playerBase);
        else ChangeTarget(GameManager.Instance.enemyBase);
    }
    protected override void Flipped(bool isFacingRight)
    {
        base.Flipped(isFacingRight);
        Debug.Log(isFacingRight);
        if(isFacingRight)
            actionCollider.transform.localPosition = new Vector2(-actionCollider.transform.localPosition.x, actionCollider.transform.localPosition.y);
        else
            actionCollider.transform.localPosition = actionCollider.transform.localPosition;
    }

    //Play the attack animation because something damagable has come into range
    public override void TryAction()
    {
        if (isActing || isDead)
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
        if(info.hitsAllInRange)
        {
            Unit[] targets = actionCollider.GetTargets();
            if(targets != null)
                foreach(var targ in targets)
                {
                    targ.GetHit(info.damage, transform.position, info.knockback);
                }
        }
        else
        {
            Unit target = actionCollider.GetTarget();

            if (target != null)
            {
                target.GetHit(info.damage, transform.position, info.knockback);
            }
        }
        //Still knockback and wait even if there was no target (empty swing)
        float recoilDirection = !isFacingRight ? -1f : 1f;
        if(rb != null)
            rb?.AddForce(new Vector2(recoilDirection * info.recoil.x, info.recoil.y), ForceMode2D.Impulse);

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
