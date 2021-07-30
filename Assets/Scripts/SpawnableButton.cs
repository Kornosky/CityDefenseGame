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
        costText.text = info.Cost + " C";
        image.sprite = info.sprite;
        cooldownPeriod = info.CooldownPeriod;
        startCooldownAction += () => StartCoroutine(Wait());
    }

    public override void OnPressed()
    {
        base.OnPressed();
        if(info.isStructure)
        {
            if (PlayerManager.Instance.CheckPurchase(info))
            {
                PlayerManager.Instance.TryPlacingUnit(info, startCooldownAction);
            }
        }
        else
        {
           if( PlayerManager.Instance.CheckPurchase(info))
            {
                PlayerManager.Instance.BuyUnit(info);
                StartCoroutine(Wait());
            }
        }
    }
}
