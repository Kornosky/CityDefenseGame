using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButton : CooldownButton<AbilityScriptableObject>
{
    bool isActive;
    private void Start()
    {
        AbilityManager.Instance.buttons.Add(this);
    }
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
        AbilityManager.Instance.ResetButtons();

        //pressed again while active
        if (isActive)
        {
            // Deactivate
            AbilityManager.Instance.ChangeAbility(null);
            SFXManager.Main.PlayFromSFXObjectLibrary(string.Format("AbilityButton_OnPressed_Deactivate_{0}",info.abilityName)); 
            Deactivate();
            return;
        }

        base.OnPressed();
        
        //Can purchase
        if (PlayerManager.Instance.CheckCast(info))
        {
            info.Begin();
            SFXManager.Main.PlayFromSFXObjectLibrary(string.Format("AbilityButton_OnPressed_Activate_{0}", info.abilityName));
            Activate();
            AbilityManager.Instance.ChangeAbility(info);
            info.onActionCompleted.AddListener(() => StartCoroutine(Wait()));
        }
    }

    public void Activate()
    {
        if (isActive)
            return;
        isActive = true;
        gameObject.GetComponent<Image>().color = Color.grey;
    }

    public void Deactivate()
    {
        if (!isActive)
            return;
        isActive = false;
        gameObject.GetComponent<Image>().color = Color.white;

    }
    private void OnDestroy()
    {
        AbilityManager.Instance?.buttons.Remove(this);
    }
    protected override void AfterCD()
    {
        base.AfterCD();
        PlayerManager.Instance.currentAbility = null;
        isActive = false;
    }
}
