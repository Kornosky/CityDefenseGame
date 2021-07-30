using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MainMenuManager : Singleton<MainMenuManager>
{
    [SerializeField] public enum CanvasType { MAINMENU, UPGRADE, PLAY, LOOT }
    [SerializeField] MainMenuCanvas mainMenuCanvas;
    [SerializeField] UpgradeCanvas upgradeCanvas;
    [SerializeField] PlayCanvas playCanvas;
    [SerializeField] OverlayCanvas overlayCanvas;
    [SerializeField] LootboxCanvas lootCanvas;
    [SerializeField] CutsceneManager cutsceneManager;

    [SerializeField] List<MenuElement> canvases = new List<MenuElement>();

    public UpgradeCanvas UpgradeCanvas { get => upgradeCanvas; set => upgradeCanvas = value; }
    public PlayCanvas PlayCanvas { get => playCanvas; set => playCanvas = value; }
    public MainMenuCanvas MainMenuCanvas { get => mainMenuCanvas; set => mainMenuCanvas = value; }
    public LootboxCanvas LootCanvas { get => lootCanvas; set => lootCanvas = value; }

    private void Reset()
    {
        mainMenuCanvas ??= FindObjectOfType<MainMenuCanvas>();
        upgradeCanvas ??= FindObjectOfType<UpgradeCanvas>();
        playCanvas ??= FindObjectOfType<PlayCanvas>();
        overlayCanvas ??= FindObjectOfType<OverlayCanvas>();
        LootCanvas ??= FindObjectOfType<LootboxCanvas>();
        cutsceneManager ??= FindObjectOfType<CutsceneManager>();

        var properties = GetType().GetProperties();
        foreach (var field in properties) //get all properties derived by menuelement
        {
            var cat = field.PropertyType;
            if (typeof(MenuElement).IsAssignableFrom(cat.BaseType))
                canvases.Add(field.GetValue(this) as MenuElement);
        }
    }
    protected override void Awake()
    {
        base.Awake();
        //var properties = GetType().GetProperties();

        //foreach (var field in properties)
        //{
        //    var cat = field.GetType();
        //    if (typeof(MenuElement).IsAssignableFrom(field.GetType()))
        //        canvases.Add(field.GetValue(this) as MenuElement);
        //}
    }
    private void Start()
    {
        SwitchCanvas(CanvasType.MAINMENU, true);

        overlayCanvas.ConnectStats();
    }
    public void SwitchCanvas(CanvasType type, bool overlayActive = true)
    {
        foreach (var can in canvases) can.Visibility(false);

        switch (type) //maybe this can be better?
        {
            case CanvasType.MAINMENU:
                MainMenuCanvas.Visibility(true);
                break;
            case CanvasType.UPGRADE:
                UpgradeCanvas.Visibility(true);
                break;
            case CanvasType.PLAY:
                PlayCanvas.Visibility(true);
                break;
            case CanvasType.LOOT:
                LootCanvas.Visibility(true);
                break;
        }
        if (overlayActive)
            overlayCanvas.Visibility(overlayActive);
    }
    public void SwitchCanvas(string str)
    {
        SwitchCanvas((CanvasType)Enum.Parse(typeof(CanvasType), str), true);
        
    }
}
