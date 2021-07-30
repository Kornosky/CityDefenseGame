using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OverlayCanvas : MenuElement
{
    [SerializeField] TMP_Text goldAmountTMP;
    [SerializeField] TMP_Text experienceTMP;
    [SerializeField] TMP_Text rareTMP;

    protected override void Awake()
    {
        base.Awake();        
    }

    protected override void Start()
    {
        base.Start();
        GameManager.Instance.LoadData += ConnectStats;
    }
    //This code keeps being called before GameManager exists
    public void ConnectStats()
    {
        GameManager.Instance.updateStatsAction += UpdateStats;
        if(GameManager.Instance.Data != null)
        {
            UpdateStats();
        }
    }
    public void UpdateStats()
    {
        goldAmountTMP.text = GameManager.Instance.Data.money + " G";
        experienceTMP.text = GameManager.Instance.Data.experience + " EXP";
        rareTMP.text = GameManager.Instance.Data.rareCurrency + " L";
    }


    private void OnDestroy()
    {
        if(GameManager.Instance)
        {
            GameManager.Instance.updateStatsAction -= UpdateStats;
        }
    }
}
