using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pixelplacement;

public abstract class UIBar : MonoBehaviour
{
    protected Slider slider;
    protected float previousValue = 0;
    [SerializeField] protected float targetValue = 0;

    private void Awake()
    {
        slider = GetComponent<Slider>();        
    }
    public abstract void Init(UnitScriptableObject info);
    public bool IsComplete()
    {
        return targetValue >= slider.maxValue;
    }
    public void UpdateValue(float change)
    {
        targetValue = Mathf.Clamp(targetValue + change, 0, slider.maxValue);
        //add effects
        UpdateEffect(change);
        previousValue = targetValue;
    }

    protected abstract void UpdateEffect(float value);
    protected void ValueChange(float value)
    {
        slider.value = value;
    }
    public float GetValue()
    {
        return targetValue;
    }    
}
