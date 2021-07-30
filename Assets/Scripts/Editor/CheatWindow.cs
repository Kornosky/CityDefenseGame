using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(CheatWindow))]
[CanEditMultipleObjects]
public class CheatWindow : EditorWindow
{

    PlayerData data = null;

    // Add menu item named "My Window" to the Window menu
    [MenuItem("Window/Cheat Window")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(CheatWindow));
    }
    private void Awake()
    {
        SaveSystem.LoadPlayer(ref data);
    }
    void OnGUI()
    {
        GUILayout.Label("Player Data", EditorStyles.boldLabel);

        DisplayFields<PlayerData>(data);
       

        if(GUI.changed)
        {
            SaveSystem.SavePlayer(data);
            if (GameManager.Instance != null)
                GameManager.Instance.Data = data;
            GameManager.Instance?.updateStatsAction.Invoke();
        }
    }

    void DisplayFields<T>( object obj)
    {

        foreach (var prop in typeof(T).GetFields())
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox);

            var type = prop.FieldType;
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Int32:
                    // It's an int
                    prop.SetValue(obj, EditorGUILayout.IntField(prop.Name, Convert.ToInt32(prop.GetValue(obj))));

                    break;
                case TypeCode.Single:
                    // It's an int
                    prop.SetValue(obj, EditorGUILayout.FloatField(prop.Name, Convert.ToSingle(prop.GetValue(obj))));

                    break;

                case TypeCode.String:
                    // It's a string
                    prop.SetValue(obj, EditorGUILayout.TextField(prop.Name, Convert.ToString(prop.GetValue(obj))));
                    break;

                // Other type code cases here...

                default:
                    // Fallback to using if-else statements...
                    if (type == typeof(GameObject))
                    {
                        // ...
                    }
                    break;
            }

            EditorGUILayout.EndHorizontal();
        }
    }
    private void OnValidate()
    {
        
    }
}