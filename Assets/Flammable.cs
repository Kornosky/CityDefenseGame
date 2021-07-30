using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flammable : MonoBehaviour
{
    public bool onFire = false;

    public void SetOnFire()
    {
        onFire = true;
        this.Invoke(() => Destroy(this), 2);
    }
}
