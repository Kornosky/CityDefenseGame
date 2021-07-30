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
    public enum CurrencyType { RARE, GOLD }
    public delegate void UpdateStats();
    public UpdateStats updateStatsAction;
    [Header("Loading")]
    LoadingScreen loadingScreen;
    #region Debug
    [Header("Debug")]
    public bool isDebug;
    [SerializeField] public bool resetSaveInformation;

    public PlayerData Data { get => data; set { data = value; updateStatsAction.Invoke(); } }

    #endregion
#if UNITY_IOS || UNITY_ANDROID
    //touch code
#else
    //non-touch code
#endif

    void Awake()
    {
        Application.targetFrameRate = 60;
        //Load 
        data = new PlayerData();

        LoadPlayer();
        Debug.Log("ALERT ALERT " + data);
        //Set all things with loaded data
        LoadData?.Invoke();
        //this is not great
        updateStatsAction?.Invoke();

        hasLoaded = true;
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        //Do this after all PassiveIncomeSources have been loaded 
        //welcomeBackScreen.EvaluateAwayIncome();
        //Begin updating realtime
        //StartCoroutine(AssessSources());
        loadingScreen = Instantiate(Resources.Load<GameObject>("LoadingScreen"), transform).GetComponent<LoadingScreen>();
    }
    private void LoadPlayer()
    {
        SaveSystem.LoadPlayer(ref data);
        //Generate new random seed if after loading data, the seed was 0
        if (data.randomSeed == 0)
            data.randomSeed = UnityEngine.Random.Range(1, 111);
    }
    private void Start()
    {
        #region Debug
        if (resetSaveInformation)
        {
            Data.FullReset();
            foreach (var so in Resources.LoadAll<UnitScriptableObject>("Units")) 
            {
                so.ResetData();
            }
        }
        #endregion
    }
    [ContextMenu("Reset")]
    public void ResetData()
    {
        Data.FullReset();
        foreach (var so in Resources.LoadAll<UnitScriptableObject>("Units"))
        {
            so.ResetData();
        }
        LoadData?.Invoke();
        updateStatsAction.Invoke();
    }
    [ContextMenu("Save")]
    public void Save()
    {
        if (!hasLoaded)
            return;
        SaveData?.Invoke();

        //Save time
        //TODO put this back
        SaveSystem.SavePlayer(Data);
    }

    public bool TryPurchase(int cost, CurrencyType currency)
    {
        if (isDebug)
            return true;
        switch (currency)
        {
            case CurrencyType.RARE:
                if (Data.rareCurrency - cost >= 0)
                {
                    Data.rareCurrency -= cost;
                    updateStatsAction.Invoke();
                    return true;
                }
                break;
            case CurrencyType.GOLD:
                if (Data.money - cost >= 0)
                {
                    Data.money -= cost;
                    updateStatsAction.Invoke();
                    return true;
                }
                break;
        }

 
        return false;
    }
    private void OnApplicationPause(bool pause)
    {
    #if !UNITY_EDITOR
            Save();
    #endif
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    public void ReturnToMap()
    {
        //load menus        
        AsyncOperation loadingOperation = SceneManager.LoadSceneAsync("Menus");

        loadingScreen.Begin(loadingOperation);
        loadingOperation.completed += LoadingOperation_completed;
    }

    private void LoadingOperation_completed(AsyncOperation obj)
    {
        MainMenuManager.Instance.SwitchCanvas(MainMenuManager.CanvasType.PLAY);
        CameraController.Instance.RefreshReferences();
    }

    public void LoadLevel(LevelScriptableObject level)
    {
        //load main        
        CutsceneManager.Instance.BeginScene(level.dialogueInfo);
        AsyncOperation loadingOperation = SceneManager.LoadSceneAsync("Level");
        Action<AsyncOperation> action = (async) => LevelManager.Instance.LoadLevel(level);
        loadingOperation.completed += action;
        //lazy load

        loadingScreen.Begin(loadingOperation);        
    }


}
