using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using Febucci.UI;

/// <summary>
/// Writes the dialogue to the specified text gameobject.
/// </summary>
[RequireComponent(typeof(TextAnimatorPlayer))]
public class DialogueWriter : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    private Queue<string> sentences;
    public List<AudioClip> voice = new List<AudioClip>();
    public AudioSource audioSource;
    private float wordDelay = .009f;
   
    private string curDialogue;
    public bool doneTalking = false;

    void Start()
    {
        sentences = new Queue<string>();
    }
    //Inside your script

    public TextAnimator textAnimator;
    public TextAnimatorPlayer textAnimatorPlayer;

    //Manage the event subscription

    private void Reset()
    {
        textAnimatorPlayer ??= GetComponent<TextAnimatorPlayer>();
        textAnimator ??= GetComponent<TextAnimator>();
        dialogueText ??= GetComponent<TextMeshProUGUI>();
    }
    private void Awake()
    {
        textAnimator.onEvent += OnEvent;
    }
    private void OnDestroy()
    {
        textAnimator.onEvent -= OnEvent;
    }

    //Do things based on messages
    void OnEvent(string message)
    {
        switch (message)
        {
            case "something":
                //do something
                break;
        }
    }
    public void WriteDialogue(string text)
    {


        voice = null;
        doneTalking = false;
        curDialogue = text;

        sentences = new Queue<string>();
        sentences.Clear();

        sentences.Enqueue(curDialogue);
        DisplayNextSentence();
    }
    public void WriteDialogue(DialogueParts part)
    {
        doneTalking = false;
        curDialogue = part.line;


       // voice = part.characterProfile.voiceNoises;
             
     
        sentences = new Queue<string>();
        sentences.Clear();

        sentences.Enqueue(curDialogue);
        

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if(sentences.Count == 0)
        {
            doneTalking = true;
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        textAnimatorPlayer.ShowText(sentence);

        //StartCoroutine(TypeSentence(sentence));

    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        if (voice != null)
        {
          //  StartCoroutine(PlayVoice());
        }
        foreach(char letter in sentence.ToCharArray())
        {                       
            dialogueText.text += letter;           
            
            yield return new WaitForSeconds(wordDelay);                      
        }        
        doneTalking = true;
    }    

    IEnumerator PlayVoice()
    {
        //Calculate totalTime that the voices can play
        float totalTime = curDialogue.Length * wordDelay;

        //Make an int to track index
        int index = 0;

        //Play initial clip then remove from list
        index = Random.Range(0, voice.Count - 1);
        audioSource.clip = voice[index];

        while (!doneTalking && totalTime > -.5)
        {
            audioSource.pitch = Random.Range(.95f, 1);
            audioSource.Play();                      
            yield return new WaitForSeconds(audioSource.clip.length);

            //Queue new clip and remove it from the list
            int temp = Random.Range(0, voice.Count - 1); 
            
            if(temp == index)
            {
                temp += 1;
                if(temp > voice.Count - 1)
                {
                    temp = 0;
                }                
            }
            index = temp;
            audioSource.clip = voice[index];
            
            //Subtract next clip from total time to make sure the clip wont carry on after text is finished
            totalTime -= audioSource.clip.length;
        }              
    }

    public void FinishNow()
    {
        dialogueText.text = curDialogue;
        StopAllCoroutines();
        doneTalking = true;
    }
}
