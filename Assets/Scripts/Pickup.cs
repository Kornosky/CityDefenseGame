using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
    [SerializeField] protected Rigidbody2D rb;
    protected Transform goalTransform;
    protected Vector3 pickupLocation;
    private Vector3 midPointLocation;

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Pickup");
        GetComponent<SpriteRenderer>().sortingLayerName = "Pickup";
    }
    protected virtual void Start()
    {
        //GetComponent<SpriteRenderer>().color = Random.ColorHSV(0f, 0, .8f, 1, .8f, 1);
        Vector2 randomUnitCirclePoint = Random.insideUnitCircle * .5f;
        //Hemicirlce that's moved down
        randomUnitCirclePoint = new Vector2(randomUnitCirclePoint.x, Mathf.Abs(randomUnitCirclePoint.y)) - Vector2.one * .2f;
        rb.AddExplosionForce(1, (Vector3)randomUnitCirclePoint + transform.position, 1, .3f, ForceMode2D.Impulse);
    }
    public virtual void OnPressed()
    {
        OnCollect();
    }

    protected virtual void OnCollect()
    {
        rb.isKinematic = true;

        Destroy(GetComponent<Collider2D>());

        //Calculate "Bulge" location
        pickupLocation = transform.position;
        Vector3 goalPos = CameraController.Instance.cam.ScreenToWorldPoint(new Vector3(goalTransform.position.x, goalTransform.position.y, CameraController.Instance.cam.nearClipPlane));
        Vector3 mid = goalPos - pickupLocation;
        midPointLocation = new Vector2(-mid.y, mid.x) * Random.Range(-.3f, .3f);
        midPointLocation += pickupLocation + Vector3.Lerp(pickupLocation, goalPos, .7f); //how far along is the "bulge" point
        
        //move to top right
        Tween.Value(0f, 1f, ToPosition, 1.5f, 0, Tween.EaseOutStrong, Tween.LoopType.None, null, () => Destroy(gameObject));
    }

    void ToPosition(float alpha)
    {
        if(alpha <= .5f)
        {
            ///go to random midpoint
            transform.position = Vector3.Lerp(pickupLocation, midPointLocation, alpha * 2);
        }
        else
        {
            alpha -= .5f;
            transform.position = Vector3.Lerp(midPointLocation, CameraController.Instance.cam.ScreenToWorldPoint(goalTransform.position), alpha * 2);
        }
    }
}
