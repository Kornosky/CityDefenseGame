using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : Projectile
{
    bool isRemoving;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        damageAnimation = Resources.Load<AnimationClip>("Explode_Basic");
    }

    private void LateUpdate()
    {
        Vector2 v = rb.velocity;
        float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isInitialized)
            return;
        Unit unit = collision?.gameObject?.GetComponent<Unit>();
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        if (collision.gameObject.GetComponent<IDamageable>() != null)
        {
            Attack(collision);
        }
    }

    void RemoveArrow()
    {
        if (isRemoving)
            return;
        isRemoving = true;
        Destroy(GetComponent<Collider2D>());
        StartCoroutine(WaitToDie());
        //Tween.Color(GetComponent<SpriteRenderer>(), Color.clear, .5f, 0, Tween.EaseLinear, Tween.LoopType.None, null, () => Destroy(gameObject));
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        timer += Time.fixedDeltaTime;
        if(timer >= decayTime)
        {
            RemoveArrow();
        }
    }

    private void Attack(Collision2D target)
    {
        Unit unit = target.gameObject.GetComponent<Unit>();
        unit?.GetHit(ownerInfo.Damage, transform.position, ownerInfo.Knockback, transform.position, damageAnimation);
        OnHit(target);
        RemoveArrow();
    }

    public override void OnHit(Collision2D target)
    {
        ///throw new System.NotImplementedException();
    }
}
