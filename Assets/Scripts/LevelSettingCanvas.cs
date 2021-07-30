using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSettingCanvas : MenuElement
{
    public void OnResume()
    {
        Visibility(false);
        LevelManager.Instance.Pause(false);
    }
    public void OnOptions()
    {

    }
    public void OnQuit()
    {
        LevelManager.Instance.UnloadLevel();
    }
}
