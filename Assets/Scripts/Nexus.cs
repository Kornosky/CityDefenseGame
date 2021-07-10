using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Nexus : Structure
{
    protected override void Start()
    {
        base.Start();
        Init(isEnemy, info);
    }
    public override void TryAction()
    {
       // throw new System.NotImplementedException();
    }

    protected override void Action()
    {
       // throw new System.NotImplementedException();
    }

    public override void Death()
    {
        base.Death();

        PlayerManager.Instance.GameOver(isEnemy);
    }
}
