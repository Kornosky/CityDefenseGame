using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Loads and handles the level based off a profile
/// </summary>
public class LevelManager : Singleton<LevelManager>
{
    LevelScriptableObject info;

    public void LoadLevel(LevelScriptableObject info)
    {
        //INit player and enemy manager
        PlayerManager.Instance.Init(info);
        EnemyManager.Instance.Init(info);
    }


}
