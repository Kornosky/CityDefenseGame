using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Unit is selected, now show their stats
/// </summary>
public class UnitSelectedPanel : MenuElement
{
    UnitScriptableObject info;
    [SerializeField] TMP_Text unitName;
    [SerializeField] TMP_Text health;
    [SerializeField] TMP_Text damage;
    [SerializeField] Button backButton;

    protected override void Awake()
    {
        base.Awake();
        backButton.onClick.AddListener(()=> Visibility(false));
    }
    public void Open(UnitScriptableObject info)
    {
        this.info = info;
        unitName.text = info.name;
        health.text = info.health.ToString();
        damage.text = info.damage.ToString();
    }
}
