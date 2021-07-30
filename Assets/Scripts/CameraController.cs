using BitBenderGames;
using System;
using System.Collections;
using UnityEngine;
//https://catlikecoding.com/unity/tutorials/movement/orbit-camera/
public class CameraController : SingletonDDOL<CameraController> 
{
    [SerializeField] MobileTouchCamera mobileTouchCamera;
    [SerializeField] TouchInputController touchInputController;
    [SerializeField] FocusCameraOnItem focusCameraHelper;
    bool isFocusing = false;
    Vector3 originalPos;
    float originalSize;
    [SerializeField] public Camera cam;
    private void Reset()
    {
        mobileTouchCamera = FindObjectOfType<MobileTouchCamera>();
        touchInputController = FindObjectOfType<TouchInputController>();
        focusCameraHelper = FindObjectOfType<FocusCameraOnItem>();
    }

    private void Awake()
    {
        cam = Camera.main;
        mobileTouchCamera ??= FindObjectOfType<MobileTouchCamera>();
        touchInputController  ??= FindObjectOfType<TouchInputController>();
        focusCameraHelper ??= FindObjectOfType<FocusCameraOnItem>();
        if(mobileTouchCamera == null)
        {
            mobileTouchCamera = cam.GetComponent<MobileTouchCamera>();
            touchInputController = cam.GetComponent<TouchInputController>();
            focusCameraHelper = cam.GetComponent<FocusCameraOnItem>();
        }
        originalPos = cam.transform.position;
        originalSize = cam.orthographicSize; 
    }

    public override void RefreshReferences()
    {
        Awake();
    }
    public Action FocusOnTarget(Transform target)
    {
        Action onEnd = UnlockFromTarget;
        StartCoroutine("Focus", target);
        return onEnd;
    }
    public void EnableCameraTouch(bool isEnabled)
    {
        //TODO this is called by PlayCanvas.cs Visibility() which I want
        if(mobileTouchCamera == null)
            RefreshReferences();
        mobileTouchCamera.enabled = isEnabled;
        //reset position
        if (!isEnabled)
        {
            transform.position = originalPos;
            Camera.main.orthographicSize = originalSize;
        }
    }
    public void ChangeCameraBounds(GameObject[] objects)
    {
        //Get bounds of map for scrolling camera based off of objects
        Bounds bounds = new Bounds(objects[0].transform.position, Vector3.one);
        foreach (var obj in objects)
        {
            bounds.Encapsulate(obj.GetComponentInChildren<Renderer>().bounds);
        }

        mobileTouchCamera.BoundaryMax = bounds.max;
        mobileTouchCamera.BoundaryMin = bounds.min;
    }
    public void ChangeCameraBounds(Bounds bounds)
    {
        mobileTouchCamera.BoundaryMax = bounds.max;
        mobileTouchCamera.BoundaryMin = bounds.min;
    }
    public void LockCameraControl(bool isLocked)
    {
        touchInputController.IsInputOnLockedArea = isLocked;
        Debug.LogWarning("Input is locked? : " + isLocked);
    }

    public void UnlockFromTarget()
    {
        isFocusing = false;
    }
    IEnumerator Focus(Transform target)
    {
        isFocusing = true;

        while (isFocusing)
        {
            focusCameraHelper.FocusCameraOnTransform(target);

            yield return new WaitForEndOfFrame();
        }
    }
    public void CameraShake(float duration = .3f, float shakeAmount = 1f)
    {
        IEnumerator coroutine = CameraShakeRoutine(duration, shakeAmount);
        StartCoroutine(coroutine);
    }

    public IEnumerator CameraShakeRoutine(float duration, float shakeAmount)
    {
        float shakeDuration = duration;
        Vector3 originalPos = Camera.main.transform.localPosition;
        while (shakeDuration > 0)
        {
            Camera.main.transform.localPosition = originalPos + UnityEngine.Random.insideUnitSphere * shakeAmount * shakeDuration;

            shakeDuration -= Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

    }
    protected override void OnDestroy()
    {
        Awake();
    }
}
