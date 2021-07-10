using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// For selecting unit and bringing up upgrades
/// </summary>
public class UnitShopButton : MenuElement
{
    [SerializeField] bool wasPressed;
    [SerializeField] Button button;
    [SerializeField] Image backgroundImage;
    [SerializeField] Image spriteImage;
    UnitScriptableObject info;

    protected override void Awake()
    {
        base.Awake();
        button.onClick.AddListener(OnPressed);
        Visibility(true);
    }
    public void Init(UnitScriptableObject info)
    {
        this.info = info;
        spriteImage.sprite = info.sprite;
        Unlocked(info.isUnlocked);
    }

    void Unlocked(bool isUnlocked)
    {
        if(isUnlocked)
        {
            backgroundImage.color = Color.white;
            spriteImage.color = Color.white;
        }
        else
        {
            backgroundImage.color = Color.grey;
            spriteImage.color = Color.black;
        }
    }

    void OnPressed()
    {
        if(wasPressed)
        {
            MainMenuManager.Instance.UpgradeCanvas.OpenUnitSelectedPanel(info);  
        }
        else
        {
            wasPressed = true;
            MainMenuManager.Instance.UpgradeCanvas.ChangeUnitFocus(info);
            StopAllCoroutines();
            StartCoroutine("Wait");
        }
    }
    IEnumerator Wait()
    {
        //Stuff before waiting
        yield return new WaitForSeconds(1f);
        wasPressed = false;
    }
}
