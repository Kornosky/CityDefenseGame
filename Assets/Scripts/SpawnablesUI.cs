using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnablesUI : MonoBehaviour
{
    [SerializeField] GameObject spawnableUIprefab;
    List<Transform> menus = new List<Transform>();
    [SerializeField] RectTransform structures;
    [SerializeField] RectTransform units;
    string currentMenu;
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
        foreach (var menu in menus) menu.gameObject.SetActive(false);
    }
    public void AddToGroup(UnitScriptableObject unit)
    {
        GameObject ui = unit as StructureScriptableObject == null? Instantiate(spawnableUIprefab, structures) : Instantiate(spawnableUIprefab, units);
        var buttonUI = ui.GetComponent<SpawnableButton>();
        buttonUI.Init(unit);
    }

    public void Switch(string n)
    {
        //deactivate all if active
        foreach (var menu in menus) if(menu.gameObject.activeSelf) menu.gameObject.SetActive(false);
        //if it's the same button pressed, turn off all
        if (currentMenu == n)
            return;
        switch(n)
        {
            case "Unit":
                units.gameObject.SetActive(true);
                break;
            case "Struct":
                structures.gameObject.SetActive(true);
                break;
            case "Misc":
                break;
        }
    }
}
