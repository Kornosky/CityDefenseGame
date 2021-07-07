using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pixelplacement;

public class BuildBar : UIBar
{
    public override void Init(UnitScriptableObject info)
    {
        slider.maxValue = info.buildTime;
        slider.value = 0;
        previousValue = 0;
        targetValue = 0;
    }

    protected override void UpdateEffect(float value)
    {
        Tween.Value(previousValue, (float)targetValue, ValueChange, .9f, 0, Tween.EaseOutStrong, Tween.LoopType.None);
    }
}
