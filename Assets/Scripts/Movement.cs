using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    protected SpriteRenderer spriteRenderer;
    protected Unit unit;
    protected bool isGrounded;
    protected bool isFacingRight;
    protected Rigidbody2D rb;


    public float toVel = .1f;
    public float maxVel = 1.0f;
    public float maxForce = 20.0f;
    public float gain = 5f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        unit = GetComponent<Unit>();
    }
    public void Recoil(float direction, Vector2 recoil)
    {
        rb.AddForce(new Vector2(direction * recoil.x, recoil.y), ForceMode2D.Impulse);
    }
    public void Knockback(Vector3 sourcePos, Vector2 knockback)
    {
        rb.AddForce(new Vector2((gameObject.transform.position - sourcePos).normalized.x * knockback.x, knockback.y), ForceMode2D.Impulse);
    }
    private void Start()
    {
        if (unit.isEnemy)
        {
            isFacingRight = !unit.isEnemy; //hmmm
            unit.homeBase = PlayerManager.Instance.enemyBase;
        }
        else
        {

            isFacingRight = unit.isEnemy; //hmmm
            unit.homeBase = PlayerManager.Instance.playerBase;
        }
        Flipped(isFacingRight);
    }
    protected virtual void Flipped(bool isFacingLeft)
    {
        spriteRenderer.flipX = isFacingLeft;
    }
    protected virtual void FixedUpdate()
    {
        if (!unit.GetInfo().canMoveWhileActing && unit.isActing)
            return;
        if (isGrounded && unit.goalTarget != null)
        {
            ////if the linecast hits a structure, then 
            //if(Physics2D.Linecast())
            Move();
        }
    }
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    protected virtual void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
    protected virtual void Move()
    {
        Vector2 dist = unit.goalTarget.position - transform.position;
        FlipSprite(dist);

        dist.y = 0; // ignore height differences
                    // calc a target vel proportional to distance (clamped to maxVel)
        Vector2 tgtVel = Vector2.ClampMagnitude(20 * dist, unit.GetInfo().MoveSpeed);
        // Vector2 the velocity error
        Vector2 error = tgtVel - rb.velocity;
        // calc a force proportional to the error (clamped to maxForce)
        Vector2 force = Vector2.ClampMagnitude(gain * error, maxForce);
        rb.AddForce(force);

        CheckIfMoving();
    }

    protected void CheckIfMoving()
    {
        if (rb.velocity.magnitude == Mathf.Epsilon)
        {
            unit.ChangeState(Unit.States.MOVING, true, false);
        }
        else
        {
            unit.ChangeState(Unit.States.MOVING, true, true);
        }
    }

    protected void FlipSprite(Vector2 dist)
    {
        bool isFlipped = dist.x < 0;
        if (isFacingRight != isFlipped && rb.velocity.magnitude != Mathf.Epsilon)
        {
            isFacingRight = isFlipped;
            Flipped(isFacingRight);
        }
    }
}
