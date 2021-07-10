using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityButton : CooldownButton<AbilityScriptableObject>
{
    bool isActive;
    public override void Init(AbilityScriptableObject info)
    {
        base.Init(info);
        nameText.text = info.name;
        costText.text = info.manaCost + " M";
        image.sprite = info.icon;
        cooldownPeriod = info.cooldownPeriod;
    }

    public override void OnPressed()
    {
        if (isActive)
        {
            isActive = false;
            button.interactable = true;
            info.onActionCompleted.RemoveAllListeners();
            PlayerManager.Instance.currentAbility = null;
            return;
        }

        base.OnPressed();
        
        //Can purchase
        if (PlayerManager.Instance.CheckCast(info))
        {
            info.Begin();

            isActive = true;
            button.interactable = false;

            PlayerManager.Instance.currentAbility = info;
            info.onActionCompleted.AddListener(() => StartCoroutine(Wait()));
        }
    }
    protected override void AfterCD()
    {
        base.AfterCD();
        PlayerManager.Instance.currentAbility = null;
        isActive = false;
    }
}
