using System.Collections;
using UnityEngine;
//https://catlikecoding.com/unity/tutorials/movement/orbit-camera/
[RequireComponent(typeof(Camera))]
public class CameraController : SingletonDDOL<CameraController> 
{
      public void CameraShake(float duration = .3f, float shakeAmount = 1f)
    {
        IEnumerator coroutine = CameraShakeRoutine(duration, shakeAmount);
        StartCoroutine(coroutine);
    }

    public IEnumerator CameraShakeRoutine(float duration, float shakeAmount)
    {
        float shakeDuration = duration;
        Vector3 originalPos = transform.localPosition;
        while (shakeDuration > 0)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount * shakeDuration;

            shakeDuration -= Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

    }
}
