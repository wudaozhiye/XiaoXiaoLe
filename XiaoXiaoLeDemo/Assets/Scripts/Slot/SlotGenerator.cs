using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Slot which generates new simple chips.
[RequireComponent(typeof(Slot))]
[RequireComponent(typeof(Slot))]
public class SlotGenerator : MonoBehaviour {

    public Slot slot;
    public Chip chip;

    float lastTime = -10;
    float delay = 0.15f; // delay between the generations

    void Awake()
    {
        slot = GetComponent<Slot>();
        slot.generator = true;
    }
    void Update()
    {
        //if (!SessionAssistant.main.enabled) return;

        //if (slot.chip) return; // Generation is impossible, if slot already contains chip

        //if (slot.block) return; // Generation is impossible, if the slot is blocked


    }
}
