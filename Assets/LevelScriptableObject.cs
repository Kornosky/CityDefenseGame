using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "LevelInfo", menuName = "ScriptableObjects/LevelInfo", order = 1)]

public class LevelScriptableObject : ScriptableObject
{
    public int levelNumber;
    public new string name;
    public int difficulty;
    public UnitScriptableObject[] units;
    [Header("Visuals")]
    public Sprite nexus;
    public Sprite background;
    public Sprite foreground;
    [Header("Story")]
    public string placeholder;
    [Header("Meta")]
    public bool isBeat;

    //Maybe i don't need this? Just a dict of lvNum and bool
    public void Load()
    {
        if (GameManager.Instance.data.unitScriptableObjects.ContainsKey(name))
            JsonUtility.FromJsonOverwrite(GameManager.Instance.data.unitScriptableObjects[name], this);

    }
}