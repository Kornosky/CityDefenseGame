using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds the data for creating new dialogue events.
/// </summary>

[CreateAssetMenu(menuName = "Infos/DialogueInfo")]
public class DialogueInfo : ScriptableObject
{
    public List<DialogueParts> dialogue;
}

[System.Serializable]
public class DialogueParts 
{
    public CharacterProfileInfo characterProfile;    
    [TextArea]
    public string line;

    [Space]
    
    [Header("Overrides")]
    public string characterName;
    public Texture image;
    public AudioClip voice;

    [Header("Common")]
    public bool rightSide;
    public bool leftSide;

    [Header("Advanced")]
    public bool leftSideOnly;
    public bool rightSideOnly;
    public bool removeAllCharactersFromScreen;
    public bool keepBothCharacters;

    [Header("Effects")]
    public bool slidesInSide;
    public bool slidesInBottom;
    public bool leftSideSlidesOff;
    public bool rightSideSlidesOff;
    [Space]
    public bool dullAllCharacters;
    public bool shakeDialogueBox;
    [Space]
    public string animationToPlay;
}



