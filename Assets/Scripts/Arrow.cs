using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public UnitScriptableObject ownerInfo;
    bool isInitialized;
    float timer;
    float decayTime = .5f;
    public void Init(UnitScriptableObject info, bool isEnemy)
    {
        ownerInfo = info;
        if (isEnemy)
        {
            gameObject.layer = LayerMask.NameToLayer("Enemy");
        }
        else
        {
           gameObject.layer = LayerMask.NameToLayer("Player");
        }

        isInitialized = true;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isInitialized)
            return;
        Unit unit = collision?.gameObject?.GetComponent<Unit>();
        if (collision.gameObject.GetComponent<IDamageable>() != null)
        {
            Attack(collision);
        }
        if (collision.gameObject.CompareTag("Ground"))
        {
            RemoveArrow();
        }
    }

    void RemoveArrow()
    {
        Destroy(GetComponent<Collider2D>());
        Tween.Color(GetComponent<SpriteRenderer>(), Color.clear, .5f, 0, Tween.EaseLinear, Tween.LoopType.None, null, () => Destroy(gameObject));
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
        if (target.gameObject?.GetComponent<Rigidbody2D>())
            target.gameObject?.GetComponent<Rigidbody2D>()?.AddForce(
                                                                    new Vector2((target.gameObject.transform.position - transform.position).normalized.x * ownerInfo.knockback.x,
                                                                    ownerInfo.knockback.y)
                                                                    , ForceMode2D.Impulse);

        target.gameObject.GetComponent<IDamageable>().TakeDamage(ownerInfo.damage);

        Destroy(gameObject);
    }
}
