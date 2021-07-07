using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class CooldownButton<T> : MonoBehaviour 
{
    public Button button;    
    public Image image;
    public TextMeshProUGUI nameText; 
    public TextMeshProUGUI costText; 
    [Header("Runtime")]
    public T info;
    private float timer;
    protected float cooldownPeriod;
    public Image cdOverlaySlider;
    public TextMeshProUGUI cdOVerlayTimerText;

    public virtual void Init(T info)
    {
        this.info = info;
        button.onClick.AddListener(OnPressed);  
        ResetCDOverlay();
    }

    private void ResetCDOverlay()
    {
        cdOverlaySlider.fillAmount = 0;
        cdOVerlayTimerText.text = "";
        button.interactable = true;
    }
    public virtual void OnPressed()
    {
       
        
    }
    protected IEnumerator Wait()
    {
        timer = cooldownPeriod;
        button.interactable = false;
        while(timer > 0)
        {
            timer -= Time.deltaTime;
            cdOverlaySlider.fillAmount=  timer / cooldownPeriod;
            cdOVerlayTimerText.text = (Mathf.CeilToInt(timer)).ToString();
            yield return new WaitForEndOfFrame();
        }
        AfterCD();
        //  isAttacking = false;
    }
    protected virtual void AfterCD()
    {
        ResetCDOverlay();
    }
}
