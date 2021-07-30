using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityManager : SingletonDDOL<AbilityManager>
{
    public List<AbilityButton> buttons = new List<AbilityButton>();
    public void ResetButtons()
    {
        foreach( var but in buttons)
        {
            but.Deactivate();
        }
    }

    public void ChangeAbility(AbilityScriptableObject info)
    {
        info.onActionCompleted.RemoveAllListeners();
        PlayerManager.Instance.currentAbility = null;
        PlayerManager.Instance.currentAbility = info;
    }
}
