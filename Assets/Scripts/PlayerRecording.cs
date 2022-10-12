using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Records the players stats
/// </summary>
public class PlayerRecording : SingletonDDOL<PlayerRecording>
{
    [SerializeField] private int moneySpentTotal;
    [SerializeField] private int moneyEarnedTotal;
    [SerializeField] private Dictionary<UnitScriptableObject, int> unitSpawnCount = new Dictionary<UnitScriptableObject, int>();
    [SerializeField] private Dictionary<UnitScriptableObject, int> activeUnits = new Dictionary<UnitScriptableObject, int>();
    
    public int MoneySpentTotal { get => moneySpentTotal; set => moneySpentTotal = value; }
    public int MoneyEarnedTotal { get => moneyEarnedTotal; set => moneyEarnedTotal = value; }
    public Dictionary<UnitScriptableObject, int> UnitSpawnCount { get => unitSpawnCount; set => unitSpawnCount = value; }
    public Dictionary<UnitScriptableObject, int> ActiveUnits { get => activeUnits; set => activeUnits = value; }

    public void AddUnitsToDictionary(Dictionary<UnitScriptableObject, int> dict, UnitScriptableObject unit, int value)
    {
        return;
        if (dict.ContainsKey(unit))
            dict[unit] += 1;
        else
            dict.Add(unit, dict.Count + value);
    }
    public int GetTotalInDictionary(Dictionary<UnitScriptableObject, int> dict)
    {
        int count = 0;
        foreach(var unit in dict)
        {
            count += unit.Value;
        }
        return count;
    }
}
