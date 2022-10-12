using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/LevelInfo", order = 1)]

public class LevelScriptableObject : ScriptableObject
{
    public int levelNumber;
    public string levelName;
    public int difficulty;
    public UnitScriptableObject[] units;
    [Header("Visuals")]
    public Sprite nexus;
    public Sprite background;
    public Sprite foreground;
    [Header("Story")]
    public DialogueInfo dialogueInfo;
    [Header("Meta")]
    public bool isBeat;


    private void OnValidate()
    {
        try
        {
            levelNumber = Int32.Parse(Regex.Match(name, @"\d+").Value);
        }
        catch(Exception e)
        {
            Debug.Log(e);
            levelNumber = 0;
        }
        
    }
    //Maybe i don't need this? Just a dict of lvNum and bool
    public void Load()
    {
        if (GameManager.Instance.Data.unitScriptableObjects.ContainsKey(name))
            JsonUtility.FromJsonOverwrite(GameManager.Instance.Data.unitScriptableObjects[name], this);

    }
}
