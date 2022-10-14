using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Coin for earning money. They are placing phyiscally in front (z pos) for OnMouseOver()
/// </summary>
public class Coin : Pickup
{
    protected override void Start()
    {
        base.Start();
        goalTransform = PlayerManager.Instance.moneyText.transform;
    }
    protected override void OnCollect()
    {
        SFXManager.Main.PlayFromSFXObjectLibrary(string.Format("Coin_OnCollect"));

        PlayerManager.Instance.Money += 1;

        base.OnCollect();
    }
}
