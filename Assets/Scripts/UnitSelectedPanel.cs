using Pixelplacement;
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
    [SerializeField] Button backButton;
    [SerializeField] RectTransform background;
    [SerializeField] VerticalLayoutGroup verticalLayoutGroup;
    GameObject upgradeButtonPrefab;

    protected override void Awake()
    {
        base.Awake();
        backButton.onClick.AddListener(()=> Visibility(false));
        upgradeButtonPrefab = Resources.Load<GameObject>("UpgradeButton");
    }
    public void Open(UnitScriptableObject info)
    {
        this.info = info;
        unitName.text = info.name;
        //Destroy any existing
        foreach(Transform child in verticalLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }
        //Create upgrade buttons
        foreach(var up in info.upgrades)
        {
            var temp = Instantiate(upgradeButtonPrefab, verticalLayoutGroup.transform);
            temp.GetComponent<UpgradeButton>().Init(info, up);
        }
    }

    public override void Visibility(bool isVisible, bool willFade = false, float duration = .3f)
    {
        base.Visibility(isVisible);

        Tween.AnchoredPosition(background, new Vector2(background.rect.width, 0), new Vector2(0, 0), .1f, 0, Tween.EaseOutBack, Tween.LoopType.None, null, null);
    }
}
