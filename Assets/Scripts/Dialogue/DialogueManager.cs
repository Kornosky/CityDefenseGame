using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pixelplacement;
/// <summary>
/// Manages the characters talking and communicates with DialogueWriter.cs. Images must have a height of 1000px or similar.
/// </summary>

public enum FocusedCharacter { Left, Right, Both, None }

public class DialogueManager : MenuElement
{
    [SerializeField]
    public RawImage rightCharacter;
    [SerializeField]
    public CutsceneManager cutsceneManager;

    public RawImage leftCharacter;
    [SerializeField]
    public TextMeshProUGUI talkerName;
    [SerializeField]
    public GameObject dialogueBox;
    [SerializeField]
    public Animator charAnim;

    public List<Image> images = new List<Image>();

    public Animator boxAnim;
    public Animator leftAnim;
    public Animator rightAnim;
    [SerializeField]
    private DialogueWriter writer;
    private bool nextLine;
    
    private bool switchImage;

    public bool stillTyping;

    protected override void Awake()
    {
        base.Awake();
        foreach(var cat in GetComponentsInChildren<Image>())
        {
            images.Add(cat);
        }
    }
    
    //Make sure everything is set per instance
    private void Initialize(DialogueInfo dialogueSO)
    {
        Visibility(true, true, .3f);
        nextLine = false;
        rightCharacter.enabled = false;
        leftCharacter.enabled = false;
        dialogueBox.GetComponent<Image>().enabled = true;
        foreach(TextMeshProUGUI text in dialogueBox.GetComponentsInChildren<TextMeshProUGUI>())
        {
            text.enabled = true;
        }
    }

    public void StartDialogue(DialogueInfo dialogueSO)
    {
        Initialize(dialogueSO);
        Visibility(true);
        StartCoroutine(WaitForClick(dialogueSO));     
    }

