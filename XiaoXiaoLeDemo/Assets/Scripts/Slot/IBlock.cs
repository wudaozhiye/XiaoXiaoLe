using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IBlock : MonoBehaviour {

    public Slot slot;

    public int level = 1;

    abstract public void BlockCrush(bool force);
    abstract public void Initialize();
    abstract public bool CanItContainChip();
    abstract public int GetLevels();

}
