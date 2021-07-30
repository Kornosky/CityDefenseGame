using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeCanvas : MenuElement
{
    [SerializeField] UnitSelectedPanel unitSelectedPanel;
    [SerializeField] UnitCollectionPanel unitCollectionPanel;
    [SerializeField] UnitArea unitArea;

    private void Start()
    {
        unitCollectionPanel.Visibility(true);
    }

    public override void Visibility(bool isVisible, bool willFade = false, float duration = .3f)
    {
        base.Visibility(isVisible);
        unitArea.Visibility(isVisible);
    }
    public void OpenUnitSelectedPanel(UnitScriptableObject info)
    {
        //init and open
        unitSelectedPanel.Open(info);
        unitSelectedPanel.Visibility(true);
    }

    public void CloseUnitSelectedPanel()
    {
        unitSelectedPanel.Visibility(false);
    }

    public void ChangeUnitFocus(UnitScriptableObject info)
    {
        unitArea.Open(info);
    }

}