    IEnumerator WaitForClick(DialogueInfo dialogueSO)
    {
        switchImage = false;
        Texture curImage;
        string curName;
        foreach (DialogueParts parts in dialogueSO.dialogue)
        {
            if(parts.animationToPlay != string.Empty)
            {
                cutsceneManager.gameObject.GetComponent<Animator>().Play(parts.animationToPlay);
            }
            string nameToUse = " ";
            if (parts.characterName != string.Empty)
            {
                nameToUse = parts.characterName;
            }

            //IF OVERRIDES PRESENT
            if(parts.image != null)
            {
                 curImage = parts.image;
            }
            else
            {                
                 curImage = parts.characterProfile.image;
            }
            if(parts.characterName != null)
            {
                 curName = parts.characterName;
            }
            else
            {
                 curName = parts.characterProfile.characterName;
            }      


            if (parts.leftSide)
            {
                leftCharacter.enabled = true;
                leftCharacter.texture = curImage;
                leftCharacter.SetNativeSize();
                
                Effects(parts,FocusedCharacter.Left);
                
                switchImage = false; //Make sure it defaults to the next side
            }
            if (parts.leftSideOnly)
            {
                rightCharacter.enabled = false;
                leftCharacter.enabled = true;
                leftCharacter.texture = curImage;
                leftCharacter.SetNativeSize();
                
                Effects(parts, FocusedCharacter.Left);
                switchImage = false; //Make sure it defaults to the next side
            }
            else if(parts.rightSide)
            {
                rightCharacter.enabled = true;
                rightCharacter.texture = curImage;                
                rightCharacter.SetNativeSize();
                rightCharacter.GetComponent<RectTransform>().sizeDelta = new Vector2(-1 * rightCharacter.GetComponent<RectTransform>().sizeDelta.x, rightCharacter.GetComponent<RectTransform>().sizeDelta.y); //IDK ITS STUPID LIKE THIS
               
                Effects(parts, FocusedCharacter.Right);
                switchImage = true; //Make sure it defaults to the next side
            }
            else if (parts.rightSideOnly)
            {
                leftCharacter.enabled = false;
                rightCharacter.enabled = true;
                rightCharacter.texture = curImage;
                rightCharacter.SetNativeSize();
                rightCharacter.GetComponent<RectTransform>().sizeDelta = new Vector2(-1 * rightCharacter.GetComponent<RectTransform>().sizeDelta.x, rightCharacter.GetComponent<RectTransform>().sizeDelta.y); //IDK ITS STUPID LIKE THIS
               
                Effects(parts, FocusedCharacter.Right);
                switchImage = true; //Make sure it defaults to the next side
            }
            else if(parts.keepBothCharacters)
            {
                rightCharacter.enabled = true;
                leftCharacter.enabled = true;
                Effects(parts, FocusedCharacter.Both);
            }
            else if(parts.dullAllCharacters)
            {
                rightCharacter.enabled = true;
              //  rightCharacter.texture = parts.image;
               // rightCharacter.SetNativeSize();
              //  rightCharacter.GetComponent<RectTransform>().sizeDelta = new Vector2(-1 * rightCharacter.GetComponent<RectTransform>().sizeDelta.x, rightCharacter.GetComponent<RectTransform>().sizeDelta.y); //IDK ITS STUPID LIKE THIS
                leftCharacter.enabled = true;
                Tween.Color(rightCharacter, Color.grey, .3f, 0f);
                Tween.Color(leftCharacter, Color.grey, .3f, 0f);
                // leftCharacter.texture = parts.image;
                // leftCharacter.SetNativeSize();
                Effects(parts, FocusedCharacter.Both);
            }
            else if(parts.removeAllCharactersFromScreen)
            {
                leftCharacter.enabled = false;
                rightCharacter.enabled = false;
                Effects(parts, FocusedCharacter.None);
            }
            else if(switchImage)
            {                
                rightCharacter.enabled = true;                
                rightCharacter.texture = curImage;
                rightCharacter.SetNativeSize();
                rightCharacter.GetComponent<RectTransform>().sizeDelta = new Vector2(-1 * rightCharacter.GetComponent<RectTransform>().sizeDelta.x, rightCharacter.GetComponent<RectTransform>().sizeDelta.y); //IDK ITS STUPID LIKE THIS
                
                Effects(parts, FocusedCharacter.Right);
            }
            else
            {
                leftCharacter.enabled = true;
                leftCharacter.texture = curImage;
                leftCharacter.SetNativeSize();
               
                Effects(parts, FocusedCharacter.Left);
            }
            talkerName.text = nameToUse;
            writer.WriteDialogue(parts);
            yield return new WaitUntil(() => nextLine == true);
            nextLine = false;
            switchImage = !switchImage;
        }

        CloseDialogue();
    }

    //Manage misc effects. 
    public void Effects(DialogueParts parts, FocusedCharacter focus)
    {
        if (parts.slidesInSide)
        {
            if(focus == FocusedCharacter.Left)
            {
                leftAnim.Play("LeftSlideSide");
            }
            else if(focus == FocusedCharacter.Right)
            {
                rightAnim.Play("RightSlideSide");
            }
        }
        else if(parts.slidesInBottom)
        {
            if (focus == FocusedCharacter.Left)
            {
                leftAnim.Play("LeftSlideBottom");
            }
            else if (focus == FocusedCharacter.Right)
            {
                rightAnim.Play("RightSlideBottom");
            }
        }
        
        if(parts.rightSideSlidesOff) //incomplete
        {

            rightAnim.Play("RightSlideSideOut");             


        }
        if (parts.leftSideSlidesOff) //incomplete
        {

            leftAnim.Play("LeftSlideSideOut");
            
        }
        if (parts.shakeDialogueBox)
        {
            boxAnim.Play("DialogueBoxShake");
        }
    }


    public void OnClick()
    {
        if(writer.doneTalking)
        {
            nextLine = true;
        }
        else
        {
            writer.FinishNow();
        }
    }

    private void CloseDialogue()
    {
        Visibility(false, true, .3f);
    }
    


}

