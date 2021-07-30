using System;
using System.Collections.Generic;

/// <summary>
/// Manages all player data
/// </summary>

[System.Serializable]
public class PlayerData 
{
    public float experience;
    public int rareCurrency;
    public int money;
    public int levelProgress;
    public Dictionary<string, string> unitScriptableObjects  = new Dictionary<string, string>();
    public PlayerData()
    {
       // FullReset();
    }

    public void FullReset()
    {
        money = 0;
        experience = 0;
        rareCurrency = 0;
        levelProgress = 0;
        unitScriptableObjects.Clear();
    }
}
