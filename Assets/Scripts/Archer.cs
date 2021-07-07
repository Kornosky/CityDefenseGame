using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Range))]
public class Archer : Unit
{
    private Range range;

    protected override void FixedUpdate()
    {   
        base.FixedUpdate();
    }
    protected override void Awake()
    {
        base.Awake();
        range ??= GetComponentInChildren<Range>();
    }
    protected override void Start()
    {
        base.Start();
        if (isEnemy) ChangeTarget(GameManager.Instance.playerBase);
        else ChangeTarget(GameManager.Instance.enemyBase);
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
        rb?.AddForce(new Vector2(-(gObj.transform.position - transform.position).normalized.x * info.recoil.x,
                                                                info.recoil.y)
                                                                , ForceMode2D.Impulse);
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
