using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable 
{
    public abstract bool TakeDamage(int dmg);
    public abstract void GetHit(int damage, Vector3 sourcePos, Vector2 knockback);
}
