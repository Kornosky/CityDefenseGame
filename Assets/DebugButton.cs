using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugButton : MonoBehaviour
{
    void Pressed()
    {
        GameManager.Instance.isDebug = !GameManager.Instance.isDebug;
        GetComponent<Image>().color = GameManager.Instance.isDebug ? Color.grey : Color.white;
    }
}
