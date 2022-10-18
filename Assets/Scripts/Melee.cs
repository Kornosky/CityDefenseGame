using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : Entity
{
    AnimationClip damageAnimation;
    protected override void Awake()
    {
        base.Awake();
        damageAnimation = Resources.Load<AnimationClip>("Explode_Basic");
    }
    protected override void Start()
    {
        base.Start();
        if (isEnemy) ChangeTarget(PlayerManager.Instance.playerBase);
        else ChangeTarget(PlayerManager.Instance.enemyBase);
    }

    //Play the attack animation because something damagable has come into range
    public override void TryAction()
    {
        if (isActing || isDead || actionCollider.GetTarget() == null)
            return;
        isActing = true;
        Debug.Log("Attack!");
        stateMachine.SetTrigger("Attack");     

    }
    protected override void Action()
    {
        base.Action();
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
                    targ.GetHit(info.Damage, transform.position, info.Knockback, targ.GetClosestPoint(transform.position), damageAnimation);
                }
        }
        else
        {
            Unit target = actionCollider.GetTarget();

            if (target != null)
            {                
                target.GetHit(info.Damage, transform.position, info.Knockback, target.GetClosestPoint(transform.position), damageAnimation);
            }
        }
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
        TryAction();
    }

}
