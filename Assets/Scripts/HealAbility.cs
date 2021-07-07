using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "HealAbility", menuName = "Ability/HealAbility", order = 0)]
public class HealAbility : AbilityScriptableObject
{
    public int healAmount;
    [SerializeReference] float shakeAmount = 1.1f;
    [SerializeReference] float shakeLength = .4f;
    public override void Init()
    {
      //  throw new System.NotImplementedException();
    }

    protected override void Effect()
    {
        Collider2D hit = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (hit && !hit.isTrigger && !hit.GetComponent<Unit>().isEnemy)
        {                    
           hit.GetComponent<IDamageable>().TakeDamage(-healAmount);
            OnStarted();
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
