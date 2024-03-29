using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flying : Movement
{
    public float hoverHeight = 2;
    public float hoverForce = 2;
    public float pulseTime = 1;
    float pulse;
   protected override void FixedUpdate()
    {
        //Hover code
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up, hoverHeight, LayerMask.NameToLayer("PlacingCollider"));
        float proportionalHeight = (hoverHeight - hit.distance) / hoverHeight;
        Vector3 appliedHoverForce = Vector3.up * proportionalHeight * hoverForce;
        pulse -= Time.fixedDeltaTime;

        if (hit && pulse <= Mathf.Epsilon)
        {
            pulse = pulseTime;
            rb.AddForce(new Vector2(0, hoverForce + Random.Range(0,.3f)), ForceMode2D.Impulse);
            if (!unit.GetInfo().canMoveWhileActing && unit.isActing)
                return;

            Move();
            
        }
     
    }

    protected override void Move()
    {
        //If there is no goal target... There should always be one tho
        if (unit.goalTarget == null)
            return;

        Vector2 dist = unit.goalTarget.position - transform.position;
        FlipSprite(dist);

        dist = dist.normalized;
        dist.y = 0; // ignore height differences
                    // calc a target vel proportional to distance (clamped to maxVel)
        Vector2 tgtVel = Vector2.ClampMagnitude(20 * dist, unit.GetInfo().MoveSpeed);
        // Vector2 the velocity error
        Vector2 error = tgtVel - rb.velocity;
        // calc a force proportional to the error (clamped to maxForce)
        Vector2 force = Vector2.ClampMagnitude(gain * error, maxForce);

        bool one = rb.velocity.x >= Mathf.Epsilon;
        bool two = dist.x >= Mathf.Epsilon;
        //if not on course to target, use additional thrust
        if (!(one ^ !two))
        {
            rb.AddForce(new Vector2(dist.x * unit.GetInfo().MoveSpeed * 2, 0), ForceMode2D.Impulse);
        }
        else
        {
             rb.AddForce(new Vector2(dist.x * unit.GetInfo().MoveSpeed, 0), ForceMode2D.Impulse);
        }


        CheckIfMoving();
    }
}
