using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbyssBall : Projectile
{
    [SerializeField] CircleCollider2D effectorRadius;
    [SerializeField] PointEffector2D pointEffector;
    [SerializeField] AbyssExplosion explosion;
    private void Awake()
    {
        explosion.damaged.AddListener(Attack);
        explosion.gameObject.SetActive(false);
    }
    public override void Init(UnitScriptableObject info, bool isEnemy)
    {
        base.Init(info, isEnemy);

        gameObject.layer = !isEnemy ? LayerMask.NameToLayer("Player") : LayerMask.NameToLayer("Enemy"); ;

        foreach (Transform child in transform)
        {
            child.gameObject.layer = !isEnemy ? LayerMask.NameToLayer("Player") : LayerMask.NameToLayer("Enemy"); ;
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isInitialized)
            return;
        Unit unit = collision?.gameObject?.GetComponent<Unit>();
        if (collision.gameObject.GetComponent<IDamageable>() != null)
        {
            Debug.LogWarning("Collided");
            Activate();
        }
    }
    private void Attack(IDamageable unit)
    {
        unit?.GetHit(ownerInfo.Damage, transform.position, ownerInfo.Knockback);
    }
    void Activate()
    {
        explosion.gameObject.SetActive(true);
        effectorRadius.radius = 2.66f;
        //Tween.Value(0f, 2.66f, Expand, .5f, 0f, Tween.EaseInOutStrong, Tween.LoopType.None, null, null);
        if(GetComponent<Rigidbody2D>()) Destroy(GetComponent<Rigidbody2D>());
        if(GetComponent<Rigidbody2D>()) Destroy(GetComponent<Collider2D>());
        StartCoroutine("WaitToDie");
    }
    void Expand(float amt)
    {
        effectorRadius.radius = amt;
    }

    private void OnDestroy()
    {
        explosion.damaged.RemoveListener(Attack);
    }

    public override void OnHit(Collision2D target)
    {
        throw new System.NotImplementedException();
    }
}
