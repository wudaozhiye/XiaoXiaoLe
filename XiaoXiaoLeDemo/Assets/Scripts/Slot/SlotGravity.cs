using Berry.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Slot))]
public class SlotGravity : MonoBehaviour {

    public Slot slot;

    public Side gravityDirection = Side.Null;
    public Side fallingDirection = Side.Null;

    // No shadow - is a direct path from the slot up to the slot with a component SlotGenerator. Towards must have slots (without blocks and wall)
    // This concept is very important for the proper physics chips
    public bool shadow;

    void Awake()
    {
        slot = GetComponent<Slot>();
    }

    public static void Reshading()
    {
        foreach (SlotGravity sg in GameObject.FindObjectsOfType<SlotGravity>())
        {
            sg.shadow = true;
        }

        Slot slot;
        List<Slot> stock = new List<Slot>();
        List<SlotGenerator> generator = new List<SlotGenerator>(GameObject.FindObjectsOfType<SlotGenerator>());
    }
}
