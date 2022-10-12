using System.Collections;
using System.Collections.Generic;
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
        throw new System.NotImplementedException();
    }

    protected override void Awake()
    {
        base.Awake();

        movement = GetComponentInChildren<Movement>(true) ?? gameObject.AddComponent<Movement>();
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
