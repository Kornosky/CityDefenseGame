using Pixelplacement;
using Pixelplacement.TweenSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 [RequireComponent(typeof(Range))]
public class Turret : Structure, IPlaceable
{
    private TweenBase placingTween;
    private Range range;

    public bool IsValidPosition()
    {
        Collider2D myColl = GetComponent<Collider2D>();
        Bounds bound = myColl.bounds;

        var hitColliders = Physics2D.OverlapCircleAll(transform.position, 2);//1 is purely chosen arbitrarly
        foreach(var coll in hitColliders)
        {
            Debug.Log(coll);
            if (coll.bounds.Contains(bound.min) && coll.bounds.Contains(bound.max) && !coll.gameObject.CompareTag("Ground") )
            {
                if(coll.gameObject != gameObject && coll.GetComponent<Structure>())
                {
                    Debug.Log("Contained in " + coll.gameObject);
                    spriteRenderer.color = Color.green;
                    return true;
                }
            }
        }
        spriteRenderer.color = Color.red;
        return false;
    }

    public void Placing(bool isPlacing)
    {
        //do placing effects

        if (isPlacing)
        {
            Activate(false);
            gameObject.layer = LayerMask.NameToLayer("Placing");
            placingTween = Tween.LocalScale(spriteRenderer.transform, Vector3.one * 1.2f, .2f, 0, Tween.EaseOutStrong, Tween.LoopType.PingPong);
            hpBar.gameObject.SetActive(false);
        }
        else
        {
            spriteRenderer.color = Color.white;
            placingTween?.Cancel();
            rb.isKinematic = true;
            Activate(true);
            //Needs to be built
            if (info.buildTime > 0)
            {
                Unbuilt();
                PlayerManager.Instance.AddStructureToQueue(this);
            }
        }
    }


    public void Built(bool isBuilt)
    {
        throw new System.NotImplementedException();
    }

    //Archer methods -------------------------
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
        Debug.Log("Attack!");
        stateMachine.SetTrigger("Attack");
        Action();
    }
    protected override void Action()
    {
        Attack();
    }

    private void Attack()
    {
        if (actionCollider.GetTarget())
            return;
        isActing = true;
        var gObj = (MonoBehaviour)actionCollider.GetTarget();
        range.LaunchProjectile(gObj.transform.position);

        //Recoil
        rb?.AddForce(new Vector2(-(gObj.transform.position - transform.position).normalized.x * info.recoil.x,
                                                                info.recoil.y)
                                                                , ForceMode2D.Impulse);

        StartCoroutine(Wait());
    }

}
