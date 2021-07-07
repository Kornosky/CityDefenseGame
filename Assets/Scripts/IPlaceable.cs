using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlaceable
{
    public abstract bool IsValidPosition();
    public abstract void Placing(bool isPlacing);
    public abstract void Built(bool isBuilt);
}
