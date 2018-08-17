using Berry.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldAssistant : MonoBehaviour {

    public static FieldAssistant main;

    [HideInInspector]
    public Field field;

    void Awake()
    {
        main = this;
    }
    public void StartLevel()
    {
        StartCoroutine(StartLevelRoutine());
    }
    IEnumerator StartLevelRoutine()
    {
        UIAssistant.main.ShowPage("Loading");

        while (CPanel.uiAnimation > 0)
            yield return 0;

        ProfileAssistant.main.local_profile["life"]--;

        SessionAssistant.main.enabled = false;

        SessionAssistant.Reset();

        yield return StartCoroutine(CreateField());

        SessionAssistant.main.enabled = true;
        SessionAssistant.main.eventCount++;

        SessionAssistant.main.StartSession(LevelProfile.main.target, LevelProfile.main.limitation);

        GameCamera.main.transform.position = new Vector3(0, 20, -10);
    }
    public IEnumerator CreateField()
    {
        RemoveField(); // Removing old field
        field = new Field(LevelProfile.main.GetClone());

        Slot.folder = new GameObject().transform;
        Slot.folder.name = "Slots";

        Slot.all.Clear();

        Vector3 fieldDimensions = new Vector3(field.width - 1, field.height - 1, 0) * ProjectParameters.main.slot_offset;
        foreach (SlotSettings settings in field.slots.Values)
        {
            yield return 0;
            Slot slot;

            #region Creating a new empty slot
            Vector3 position = new Vector3(settings.position.x, settings.position.y, 0) * ProjectParameters.main.slot_offset - fieldDimensions / 2;
            GameObject obj = ContentAssistant.main.GetItem("SlotEmpty", position);
            obj.name = "Slot_" + settings.position.x + "x" + settings.position.y;
            obj.transform.SetParent(Slot.folder);
            slot = obj.GetComponent<Slot>();
            slot.coord = settings.position;
            Slot.all.Add(slot.coord, slot);
            #endregion

            #region Creating a generator
            if (settings.generator)
                slot.gameObject.AddComponent<SlotGenerator>();
            #endregion

            #region Creating a teleport
            if (settings.teleport != Int2.Null)
                slot.slotTeleport.target_postion = settings.teleport;
            else
                Destroy(slot.slotTeleport);
            #endregion

            #region Setting gravity direction
            slot.slotGravity.gravityDirection = settings.gravity;
            #endregion

            #region Setting sugar target (by slot tag)
            if (LevelProfile.main.target == FieldTarget.SugarDrop && settings.tags.Contains("SugarDrop"))
            {
                slot.sugarDropSlot = true;
                GameObject sd = ContentAssistant.main.GetItem("SugarDrop", position);
                sd.name = "SugarDrop";
                sd.transform.parent = slot.transform;
                sd.transform.localPosition = Vector3.zero;
                sd.transform.Rotate(0, 0, Utils.SideToAngle(settings.gravity) + 90);
            }
            #endregion

            #region Creating a block
            if (settings.block_type != "")
            {
                GameObject b_obj = ContentAssistant.main.GetItem(settings.block_type);
                b_obj.transform.SetParent(slot.transform);
                b_obj.transform.localPosition = Vector3.zero;
                b_obj.name = settings.block_type + "_" + settings.position.x + "x" + settings.position.y;
                IBlock block = b_obj.GetComponent<IBlock>();
                slot.block = block;
                block.slot = slot;
                block.level = settings.block_level;
                block.Initialize();
            }
            #endregion

            #region Create a jelly
            if (LevelProfile.main.target == FieldTarget.Jelly && settings.jelly_level > 0)
            {
                GameObject j_obj;
                j_obj = ContentAssistant.main.GetItem(settings.jelly_level == 1 ? "SingleLayerJelly" : "Jelly");
                j_obj.transform.SetParent(slot.transform);
                j_obj.transform.localPosition = Vector3.zero;
                j_obj.name = "Jelly_" + settings.position.x + "x" + settings.position.y;
                Jelly jelly = j_obj.GetComponent<Jelly>();
                jelly.level = settings.jelly_level;
                slot.jelly = jelly;
            }
            #endregion

            #region Create a jam
            if (LevelProfile.main.target == FieldTarget.Jam || LevelProfile.main.target == FieldTarget.Duel)
            {
                Jam.JamIt(slot, settings.jam);
            }
            #endregion

            #region Create a chip
            if (!string.IsNullOrEmpty(settings.chip) && (slot.block == null || slot.block.CanItContainChip()))
            {
                SessionAssistant.ChipInfo chipInfo = SessionAssistant.main.chipInfos.Find(x => x.name == settings.chip);
                if (chipInfo != null)
                {
                    string key = chipInfo.contentName + (chipInfo.color ? Chip.chipTypes[Mathf.Clamp(settings.color_id, 0, Chip.colors.Length - 1)] : "");
                    GameObject c_obj = ContentAssistant.main.GetItem(key);
                    c_obj.transform.SetParent(slot.transform);
                    c_obj.transform.localPosition = Vector3.zero;
                    c_obj.name = key;
                    slot.chip = c_obj.GetComponent<Chip>();
                }
            }
            #endregion

        }

    }
    // Removing old field
    public void RemoveField()
    {
        if (Slot.folder)
            Destroy(Slot.folder.gameObject);
    }
}
public class Field
{
    public int width;
    public int height;
    public int colorCount;
    public Dictionary<Int2, SlotSettings> slots = new Dictionary<Int2, SlotSettings>();
    public List<Int2> wall_horizontal = new List<Int2>();
    public List<Int2> wall_vertical = new List<Int2>();
    public int targetValue = 0;

    public Field(LevelProfile profile)
    {
        width = profile.width;
        height = profile.height;
        colorCount = profile.colorCount;
        foreach (SlotSettings slot in profile.slots)
            if (!slots.ContainsKey(slot.position))
                slots.Add(slot.position, slot.GetClone());
        wall_horizontal = new List<Int2>(profile.wall_horizontal);
        wall_vertical = new List<Int2>(profile.wall_vertical);
        FirstChipGeneration();
    }
    int NewRandomChip(Int2 coord)
    {
        List<int> ids = new List<int>();
        for (int i = 0; i < colorCount; i++)
            ids.Add(i + 1);

        foreach (Side side in Utils.straightSides)
            if (slots.ContainsKey(coord + side) && ids.Contains(slots[coord + side].color_id))
                ids.Remove(slots[coord + side].color_id);

        if (ids.Count > 0)
            return ids.GetRandom();
        else
            return Random.Range(0, colorCount);
    }
    public void FirstChipGeneration()
    {
        // replace random chips on nonrandom
        foreach (Int2 pos in slots.Keys)
            if (slots[pos].color_id == 0)
                slots[pos].color_id = NewRandomChip(pos);

        foreach (Int2 pos in slots.Keys)
            if (slots[pos].color_id > 0)
                slots[pos].color_id--;

        int[] a = new int[Chip.colors.Length];
        // a => 0, 1, 2, 3, 4...
        for (int i = 0; i < a.Length; i++)
            a[i] = i;

        for (int i = Chip.colors.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i);
            a[j] = a[j] + a[i];
            a[i] = a[j] - a[i];
            a[j] = a[j] - a[i];
        }

        SessionAssistant.main.colorMask = a;
    }
}
public enum FieldTarget
{
    None = 0,
    Jelly = 1,
    Block = 2,
    Color = 3,
    SugarDrop = 4,
    Jam = 5,
    Duel = 6
}
public enum Limitation
{
    Moves,
    Time
}