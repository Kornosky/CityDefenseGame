using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LoadingScreen : MonoBehaviour
{
    [SerializeField] TMP_Text progressText;
    private void Awake()
    {
        gameObject.SetActive(false);
    }
    public void UpdateGraphics(float progress)
    {
        float progressValue = Mathf.Clamp01(progress / 0.9f);
        progressText.text = Mathf.Round(progressValue * 100) + "%";
    }
    public void Begin(AsyncOperation operation)
    {
        gameObject.SetActive(true);
        StartCoroutine(Load(operation));
    }
    IEnumerator Load(AsyncOperation loadingOperation)
    {
        //reveal loading
        while (!loadingOperation.isDone)
        {
            UpdateGraphics(loadingOperation.progress);
            yield return new WaitForEndOfFrame();
        }
        DoneLoading();
    }

    private void DoneLoading()
    {
        gameObject.SetActive(false);
    }
}
