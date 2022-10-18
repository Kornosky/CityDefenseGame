using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireArrow : Arrow
{
    [SerializeField] Fire fire;

    private void Awake()
    {
        //fire = GetComponent<Fire>();
        fire.gameObject.SetActive(false);
    }

    private void Start()
    {
        Invoke("Activate", .2f);
    }
    void Activate()
    {
        fire.gameObject.SetActive(true);
    }
    public override void OnHit(Collision2D target)
    {
        fire.transform.SetParent(target.transform);
        fire.transform.position = target.transform.position;
        fire.BeginBurn();
    }
}
