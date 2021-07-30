using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericProjectile : Projectile
{
    public override void OnHit(Collision2D target)
    {
        //do nothing because generic
    }
}
