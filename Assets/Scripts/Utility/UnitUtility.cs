using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnitUtility 
{
    public static void ChangeLayer(GameObject gameObject, bool isEnemy, string layer = "")
    {
        if (layer != "")
        {
            gameObject.layer = LayerMask.NameToLayer(layer);

            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer(layer);
            }
        }
        else if (isEnemy)
        {
            gameObject.layer = LayerMask.NameToLayer("Enemy");

            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Enemy");
            }
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Player");
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Player");
            }
        }
    }
}
