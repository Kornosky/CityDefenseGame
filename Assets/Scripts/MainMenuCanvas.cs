using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Show player data
/// </summary>
public class MainMenuCanvas : MenuElement
{

    public void Cheat()
    {
        GameManager.Instance.data.money = 10000;
        GameManager.Instance.data.experience= 10000;
        GameManager.Instance.data.rareCurrency= 10000;
        GameManager.Instance.updateStatsAction.Invoke();
    }
}
