using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : SingletonDDOL<GameManager>
{
    private bool hasLoaded;
    public Action SaveData;
    public Action LoadData;
    public PlayerData data;

    [Header("Loading")]
    LoadingScreen loadingScreen;
    #region Debug
    [Header("Debug")]
    [SerializeField] private bool resetSaveInformation;
    #endregion
#if UNITY_IOS || UNITY_ANDROID
    //touch code
#else
    //non-touch code
#endif

    private void Start()
    {
        Application.targetFrameRate = 60;
        //Load 
        SaveSystem.LoadPlayer(ref data);

        //Initialize unlocks
        //UnlockablesManager.singleton.Initialize();
        //welcomeBackScreen.Initialize();
        #region Debug
        if (resetSaveInformation)
            data.FullReset();
        #endregion

        //Set all things with loaded data
        LoadData?.Invoke();
        hasLoaded = true;
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        //Do this after all PassiveIncomeSources have been loaded 
        //welcomeBackScreen.EvaluateAwayIncome();

        //Begin updating realtime
        //StartCoroutine(AssessSources());
    }

    [ContextMenu("Save")]
    public void Save()
    {
        if (!hasLoaded)
            return;
        SaveData?.Invoke();

        //Save time
        //TODO put this back
        SaveSystem.SavePlayer(data);
    }

    private void OnApplicationPause(bool pause)
    {
        Save();
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    public void ReturnToMap()
    {
        //load menus        
        AsyncOperation loadingOperation = SceneManager.LoadSceneAsync("Menus");

        //lazy load
        if (loadingScreen)
            loadingScreen.Begin(loadingOperation);
        else
            Instantiate(Resources.Load("LoadingScreen"), transform);

        MainMenuManager.Instance.SwitchCanvas(MainMenuManager.CanvasType.PLAY);
    }

    public void LoadLevel(LevelScriptableObject level)
    {
        //load main        
        AsyncOperation loadingOperation = SceneManager.LoadSceneAsync("main");
        Action<AsyncOperation> action = (async) => LevelManager.Instance.LoadLevel(level);
        loadingOperation.completed += action;
        //lazy load
        if (loadingScreen == null)
            loadingScreen = Instantiate(Resources.Load<GameObject>("LoadingScreen"), transform).GetComponent<LoadingScreen>();

        loadingScreen.Begin(loadingOperation);


        
    }

 
}
