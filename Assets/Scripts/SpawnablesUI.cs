using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnablesUI : MonoBehaviour
{
    [SerializeField] GameObject spawnableUIprefab;
    public void Start()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        foreach(var unit in GameManager.Instance.availableUnits)
        {
            AddToGroup(unit);
        }
    }
    public void AddToGroup(UnitScriptableObject unit)
    {
        GameObject ui = Instantiate(spawnableUIprefab, transform);
        var buttonUI = ui.GetComponent<SpawnableButton>();
        buttonUI.Init(unit);
    }
}
