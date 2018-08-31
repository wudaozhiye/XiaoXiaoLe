using Berry.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour {

    public static Dictionary<Int2, Slot> all = new Dictionary<Int2, Slot>();

    public bool generator = false;
    public bool teleportTarget = false;

    public Jelly jelly; // Jelly for this slot
    public Jam jam;
    public IBlock block; // Block for this slot

    // Position of this slot
    public Int2 coord = new Int2();

    public int SlotX { get { return coord.x; } }
    public int SlotY { get { return coord.y; } }
    public Slot this[Side index]
    { // access to neighby slots on the index
        get
        {
            return nearSlot[index];
        }
    }
    public Dictionary<Side, Slot> nearSlot = new Dictionary<Side, Slot>(); // Nearby slots dictionary
    public Dictionary<Side, bool> wallMask = new Dictionary<Side, bool>();

    public SlotGravity slotGravity;
    public SlotTeleport slotTeleport;

    public bool sugarDropSlot = false;
    public static Transform folder;
    void Awake()
    {
        slotGravity = GetComponent<SlotGravity>();
        slotTeleport = GetComponent<SlotTeleport>();
    }
    public static void Initialize()
    {
        foreach (Slot slot in FindObjectsOfType<Slot>())
            if (!all.ContainsKey(slot.coord))
                all.Add(slot.coord, slot);


        foreach (Slot slot in all.Values)
        {
            foreach (Side side in Utils.allSides) // Filling of the nearby slots dictionary 
                slot.nearSlot.Add(side, all.ContainsKey(slot.coord + side) ? all[slot.coord + side] : null);
            slot.nearSlot.Add(Side.Null, null);
            foreach (Side side in Utils.straightSides) // Filling of the walls dictionary
                slot.wallMask.Add(side, false);
        }

        Side direction;
        SlotTeleport teleport;
        foreach (Slot slot in all.Values)
        {
            direction = slot.slotGravity.gravityDirection;
            if (slot[direction])
            {
                slot[direction].slotGravity.fallingDirection = Utils.MirrorSide(direction);
            }
            teleport = slot.GetComponent<SlotTeleport>();
            if (teleport)
                teleport.Initialize();
        }
    }

    Chip _chip;
    public Chip chip
    {
        get
        {
            return _chip;
        }
        set
        {
            if (value == null)
            {
                if (_chip)
                    _chip.slot = null;
                _chip = null;
                return;
            }
            if (_chip)
                _chip.slot = null;
            _chip = value;
            _chip.transform.parent = transform;
            if (_chip.slot)
                _chip.slot.chip = null;
            _chip.slot = this;
        }
    }
    public static Slot GetSlot(Int2 position)
    {
        if (all.ContainsKey(position))
            return all[position];
        return null;
    }
}
