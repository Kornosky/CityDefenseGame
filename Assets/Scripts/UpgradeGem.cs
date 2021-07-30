using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeGem : MonoBehaviour
{
    [SerializeField] GameObject activeGem;
    [SerializeField] GameObject inactiveGem;
    public void ActivateGem(bool isActive)
    {
        activeGem.SetActive(isActive);
    }
}
