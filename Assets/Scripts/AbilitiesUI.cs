using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesUI : MonoBehaviour
{
    [SerializeField] GameObject spawnableUIprefab;
    public void Start()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        foreach (var unit in PlayerManager.Instance.availableAbilities)
        {
            AddToGroup(unit);
        }
    }
    public void AddToGroup(AbilityScriptableObject unit)
    {
        GameObject ui = Instantiate(spawnableUIprefab, transform);
        var buttonUI = ui.GetComponent<AbilityButton>();
        buttonUI.Init(unit);
    }
}
