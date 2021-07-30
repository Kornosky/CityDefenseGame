using Febucci.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : SingletonDDOL<CutsceneManager>
{
    [SerializeField] DialogueManager tAnimator;
    [SerializeField] DialogueInfo info;
    private void Reset()
    {
        tAnimator ??= FindObjectOfType<DialogueManager>();
    }
    protected void Start()
    {
    }
    public void BeginScene(DialogueInfo info)
    {
        tAnimator.StartDialogue(info);
    }
    public void EndScene()
    {

    }
}
