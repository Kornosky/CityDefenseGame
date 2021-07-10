using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MainMenuManager : Singleton<MainMenuManager>
{
    [SerializeField] public enum CanvasType { MAINMENU, UPGRADE, PLAY }
    [SerializeField] MainMenuCanvas mainMenuCanvas;
    [SerializeField] UpgradeCanvas upgradeCanvas;
    [SerializeField] PlayCanvas playCanvas;

    [SerializeField] List<MenuElement> canvases = new List<MenuElement>();
    
    public UpgradeCanvas UpgradeCanvas { get => upgradeCanvas; set => upgradeCanvas = value; }
    public PlayCanvas PlayCanvas { get => playCanvas; set => playCanvas = value; }
    public MainMenuCanvas MainMenuCanvas { get => mainMenuCanvas; set => mainMenuCanvas = value; }

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
        SwitchCanvas(CanvasType.MAINMENU);
    }
    public void SwitchCanvas(CanvasType type)
    {
        foreach (var can in canvases) can.Visibility(false);

        switch (type) //maybe this can be better?
        {
            case CanvasType.MAINMENU:
                mainMenuCanvas.Visibility(true);
                break;
            case CanvasType.UPGRADE:
                upgradeCanvas.Visibility(true);
                break;
            case CanvasType.PLAY:
                PlayCanvas.Visibility(true);
                break;
        }        
    }
    public void SwitchCanvas(string str)
    {
        CanvasType type = (CanvasType) Enum.Parse(typeof(CanvasType), str);
        foreach (var can in canvases) can.Visibility(false);

        switch (type) //maybe this can be better?
        {
            case CanvasType.MAINMENU:
                mainMenuCanvas.Visibility(true);
                break;
            case CanvasType.UPGRADE:
                upgradeCanvas.Visibility(true);
                break;
            case CanvasType.PLAY:
                PlayCanvas.Visibility(true);
                break;
        }        
    }
}
