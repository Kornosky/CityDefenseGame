using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Loads and handles the level based off a profile
/// </summary>
public class LevelManager : Singleton<LevelManager>
{
    LevelScriptableObject info;
    [SerializeField] LevelSettingCanvas levelSettingsCanvas;
    public event Action<LevelState, LevelState> OnGameStateChange;
    LevelState previousState;
    LevelState currentState;
    [Header("Debug")]
    public bool isDebug;
    public enum LevelState
    {
        Unpaused,
        Paused
    }
    public void SwitchGameState(LevelState to)
    {
        previousState = currentState;
        currentState = to;

        switch (to)
        {
            case LevelState.Unpaused:
                break;
            case LevelState.Paused:
                break;
        }

        OnGameStateChange?.Invoke(previousState, currentState);
    }
    public void LoadLevel(LevelScriptableObject info)
    {
        //INit player and enemy manager
        PlayerManager.Instance.Init(info);
        EnemyManager.Instance.Init(info);
        CameraController.Instance.cam = Camera.main;
        CameraController.Instance.ChangeCameraBounds(new GameObject[] { PlayerManager.Instance.playerBase.gameObject, PlayerManager.Instance.enemyBase.gameObject });
    }
    protected override void Awake()
    {
        base.Awake();
        if(isDebug)
        {
            LoadLevel(Resources.Load<LevelScriptableObject>("Levels/DebugLevel"));
        }
    }
    public void UnloadLevel()
    {
        //Check if level was won
        if(info)//for debug
        info.isBeat = PlayerManager.Instance.levelWin;
        //maybe show another screen here before returning
        //Unpause
        Pause(false);
        GameManager.Instance.ReturnToMap();
    }
    public void Pause(bool isPaused)
    {
        Time.timeScale = isPaused ? 0 : 1;
    }
    public void DisplaySettings(bool isDisplayed)
    {
        Pause(isDisplayed);
        levelSettingsCanvas.Visibility(isDisplayed);
    }

}
