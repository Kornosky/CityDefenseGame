using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCollectionPanel : MenuElement
{
    [SerializeField] GameObject buttonPrefab;
    public void Start()
    {
        foreach(UnitScriptableObject unit in Resources.LoadAll<UnitScriptableObject>("Units"))
        {
            var temp = Instantiate(buttonPrefab, transform).GetComponent<UnitShopButton>();
            temp.Init(unit);
        }
    }
}
