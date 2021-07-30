using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnablesUI : MonoBehaviour
{
    [SerializeField] GameObject spawnableUIprefab;
    List<Transform> menus = new List<Transform>();
    [SerializeField] RectTransform structures;
    [SerializeField] RectTransform units;
    private void Awake()
    {
        menus.Add(structures);
        menus.Add(units);
    }
    public void Start()
    {
        
        foreach(var unit in PlayerManager.Instance.availableUnits)
        {
            AddToGroup(unit);
        }
    }
    public void AddToGroup(UnitScriptableObject unit)
    {
        GameObject ui = unit.isStructure ? Instantiate(spawnableUIprefab, structures) : Instantiate(spawnableUIprefab, units);
        var buttonUI = ui.GetComponent<SpawnableButton>();
        buttonUI.Init(unit);
    }

    public void Switch(string n)
    {
        foreach (var menu in menus) menu.gameObject.SetActive(false);
        switch(n)
        {
            case "Unit":
                structures.gameObject.SetActive(true);
                break;
            case "Struct":
                units.gameObject.SetActive(true);
                break;
            case "Misc":
                break;
        }
    }
}
