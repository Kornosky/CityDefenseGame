using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/// <summary>
/// Level prefab for map
/// </summary>
public class LevelMapView : MonoBehaviour
{
    [SerializeField] TMP_Text levelNumber;
    LevelScriptableObject info;

    public void Init(LevelScriptableObject info)
    {
        this.info = info;
        levelNumber.text = info.levelNumber.ToString();
    }
    private void OnMouseDown()
    {
        GameManager.Instance.LoadLevel(info);
    }
}
