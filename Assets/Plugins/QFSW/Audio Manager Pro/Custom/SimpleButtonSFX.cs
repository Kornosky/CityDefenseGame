using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Button))]
public class SimpleButtonSFX : MonoBehaviour
{
    [SerializeField] SFXObject sfx;
    private void Start()
    {
        Button button = GetComponentInChildren<Button>(true);
        button.onClick.AddListener(delegate { SFXManager.Main.Play(sfx); });
    }
}
