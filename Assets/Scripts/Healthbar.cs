using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pixelplacement;

public class Healthbar : UIBar
{
    [SerializeField] GameObject hitDamage;
    public override void Init(UnitScriptableObject info)
    {
        slider.maxValue = info.Health;
        slider.value = slider.maxValue;
        previousValue = (int)slider.maxValue;
        targetValue = (int)slider.maxValue;
    }

    public HealthBundle GetHealth()
    {
        return new HealthBundle(targetValue, slider.minValue, slider.maxValue);
    }
    protected override void UpdateEffect(float value)
    {
        HitDamage newDamage = Instantiate(hitDamage, transform.parent.transform).GetComponent<HitDamage>();
        newDamage.Init(value);

        Tween.Shake(transform, transform.position, Vector3.one * 3.2f, .9f, 0);
        Tween.Value(previousValue, (float)targetValue, ValueChange, .9f, 0, Tween.EaseOutStrong, Tween.LoopType.None);
    }
}
