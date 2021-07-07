using Pixelplacement;
using Pixelplacement.TweenSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barricade : Structure, IPlaceable
{
    private TweenBase placingTween;

    public bool IsValidPosition()
    {
        Collider2D myColl = GetComponent<Collider2D>();
        Bounds bound = myColl.bounds;
        var hitColliders = Physics2D.BoxCastAll(transform.position, bound.size, 0, new Vector2(0, -1));
        bool canPlace = false;
        foreach (var coll in hitColliders)
        {
            //coll.transform.gameObject.GetComponent<Unit>().isEnemy != isEnemy
            if (coll.transform.CompareTag("Ground"))
            {
                canPlace = true;
                break;
            }
        }
      
        if(canPlace)
            spriteRenderer.color = Color.green;
        else
            spriteRenderer.color = Color.red;

        return canPlace;
    }

    public void Placing(bool isPlacing)
    {
        //do placing effects

        if (isPlacing)
        {
            Activate(false);
            gameObject.layer = LayerMask.NameToLayer("Placing");
            placingTween = Tween.LocalScale(spriteRenderer.transform, Vector3.one * 1.2f, .2f, 0, Tween.EaseOutStrong, Tween.LoopType.PingPong);
            hpBar.gameObject.SetActive(false);
        }
        else
        {
            spriteRenderer.color = Color.white;
            placingTween?.Cancel();
            rb.isKinematic = false;
            Activate(true);
            //Needs to be built
            if(info.buildTime > 0)
            {
                Unbuilt();
                GameManager.Instance.AddStructureToQueue(this);
            }
        }
    }


    public void Built(bool isBuilt)
    {
        throw new System.NotImplementedException();
    }

    protected override void Action()
    {
        throw new System.NotImplementedException();
    }

    public override void TryAction()
    {
        throw new System.NotImplementedException();
    }
}
