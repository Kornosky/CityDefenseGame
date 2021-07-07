using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnableButton : CooldownButton<UnitScriptableObject>
{
    Action startCooldownAction;    
    public override void Init(UnitScriptableObject info)
    {
        base.Init(info);
        nameText.text = info.name;
        costText.text = info.cost + " C";
        image.sprite = info.sprite;
        cooldownPeriod = info.cooldownPeriod;
        startCooldownAction += () => StartCoroutine(Wait());
    }

    public override void OnPressed()
    {
        base.OnPressed();
        if(info.isStructure)
        {
            if (GameManager.Instance.CheckPurchase(info))
            {
                GameManager.Instance.TryPlacingUnit(info, startCooldownAction);
            }
        }
        else
        {
           if( GameManager.Instance.CheckPurchase(info))
            {
                GameManager.Instance.BuyUnit(info);
                StartCoroutine(Wait());
            }
        }
    }
}
