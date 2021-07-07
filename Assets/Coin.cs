using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Coin for earning money. They are placing phyiscally in front (z pos) for OnMouseOver()
/// </summary>
public class Coin : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    private void Start()
    {
        //GetComponent<SpriteRenderer>().color = Random.ColorHSV(0f, 0, .8f, 1, .8f, 1);
        Vector2 randomUnitCirclePoint = Random.insideUnitCircle * .5f;
        //Hemicirlce that's moved down
        randomUnitCirclePoint = new Vector2(randomUnitCirclePoint.x, Mathf.Abs(randomUnitCirclePoint.y)) - Vector2.one * .2f;
        rb.AddExplosionForce(1, (Vector3) randomUnitCirclePoint + transform.position, 1, .3f, ForceMode2D.Impulse);
    }
    private void OnMouseOver()
    {
        OnCollect();
    }

    void OnCollect()
    {
        rb.isKinematic = true;
        GameManager.Instance.Money += 1;
        Destroy(GetComponent<Collider2D>());

        Vector3 goal = Camera.main.ScreenToWorldPoint(GameManager.Instance.moneyText.transform.position);
        //move to top right
        Tween.Position(transform, goal, .5f, 0, Tween.EaseOutStrong, Tween.LoopType.None, null, () => Destroy(gameObject));
    }
}
