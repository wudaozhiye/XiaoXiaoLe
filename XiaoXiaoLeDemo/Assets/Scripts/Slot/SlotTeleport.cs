using Berry.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotTeleport : MonoBehaviour {

    public Slot target;
    public Slot slot;

    public Int2 target_postion = null;

    float lastTime = -10;
    float delay = 0.15f; // delay between the generations
    void Start()
    {
        slot = GetComponent<Slot>();
        slot.slotTeleport = this;
    }
}
