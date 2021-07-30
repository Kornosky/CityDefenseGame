using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Makes unique, default profiles for each character during dialogue.
/// </summary>

[CreateAssetMenu(menuName = "CharacterProfileInfo")]

[System.Serializable]
public class CharacterProfileInfo : ScriptableObject
{
    public string characterName;
    public Texture image;
    public List<AudioClip> voiceNoises = new List<AudioClip>();
}
