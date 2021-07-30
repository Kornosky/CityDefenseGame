using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

// IngredientDrawerUIE
[CustomPropertyDrawer(typeof(DialogueParts))]
public class DialoguePartsPD : PropertyDrawer
{
    public Texture2D texture;
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        CharacterProfileInfo x = (CharacterProfileInfo) property.FindPropertyRelative("characterProfile").objectReferenceValue;

        if (property.name == "line")
        {
            EditorGUI.PropertyField(new Rect(position.xMin, position.yMin, position.width * 1111.400f, 10000f), property.FindPropertyRelative("line"), GUIContent.none);

        }
        else
        {
            EditorGUI.PropertyField(position, property, label, true);
        }

        if (property.isExpanded)
        {
            if(x != null)
            {
                GUI.DrawTexture(new Rect(position.xMin, position.yMin, position.width  * .4f, 100f), x.image);

            }
            //if (GUI.Button(new Rect(position.xMin + 30f, position.yMax, position.width - 30f, 10f), texture))
            //{

            //}
            //GUI.DrawTexture(new Rect(position.xMin + 30f, position.yMax, position.width - 30f, 10f), texture);


        }
        
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        
        if (property.isExpanded)
            return EditorGUI.GetPropertyHeight(property) + 0f;

        return EditorGUI.GetPropertyHeight(property);
    }
}