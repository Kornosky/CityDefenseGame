using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HitDamage : MonoBehaviour
{
    TextMeshProUGUI tmp;
    private float fontScaleAmt = 5;
    private float animationTime = 1f;
    Color originalColor;

    private void Awake()
    {
        tmp ??= GetComponent<TextMeshProUGUI>();
        originalColor = tmp.color;
    }
    public void Init(float amount)
    {       
        tmp.text = ((int)Mathf.Abs(amount)).ToString();
        tmp.fontSize *= 1 + (Mathf.Abs(amount) / fontScaleAmt);
        Tween.LocalPosition(transform.GetComponent<RectTransform>(), transform.position + new Vector3(0, 50, 0), animationTime, 0, Tween.EaseOutStrong, Tween.LoopType.None, null, () => Destroy(gameObject));
        Tween.Value(0f, 1f, HandleButtonWidthChange, animationTime, 0);
    }

    void HandleButtonWidthChange(float value)
    {
        tmp.color = Color.Lerp(originalColor, new Color(1,1,1,0), value);
    }
}
