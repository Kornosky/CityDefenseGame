using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Entity : Unit
{
    protected Movement movement;

    public override void TryAction()
    {
        throw new System.NotImplementedException();
    }

    protected override void Action()
    {        
        try
        {
            SFXManager.Main.PlayFromSFXObjectLibrary(string.Format("{0}_{1}", this.GetType().Name, MethodBase.GetCurrentMethod().Name), new Vector3(transform.position.x, transform.position.y, -10), transform);
        }
        catch
        {  }
    }

    protected override void Awake()
    {
        base.Awake();

        movement = GetComponentInChildren<Movement>(true) ?? gameObject.AddComponent<Movement>();
    }
    protected void FaceTarget(Transform target)
    {
        Vector2 targetDirection = target.position - transform.position;

        // flip to position of target
        movement.Flipped(targetDirection.x < 0 ? true : false);
    }

    public override bool GetHit(int damage, Vector3 sourcePos, Vector2 knockback, Vector3 location, AnimationClip vfx)
    {
        if(base.GetHit(damage, sourcePos, knockback, location, vfx))
        {         
            movement.Knockback(sourcePos, knockback);

            return true;
        }

        return false;
    }
    public override bool GetHit(int damage, Vector3 sourcePos, Vector2 knockback)
    {
        if(base.GetHit(damage, sourcePos, knockback))
        {
            movement.Knockback(sourcePos, knockback);

            return true;
        }

        return false;
    }
}
