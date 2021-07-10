using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Jump", menuName = "Ability/Jump", order = 0)]
public class JumpAbility : AbilityScriptableObject
{
    public int jumpForce;
    [SerializeReference] float shakeAmount = 1.1f;
    [SerializeReference] float shakeLength = .4f;
    public override void Init()
    {
      //  throw new System.NotImplementedException();
    }

    protected override void Effect()
    {
        Collider2D hit = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (hit && !hit.isTrigger && hit.attachedRigidbody != null)
        {
            hit.attachedRigidbody.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            OnStarted();
        }
        else
        {
            //do nothing yet
        }
    }

    protected override void OnFinished()
    {
        //Effects
        if(vfxPrefab)
        Instantiate(vfxPrefab, unit.transform.position, Quaternion.identity);

        CameraController.Instance.CameraShake(shakeLength, shakeAmount);

        OnCompleted();
    }
}
